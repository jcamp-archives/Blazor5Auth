using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Features.Base
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MediatrControllerBase : ControllerBase
    {
     
        protected readonly ISender _sender;
        
        public MediatrControllerBase(ISender sender)
        {
            _sender = sender;
        }

        protected async Task<IActionResult> Send<T>(IRequest<T> request)
        {
            var result = await _sender.Send(request);
            if (result is BaseResult baseResult)
            {
                if (!baseResult.IsSuccessful)
                {
                    return BadRequest(result);
                }
            }
            return Ok(result);
        }   
    }
}
