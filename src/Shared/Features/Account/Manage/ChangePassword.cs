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
    public class ChangePassword
    {
        public class Command : IRequest<Result>
        {
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
            public string ConfirmPassword { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(p => p.OldPassword).NotEmpty().MinimumLength(8);
                RuleFor(p => p.NewPassword).NotEmpty().MinimumLength(8);
                RuleFor(p => p.ConfirmPassword).Matches(v => v.NewPassword);
            }
        }

        public class Result : BaseResult { }

        //this is here for easy navigation with goto implementation
        public interface ICommandHandler : IRequestHandler<Command, Result> { }

    }
}
