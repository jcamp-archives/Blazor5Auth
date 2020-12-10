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
    public class MfaInfo_ : MfaInfo
    {
        public class QueryHandler : IQueryHandler
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly SignInManager<ApplicationUser> _signInManager;
            private readonly ClaimsPrincipal _user;

            public QueryHandler(UserManager<ApplicationUser> userManager, IUserAccessor user, SignInManager<ApplicationUser> signInManager)
            {
                _userManager = userManager;
                _signInManager = signInManager;
                _user = user.User;
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_user);

                var result = new Result
                {
                    HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                    IsMfaEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                    IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
                    RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
                    IsSuccessful = true
                };
                return result;
            }
        }

    }
}
