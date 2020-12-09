using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Blazor5Auth.Server.Extensions;
using Blazor5Auth.Server.Models;
using Blazor5Auth.Server.Services;
using Blazor5Auth.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Features.Account
{
    //this allows us to avoid Create. in front of results, commands, etc
    public class ConfirmEmailChange_ : ConfirmEmailChange
    {
        public class CommandHandler : ICommandHandler
        {
            private readonly IJwtHelper _jwtHelper;
            private readonly SignInManager<ApplicationUser> _signInManager;
            private readonly IHttpContextAccessor _contextAccessor;
            private readonly IEmailService _emailService;
            private readonly ClaimsPrincipal _user;

            public CommandHandler(IJwtHelper jwtHelper,
                SignInManager<ApplicationUser> signInManager, IHttpContextAccessor contextAccessor, IEmailService emailService, IUserAccessor user)
            {
                _jwtHelper = jwtHelper;
                _signInManager = signInManager;
                _contextAccessor = contextAccessor;
                _emailService = emailService;
                _user = user.User;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _signInManager.UserManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return new Result().Failed($"Unable to load user with ID '{request.UserId}'.");
                }

                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
                var result = await _signInManager.UserManager.ChangeEmailAsync(user, request.Email, code);

                if (!result.Succeeded) {
                    return new Result().Failed("Error confirming your email.");
                }

                // In our UI email and user name are one and the same, so when we update the email
                // we need to update the user name.
                var setUserNameResult = await _signInManager.UserManager.SetUserNameAsync(user, request.Email);
                await _signInManager.RefreshSignInAsync(user);

                if (!setUserNameResult.Succeeded) {
                    return new Result().Failed("Error changing user name.");
                }

                if (_user.Identity.IsAuthenticated) {
                    var loggedInUser = await _signInManager.UserManager.GetUserAsync(_user);
                    if (loggedInUser.Id == request.UserId) {
                        var roles = await _signInManager.UserManager.GetRolesAsync(user);
                        var token = _jwtHelper.GenerateJwt(user, roles);
                        return (new Result{ Token = token }).Succeeded("Thank you for confirming your email change.");
                    }
                }

                return new Result().Succeeded("Thank you for confirming your email change.");
            }
        }
    }
}
