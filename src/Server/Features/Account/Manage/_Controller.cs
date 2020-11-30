using System.Threading.Tasks;
using Features.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Features.Account.Manage
{
    [Authorize]
    [Route("api/account/[controller]/[action]")]
    public class ManageController : MediatrControllerBase
    {
        public ManageController(ISender sender) : base(sender) { }
        
        [HttpGet]
        public async Task<IActionResult> UserProfile() => await Send(new UserProfile.Query());

        [HttpPost]
        public async Task<IActionResult> UserProfile(UserProfile.Command model) => await Send(model);
    }
}
