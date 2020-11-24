using System.Collections.Generic;

namespace Blazor5Auth.Shared
{
    public class RecoveryCodesResult
    {
        public bool Successful { get; set; }
        public string Status { get; set; }
        public string[] RecoveryCodes { get; set; }
    }
}
