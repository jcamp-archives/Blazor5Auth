using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blazor5Auth.Server.Models;
using Blazor5Auth.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Blazor5Auth.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private static UserModel LoggedOutUser = new UserModel { IsAuthenticated = false };

        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegisterModel model)
        {
            var newUser = new ApplicationUser { UserName = model.Email, Email = model.Email };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description);

                return Ok(new RegisterResult { Successful = false, Errors = errors });
            }

            // Add all new users to the User role
            await _userManager.AddToRoleAsync(newUser, "User");

            // Add new users whose email starts with 'admin' to the Admin role
            if (newUser.Email.StartsWith("admin"))
            {
                await _userManager.AddToRoleAsync(newUser, "Admin");
            }

            return Ok(new RegisterResult { Successful = true });
        }

        [HttpPost("confirmemail")]
        public async Task<IActionResult> PostConfirmEmail(ConfirmModel model)
        {
            if (model.UserId == null || model.Code == null)
            {
                return BadRequest(new VerifyMfaResult {Successful = false});
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return BadRequest(new VerifyMfaResult {Successful = false});
            }

            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            var statusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";

            if (!result.Succeeded)
            {
                return BadRequest(new VerifyMfaResult {Successful = false, Status = statusMessage});
            }
            
            return Ok(new VerifyMfaResult {Successful = true, Status = statusMessage});
        }

        [HttpGet("userprofile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new UserProfileModel() {Email = user.Email, PhoneNumber = user.PhoneNumber};

            return Ok(model);
        }

        
        [HttpPost("userprofile")]
        public async Task<IActionResult> PostUserProfile(UserProfileModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var statusMessage = ""; 
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    statusMessage = "Unexpected error when trying to set phone number.";
                }
                else
                {
                    statusMessage = "Your profile has been updated";
                }
            }

            return Ok(new VerifyMfaResult() {Successful = true, Status = statusMessage});
        }
    }
}
