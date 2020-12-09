using Features.Base;
using FluentValidation;
using MediatR;

namespace Features.Account.Manage
{
    public class UserProfile
    {
        public class Query : IRequest<Command> { }

        public class Command : IRequest<Result>
        {
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public bool IsEmailConfirmed { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.Email).NotEmpty().EmailAddress();
            }
        }

        public class Result : BaseResult { }

        //this is here for easy navigation with goto implementation
        public interface ICommandHandler : IRequestHandler<Command, Result> { }
        public interface IQueryHandler : IRequestHandler<Query, Command> { }

    }
}
