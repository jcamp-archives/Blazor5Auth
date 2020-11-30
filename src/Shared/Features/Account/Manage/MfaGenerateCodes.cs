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
    public class MfaGenerateCodes
    {
        public class Command : IRequest<Result> { }

        public class Result : BaseResult
        {
            public string[] RecoveryCodes { get; set; }
        }

        //this is here for easy navigation with goto implementation
        public interface ICommandHandler : IRequestHandler<Command, Result> { }
    }
}
