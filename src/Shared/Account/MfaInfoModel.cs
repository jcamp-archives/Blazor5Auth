using System;
namespace Blazor5Auth.Shared
{
    public class MfaInfoModel
    {
        public MfaInfoModel() { }

        public bool HasAuthenticator { get; set; }
        public int RecoveryCodesLeft { get; set; }
        public bool Is2faEnabled { get; set; }
        public bool IsMachineRemembered { get; set; }
        public string StatusMessage { get; set; }
    }
}
