using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Features.Base;
using FluentValidation;
using MediatR;

namespace Features.Account
{
    public class LoginMultiFactor
    {
        public class Query : IRequest<QueryResult> { }

        public class QueryResult : BaseResult { }

        public class Command : IRequest<Result>
        {
            public string TwoFactorCode { get; set; }
            public bool RememberMachine { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.TwoFactorCode).NotEmpty().Length(6,7);
            }
        }

        public class Result : BaseResult
        {
            public string Token { get; set; }
        }

        //this is here for easy navigation with goto implementation
        public interface ICommandHandler : IRequestHandler<Command, Result> { }
        public interface IQueryHandler : IRequestHandler<Query, QueryResult> { }

    }
}
