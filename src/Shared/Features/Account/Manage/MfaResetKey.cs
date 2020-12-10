using Features.Base;
using MediatR;

namespace Features.Account.Manage
{
    public class MfaResetKey
    {
        public class Command : IRequest<Result> { }

        public class Result : BaseResult { }

        //this is here for easy navigation with goto implementation
        public interface ICommandHandler : IRequestHandler<Command, Result> { }
    }
}
