using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazor5Auth.Shared;
using Features.Account;

namespace Blazor5Auth.Client.Services
{
    public interface IAuthService
    {
        Task<LoginPassword.Result> Login(LoginPassword.Command loginModel);
        Task<LoginMultiFactor.QueryResult> CheckMfa();
        Task<LoginMultiFactor.Result> LoginMfa(LoginMultiFactor.Command loginModel);
        Task<LoginRecoveryCode.Result> LoginRecoveryCode(LoginRecoveryCode.Command loginModel);
        Task Logout();
        Task<Register.Result> Register(Register.Command registerModel);
    }
}
