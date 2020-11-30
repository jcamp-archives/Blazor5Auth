using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Features.Base;
using FluentValidation;
using MediatR;

namespace Features.Account.Manage
{
    public class ChangeEmail
    {
        public class Command : IRequest<Result>
        {
            public string NewEmail { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.NewEmail).NotEmpty().EmailAddress();
            }
        }

        public class Result : BaseResult { }

        //this is here for easy navigation with goto implementation
        public interface ICommandHandler : IRequestHandler<Command, Result> { }

    }
}
