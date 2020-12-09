using System.Threading.Tasks;
using Features.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Features.Account.Manage
{
    [Authorize]
    [Route("api/account/[controller]/[action]")]
    public class ManageController : MediatrControllerBase
    {
        public ManageController(ISender sender) : base(sender) { }

        [HttpGet]
        public async Task<IActionResult> UserProfile() => await Send(new UserProfile.Query());

        [HttpPost]
        public async Task<IActionResult> UserProfile(UserProfile.Command model) => await Send(model);

        [HttpGet]
        public async Task<IActionResult> MfaInfo() => await Send(new MfaInfo.Query());

        [HttpGet]
        public async Task<IActionResult> MfaEnable() => await Send(new MfaEnable.Query());

        [HttpPost]
        public async Task<IActionResult> MfaEnable(MfaEnable.Command command) => await Send(command);

        [HttpPost]
        public async Task<IActionResult> MfaDisable() => await Send(new MfaDisable.Command());

        [HttpPost]
        public async Task<IActionResult> MfaForgetBrowser() => await Send(new MfaForgetBrowser.Command());

        [HttpPost]
        public async Task<IActionResult> MfaGenerateCodes() => await Send(new MfaGenerateCodes.Command());

        [HttpPost]
        public async Task<IActionResult> MfaResetKey() => await Send(new MfaResetKey.Command());

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(ChangeEmail.Command command) => await Send(command);

        [HttpPost]
        public async Task<IActionResult> SendVerificationEmail() => await Send(new SendVerificationEmail.Command());

        [HttpPost]
        public async Task<IActionResult> ConfirmEmailChange(ConfirmEmailChange.Command model) {
            model.ClientAuth = true;
            return await Send(model);
        }

    }
}
