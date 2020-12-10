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
    public class ResetPassword
    {
        public class Command : IRequest<Result>
        {
            public string Code { get; set; }
            public string Email { get; set; }
            public string NewPassword { get; set; }
            public string ConfirmPassword { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.Code).NotEmpty();
                RuleFor(p => p.Email).NotEmpty().EmailAddress();
                RuleFor(p => p.NewPassword).NotEmpty().MinimumLength(8);
                RuleFor(p => p.ConfirmPassword).Matches(v => v.NewPassword);
            }
        }

        public class Result : BaseResult { }

        //this is here for easy navigation with goto implementation
        public interface ICommandHandler : IRequestHandler<Command, Result> { }

    }
}
