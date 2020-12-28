using System.Threading;
using System.Threading.Tasks;
using Blazor5Auth.Server.Extensions;
using Blazor5Auth.Server.Models;
using Microsoft.AspNetCore.Identity;

namespace Features.Account
{
    //this allows us to avoid Create. in front of results, commands, etc
    public class SecondaryLoginPassword_ : SecondaryLoginPassword
    {
        public class CommandHandler : ICommandHandler
        {
            private readonly IJwtHelper _jwtHelper;
            private readonly SignInManager<SecondaryUser> _signInManager;

            public CommandHandler(IJwtHelper jwtHelper,
                SignInManager<SecondaryUser> signInManager)
            {
                _jwtHelper = jwtHelper;
                _signInManager = signInManager;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, false);

                if (result.RequiresTwoFactor) return new Result {IsSuccessful = false, RequiresTwoFactor = true};
                if (result.IsLockedOut) return new Result {IsSuccessful = false, IsLockedOut = true};

                if (result.IsNotAllowed) {
                    var user2 = await _signInManager.UserManager.FindByEmailAsync(request.Email);
                    if (!(await _signInManager.UserManager.IsEmailConfirmedAsync(user2)))
                    {
                        return new Result {IsSuccessful = false, RequiresEmailConfirmation = true};
                    }
                }

                if (!result.Succeeded) return new Result().Failed("Username and password are invalid.");

                var user = await _signInManager.UserManager.FindByEmailAsync(request.Email);
                var roles = await _signInManager.UserManager.GetRolesAsync(user);

                var token = _jwtHelper.GenerateJwt(user, roles);

                return new Result {IsSuccessful = true, Token = token};
            }
        }
    }
}
