using System;
using System.Linq;
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
    public class ConfirmEmail_ : ConfirmEmail
    {
        public class CommandHandler : ICommandHandler
        {
            private readonly IJwtHelper _jwtHelper;
            private readonly SignInManager<ApplicationUser> _signInManager;
            private readonly IHttpContextAccessor _contextAccessor;
            private readonly IEmailService _emailService;

            public CommandHandler(IJwtHelper jwtHelper,
                SignInManager<ApplicationUser> signInManager, IHttpContextAccessor contextAccessor, IEmailService emailService)
            {
                _jwtHelper = jwtHelper;
                _signInManager = signInManager;
                _contextAccessor = contextAccessor;
                _emailService = emailService;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _signInManager.UserManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return new Result().Failed($"Unable to load user with ID '{request.UserId}'.");
                }

                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
                var result = await _signInManager.UserManager.ConfirmEmailAsync(user, code);

                if (!result.Succeeded) {
                    return new Result().Failed("Error confirming your email.");
                }

                return new Result().Succeeded("Thank you for confirming your email. You may now login.");
            }
        }
    }
}
