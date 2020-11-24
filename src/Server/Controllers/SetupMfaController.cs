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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SetupMfaController : ControllerBase
    {
        private static UserModel LoggedOutUser = new UserModel { IsAuthenticated = false };

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UrlEncoder _urlEncoder;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public SetupMfaController(UserManager<ApplicationUser> userManager, UrlEncoder urlEncoder, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _urlEncoder = urlEncoder;
            _signInManager = signInManager;
        }

        [HttpGet("getinfo")]
        public async Task<IActionResult> GetInfo()
        {
            var result = new MfaInfoModel();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            result.HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null;
            result.Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            result.IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user);
            result.RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user);

            return Ok(result);
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

        [HttpPost("resetkey")]
        public async Task<IActionResult> PostResetKey()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            await _signInManager.RefreshSignInAsync(user);

            var statusMessage = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";

            return Ok(new VerifyMfaResult { Successful = true, Status = statusMessage });
        }

        [HttpPost("disable")]
        public async Task<IActionResult> PostDisable()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred disabling 2FA for user with ID '{_userManager.GetUserId(User)}'.");
            }

            var statusMessage = "2fa has been disabled. You can reenable 2fa when you setup an authenticator app";

            return Ok(new VerifyMfaResult { Successful = true, Status = statusMessage });
        }

        [HttpPost("generatecodes")]
        public async Task<IActionResult> PostGenerateCodes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!isTwoFactorEnabled)
            {
                throw new InvalidOperationException($"Cannot generate recovery codes for user with ID '{userId}' as they do not have 2FA enabled.");
            }

            var result = new RecoveryCodesResult
            {
                Successful = true,
                RecoveryCodes = (await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10)).ToArray(),
                Status = "You have generated new recovery codes."
            };

            return Ok(result);
        }

        [HttpPost("forgetbrowser")]
        public async Task<IActionResult> PostForgetBrowser()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _signInManager.ForgetTwoFactorClientAsync();

            var result = new VerifyMfaResult
            {
                Successful = true,
                Status = "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code."
            };

            return Ok(result);
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
