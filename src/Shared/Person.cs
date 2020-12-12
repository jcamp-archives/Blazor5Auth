using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Blazor5Auth.Shared
{
    public class Person
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int? Age { get; set; }
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}
