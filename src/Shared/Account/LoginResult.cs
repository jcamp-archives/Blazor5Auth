﻿namespace Blazor5Auth.Shared
{
    public class LoginResult
    {
        public bool Successful { get; set; }
        public bool RequiresTwoFactor { get; set; }
        public bool IsLockedOut { get; set; }
        public string Error { get; set; }
        public string Token { get; set; }
    }
}