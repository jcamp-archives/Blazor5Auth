using System.Threading.Tasks;
using Features.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Features.Account
{
    public class AccountController : MediatrControllerBase
    {
        public AccountController(ISender sender) : base(sender) { }

        [HttpPost]
        public async Task<IActionResult> Login(LoginPassword.Command model) => await Send(model);

        [HttpPost]
        public async Task<IActionResult> CheckMfa(LoginMultiFactor.Query model) => await Send(model);

        [HttpPost]
        public async Task<IActionResult> LoginMfa(LoginMultiFactor.Command model) => await Send(model);

        [HttpPost]
        public async Task<IActionResult> LoginRecovery(LoginRecoveryCode.Command model) => await Send(model);

        [HttpPost]
        public async Task<IActionResult> Register(Register.Command model) => await Send(model);

        [HttpPost]
        public async Task<IActionResult> ConfirmEmailChange(ConfirmEmailChange.Command model) {
            model.ClientAuth = false;
            return await Send(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmail.Command model) => await Send(model);

        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmation.Command model) => await Send(model);

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPassword.Command model) => await Send(model);

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPassword.Command model) => await Send(model);
    }

}

