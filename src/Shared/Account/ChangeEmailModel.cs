using System.ComponentModel.DataAnnotations;

namespace Blazor5Auth.Shared
{
    public class ChangeEmailModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "New email")]
        public string NewEmail { get; set; }
    }
}
