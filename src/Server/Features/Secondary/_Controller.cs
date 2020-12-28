using System.Threading.Tasks;
using Features.Base;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Features.Account
{
    public class SecondaryController : MediatrControllerBase
    {
        public SecondaryController(ISender sender) : base(sender) { }

        [HttpPost]
        public async Task<IActionResult> Login(SecondaryLoginPassword.Command model) => await Send(model);

        [HttpPost]
        public async Task<IActionResult> Register(SecondaryRegister.Command model) => await Send(model);
    }

}

