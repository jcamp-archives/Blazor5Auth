using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Blazor5Auth.Server.Extensions;
using Blazor5Auth.Server.Models;
using Microsoft.AspNetCore.Identity;

namespace Features.Account.Manage
{
    //this allows us to avoid Create. in front of results, commands, etc
    public class MfaForgetBrowser_ : MfaForgetBrowser
    {
        public class CommandHandler : ICommandHandler
        {
            private readonly SignInManager<ApplicationUser> _signInManager;

            public CommandHandler(SignInManager<ApplicationUser> signInManager)
            {
                _signInManager = signInManager;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                await _signInManager.ForgetTwoFactorClientAsync();

                return new Result().Succeeded(
                    "The current browser has been forgotten. When you login again from this browser you will be prompted for your Mfa code.");
            }
        }
    }
}
