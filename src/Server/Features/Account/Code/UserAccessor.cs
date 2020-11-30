using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Features.Account
{
    public interface IUserAccessor
    {
        ClaimsPrincipal User { get; }
    }

    public class UserAccessor : IUserAccessor
    { 
        private IHttpContextAccessor _accessor;
        public UserAccessor(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
        public ClaimsPrincipal User => _accessor. HttpContext.User;
    }
    
}
