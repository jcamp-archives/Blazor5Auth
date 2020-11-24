using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazor5Auth.Shared;

namespace Blazor5Auth.Client.Services
{
    public interface IAuthService
    {
        Task<LoginResult> CheckMfa();
        Task<LoginResult> Login(LoginModel loginModel);
        Task<LoginResult> Login2fa(Login2faModel loginModel);
        Task<LoginResult> LoginRecoveryCode(Login2faModel loginModel);
        Task Logout();
        Task<RegisterResult> Register(RegisterModel registerModel);
    }
}
