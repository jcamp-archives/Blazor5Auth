using System.Threading;
using System.Threading.Tasks;
using Blazor5Auth.Server.Extensions;
using Blazor5Auth.Server.Models;
using Microsoft.AspNetCore.Identity;

namespace Features.Account
{
    //this allows us to avoid Create. in front of results, commands, etc
    public class LoginRecoveryCode_ : LoginRecoveryCode
    {
        public class CommandHandler : ICommandHandler
        {
            private readonly IJwtHelper _jwtHelper;
            private readonly SignInManager<ApplicationUser> _signInManager;

            public CommandHandler(IJwtHelper jwtHelper,
                SignInManager<ApplicationUser> signInManager)
            {
                _jwtHelper = jwtHelper;
                _signInManager = signInManager;
            }
            
            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null) return new Result().Failed("Unable to load two-factor authentication user.");

                var authenticatorCode = request.RecoveryCode.Replace(" ", string.Empty).Replace("-", string.Empty);

                var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(authenticatorCode);

                if (!result.Succeeded) return new Result().Failed("Invalid recovery code.");

                var roles = await _signInManager.UserManager.GetRolesAsync(user);

                var token = _jwtHelper.GenerateJwt(user, roles);
                
                return new Result {IsSuccessful = true, Token = token};
            }
        }
    }
}
