using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazor5Auth.Server.Extensions;
using Blazor5Auth.Server.Models;
using Blazor5Auth.Shared;
using Microsoft.AspNetCore.Identity;

namespace Features.Account
{
    //this allows us to avoid Create. in front of results, commands, etc
    public class Register_ : Register
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
                var newUser = new ApplicationUser { UserName = request.Email, Email = request.Email };

                var result = await _signInManager.UserManager.CreateAsync(newUser, request.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(x => x.Description);

                    return new Result().WithErrors(errors);
                }

                // Add all new users to the User role
                await _signInManager.UserManager.AddToRoleAsync(newUser, "User");

                // Add new users whose email starts with 'admin' to the Admin role
                if (newUser.Email.StartsWith("admin"))
                {
                    await _signInManager.UserManager.AddToRoleAsync(newUser, "Admin");
                }

                return new Result().Succeeded();
            }
        }
    }
}
