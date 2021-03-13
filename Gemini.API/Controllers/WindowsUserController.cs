using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Gemini.API.Controllers
{
    /// <summary>
    /// Windows user controller
    /// </summary>
    [Route("api/v{version:apiVersion}/loggedUser")]
    [ApiController]
    public class WindowsUserController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Meta information for the current logged in user
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        public WindowsUserController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the current logged user
        /// </summary>
        /// <returns>The User.Identity.Name in <see cref="IHttpContextAccessor.HttpContext"/></returns>
        /// <response code="200">Returns string with the user name </response>
        [Produces("text/plain")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetLoggedUser()
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
            return Ok(userId);
        }
    }
}
