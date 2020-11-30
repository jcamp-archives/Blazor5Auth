using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Blazor5Auth.Server.Extensions;
using Blazor5Auth.Server.Models;
using Microsoft.AspNetCore.Identity;

namespace Features.Account.Manage
{
    //this allows us to avoid Create. in front of results, commands, etc
    public class MfaResetKey_ : MfaResetKey
    {
        public class CommandHandler : ICommandHandler
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly SignInManager<ApplicationUser> _signInManager;
            private readonly ClaimsPrincipal _user;

            public CommandHandler(UserManager<ApplicationUser> userManager, IUserAccessor user, SignInManager<ApplicationUser> signInManager)
            {
                _userManager = userManager;
                _signInManager = signInManager;
                _user = user.User;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_user);
                await _userManager.SetTwoFactorEnabledAsync(user, false);
                await _userManager.ResetAuthenticatorKeyAsync(user);
                await _signInManager.RefreshSignInAsync(user);

                return new Result().Success(
                    "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.");
            }
        }
    }
}
