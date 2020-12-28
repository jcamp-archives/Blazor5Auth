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
    public class SecondaryRegister_ : SecondaryRegister
    {
        public class CommandHandler : ICommandHandler
        {
            private readonly IJwtHelper _jwtHelper;
            private readonly SignInManager<SecondaryUser> _signInManager;
            private readonly IHttpContextAccessor _contextAccessor;
            private readonly IEmailService _emailService;

            public CommandHandler(IJwtHelper jwtHelper,
                SignInManager<SecondaryUser> signInManager, IHttpContextAccessor contextAccessor, IEmailService emailService)
            {
                _jwtHelper = jwtHelper;
                _signInManager = signInManager;
                _contextAccessor = contextAccessor;
                _emailService = emailService;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var newUser = new SecondaryUser { UserName = request.Email, Email = request.Email };

                var result = await _signInManager.UserManager.CreateAsync(newUser, request.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(x => x.Description);

                    return new Result().WithErrors(errors);
                }

                // Send confirmation email
                var code = await _signInManager.UserManager.GenerateEmailConfirmationTokenAsync(newUser);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var httpRequest = _contextAccessor.HttpContext.Request;
                var domain = $"{httpRequest.Scheme}://{httpRequest.Host}";

                var callbackUrl = $"{domain}/Account/ConfirmEmail?userId={Uri.EscapeDataString(newUser.Id)}&code={code}";

                await _emailService.SendAsync(newUser.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

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
