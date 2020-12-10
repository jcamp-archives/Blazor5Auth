using Features.Base;
using FluentValidation;
using MediatR;

namespace Features.Account.Manage
{
    public class PersonalData
    {
        public class Query : IRequest<QueryResult> { }

        public class QueryResult : BaseResult
        {
            public string JsonData { get; set; }
        }

        public class Command : IRequest<Result>
        {
            public string Password { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.Password).NotEmpty().MinimumLength(8);
            }
        }

        public class Result : BaseResult { }

        //this is here for easy navigation with goto implementation
        public interface ICommandHandler : IRequestHandler<Command, Result> { }
        public interface IQueryHandler : IRequestHandler<Query, QueryResult> { }

    }
}
