using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazor5Auth.Shared;
using Features.Account;

namespace Blazor5Auth.Client.Services
{
    public interface ISecondaryAuthService
    {
        Task<SecondaryLoginPassword.Result> Login(SecondaryLoginPassword.Command loginModel);
        Task<LoginMultiFactor.QueryResult> CheckMfa();
        Task<LoginMultiFactor.Result> LoginMfa(LoginMultiFactor.Command loginModel);
        Task<LoginRecoveryCode.Result> LoginRecoveryCode(LoginRecoveryCode.Command loginModel);
        Task Logout();
        Task<SecondaryRegister.Result> Register(SecondaryRegister.Command registerModel);
        Task UpdateToken(string token);
    }
}
