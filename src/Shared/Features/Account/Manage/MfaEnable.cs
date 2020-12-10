using Features.Base;
using FluentValidation;
using MediatR;

namespace Features.Account.Manage
{
    public class MfaEnable
    {
        public class Query : IRequest<QueryResult> { }

        public class QueryResult : BaseResult
        {
            public string SharedKey { get; set; }
            public string AuthenticatorUri { get; set; }
            public string QrCodeBase64 { get; set; }
        }

        public class Command : IRequest<Result>
        {
            public string VerificationCode { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.VerificationCode).NotEmpty().Length(6, 8);
            }
        }

        public class Result : BaseResult { }

        //this is here for easy navigation with goto implementation
        public interface ICommandHandler : IRequestHandler<Command, Result> { }
        public interface IQueryHandler : IRequestHandler<Query, QueryResult> { }

    }
}
