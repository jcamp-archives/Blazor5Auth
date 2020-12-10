using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Blazor5Auth.Server.Extensions;
using Blazor5Auth.Server.Models;
using Blazor5Auth.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace Features.Account.Manage
{
    //this allows us to avoid Create. in front of results, commands, etc
    public class PersonalData_ : PersonalData
    {
        public class QueryHandler : IQueryHandler
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly ClaimsPrincipal _user;
            private readonly UrlEncoder _urlEncoder;

            private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

            public QueryHandler(UserManager<ApplicationUser> userManager, IUserAccessor user, UrlEncoder urlEncoder)
            {
                _userManager = userManager;
                _urlEncoder = urlEncoder;
                _user = user.User;
            }

            public async Task<QueryResult> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_user);
                
                // Only include personal data for download
                var personalData = new Dictionary<string, string>();
                var personalDataProps = typeof(ApplicationUser).GetProperties().Where(
                    prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
                foreach (var p in personalDataProps)
                {
                    personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
                }

                var logins = await _userManager.GetLoginsAsync(user);
                foreach (var l in logins)
                {
                    personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
                }

                var result = new QueryResult().Succeeded();
                result.JsonData = JsonSerializer.Serialize(personalData);

                return result;

                // Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
                // return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");

            }

        }

        public class CommandHandler : ICommandHandler
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly ClaimsPrincipal _user;
            private readonly SignInManager<ApplicationUser> _signInManager;
            
            public CommandHandler(UserManager<ApplicationUser> userManager, IUserAccessor user, SignInManager<ApplicationUser> signInManager)
            {
                _userManager = userManager;
                _signInManager = signInManager;
                _user = user.User;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_user);

                if (!await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    return new Result().Failed("Incorrect password.");
                }

                var result = await _userManager.DeleteAsync(user);
                var userId = await _userManager.GetUserIdAsync(user);
                if (!result.Succeeded)
                {
                    return new Result().Failed($"Unexpected error occurred deleting user with ID '{userId}'.");
                }

                await _signInManager.SignOutAsync();

                return new Result().Succeeded();
            }
        }
    }
}
