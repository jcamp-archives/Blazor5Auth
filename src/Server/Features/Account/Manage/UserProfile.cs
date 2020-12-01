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
    public class UserProfile_ : UserProfile
    {
        public class QueryHandler : IQueryHandler
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly ClaimsPrincipal _user;

            public QueryHandler(UserManager<ApplicationUser> userManager, IUserAccessor user)
            {
                _userManager = userManager;
                _user = user.User;
            }

            public async Task<Command> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(_user);
                var model = new Command {Email = user.Email, PhoneNumber = user.PhoneNumber};
                return model;
            }
        }

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
                var statusMessage = "";
                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

                if (request.PhoneNumber != phoneNumber)
                {
                    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
                    statusMessage = setPhoneResult.Succeeded ? "Your profile has been updated" : "Unexpected error when trying to set phone number.";
                }

                return new Result().Succeeded(statusMessage);
            }
        }
    }
}
