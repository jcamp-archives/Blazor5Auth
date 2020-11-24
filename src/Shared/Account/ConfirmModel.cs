using System.ComponentModel.DataAnnotations;

namespace Blazor5Auth.Shared
{
    public class ConfirmModel
    {
        public string UserId { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
    }
}
