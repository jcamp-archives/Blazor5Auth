namespace Blazor5Auth.Shared
{
    public class SetupMfaModel
    {
        public string SharedKey { get; set; }
        public string AuthenticatorUri { get; set; }
        public string QrCodeBase64 { get; set; }
    }

    public class VerifyMfaModel
    {
        public string Verification { get; set; }
    }

    public class VerifyMfaResult
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
        public string Status { get; set; }
    }
}
