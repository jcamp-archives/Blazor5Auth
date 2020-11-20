using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor5Auth.Shared
{
    public class UserModel
    {
        public string Email { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
