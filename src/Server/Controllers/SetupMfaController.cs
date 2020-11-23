using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Blazor5Auth.Server.Models;
using Blazor5Auth.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace Blazor5Auth.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetupMfaController : ControllerBase
    {
        private static UserModel LoggedOutUser = new UserModel { IsAuthenticated = false };

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UrlEncoder _urlEncoder;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public SetupMfaController(UserManager<ApplicationUser> userManager, UrlEncoder urlEncoder)
        {
            _userManager = userManager;
            _urlEncoder = urlEncoder;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = new SetupMfaModel();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadSharedKeyAndQrCodeUriAsync(user, result);

            result.QrCodeBase64 = CreateQRCode(result.AuthenticatorUri);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] VerifyMfaModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return Ok(new VerifyMfaResult { Successful = false, Error = ModelState.First().Value.Errors[0].ErrorMessage });
            }

            // Strip spaces and hypens
            var verificationCode = model.Verification.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Verification", "Verification code is invalid.");
                return Ok(new VerifyMfaResult { Successful = false, Error = ModelState.First().Value.Errors[0].ErrorMessage });
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            return Ok(new VerifyMfaResult { Successful = true, Status = "Your authenticator app has been verified." });
        }

        private static string CreateQRCode(string text)
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.L);
            var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(4);
            return Convert.ToBase64String(qrCodeImage);
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user, SetupMfaModel model)
        {
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            model.SharedKey = FormatKey(unformattedKey);

            var email = await _userManager.GetEmailAsync(user);
            model.AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey);
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            var currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("Blazor5Auth.Server"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

    }
}
