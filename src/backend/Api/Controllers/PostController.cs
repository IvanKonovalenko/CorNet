using BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostControl _postControl;
        public PostController(IPostControl postControl)
        {
            this._postControl = postControl;
        }
    }
}
