using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Blazor5Auth.Server.Extensions;
using Blazor5Auth.Server.Models;
using Blazor5Auth.Shared;
using Microsoft.AspNetCore.Identity;

namespace Features.Account.Manage
{
    //this allows us to avoid Create. in front of results, commands, etc
    public class MfaDisable_ : MfaDisable
    {
        public class CommandHandler : ICommandHandler
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly ClaimsPrincipal _user;

            public CommandHandler(UserManager<ApplicationUser> userManager, IUserAccessor user)
            {
                _userManager = userManager;
                _user = user.User;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_user);

                var disableMfaResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
                if (!disableMfaResult.Succeeded)
                {
                    return new Result().Failed(
                        $"Unexpected error occurred disabling 2FA for user with ID '{_userManager.GetUserId(_user)}'.");
                }

                return new Result().Succeeded(
                    "Mfa has been disabled. You can reenable 2fa when you setup an authenticator app");
            }
        }
    }
}
