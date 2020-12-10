using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Blazor5Auth.Shared;
using Blazored.LocalStorage;
using Features.Account;
using Microsoft.AspNetCore.Components.Authorization;

namespace Blazor5Auth.Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILocalStorageService _localStorage;

        public AuthService(HttpClient httpClient,
                           AuthenticationStateProvider authenticationStateProvider,
                           ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
            _localStorage = localStorage;
        }

        public async Task<Register.Result> Register(Register.Command registerModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/account/register", registerModel);

            return await response.Content.ReadFromJsonAsync<Register.Result>();
        }

        public async Task UpdateToken(string token)
        {
            await _localStorage.SetItemAsync("authToken", token);
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(token);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        }

        public async Task<LoginPassword.Result> Login(LoginPassword.Command loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/account/login", loginModel);
            var loginResult = await response.Content.ReadFromJsonAsync<LoginPassword.Result>();

            if (!response.IsSuccessStatusCode)
            {
                return loginResult;
            }

            await UpdateToken(loginResult.Token);

            return loginResult;
        }

        public async Task<LoginMultiFactor.Result> LoginMfa(LoginMultiFactor.Command loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/account/loginmfa", loginModel);
            var loginResult = await response.Content.ReadFromJsonAsync<LoginMultiFactor.Result>();

            if (!response.IsSuccessStatusCode)
            {
                return loginResult;
            }

            await UpdateToken(loginResult.Token);

            return loginResult;
        }

        public async Task<LoginRecoveryCode.Result> LoginRecoveryCode(LoginRecoveryCode.Command loginModel)
        {
            var response = await _httpClient.PostAsJsonAsync("api/login/loginrecovery", loginModel);
            var loginResult = await response.Content.ReadFromJsonAsync<LoginRecoveryCode.Result>();

            if (!response.IsSuccessStatusCode)
            {
                return loginResult;
            }

            await UpdateToken(loginResult.Token);

            return loginResult;
        }

        public async Task<LoginMultiFactor.QueryResult> CheckMfa()
        {
            var response = await _httpClient.PostAsJsonAsync("api/account/checkmfa", new { });
            var loginResult = await response.Content.ReadFromJsonAsync<LoginMultiFactor.QueryResult>();
            return loginResult;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
