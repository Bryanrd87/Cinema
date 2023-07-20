using Application.Contracts;
using Application.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]        
        [ProducesResponseType(StatusCodes.Status404NotFound)]          
        public ActionResult Login([FromBody] AuthRequest userLogin)
        {
            var user = _authService.Authenticate(userLogin);

            if (user is null)
            {
                return NotFound("user not found");
            }

            var token = _authService.GenerateToken(user);
            return Ok(token);
        }
    }
}
