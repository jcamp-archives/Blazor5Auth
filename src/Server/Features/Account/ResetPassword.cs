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

namespace Features.Account.Manage
{
    //this allows us to avoid Create. in front of results, commands, etc
    public class ResetPassword_ : ResetPassword
    {
        public class CommandHandler : ICommandHandler
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly SignInManager<ApplicationUser> _signInManager;
            private readonly ClaimsPrincipal _user;
            private readonly IHttpContextAccessor _contextAccessor;
            private readonly IEmailService _emailService;

            public CommandHandler(UserManager<ApplicationUser> userManager, IUserAccessor user, IEmailService emailService, IHttpContextAccessor contextAccessor, SignInManager<ApplicationUser> signInManager)
            {
                _userManager = userManager;
                _user = user.User;
                _emailService = emailService;
                _contextAccessor = contextAccessor;
                _signInManager = signInManager;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null) return new Result().Succeeded();
                
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));

                var result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);

                if (!result.Succeeded)
                {
                    return new Result().Failed(result.Errors.First().Description);
                }

                return new Result().Succeeded();
            }
        }
    }
}
