namespace cnd.Controllers
{
    using cnd.Services;
    using Microsoft.AspNetCore.Mvc;
    using cnd.Models.Auth;

    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var token = await _auth.RegisterAsync(request.Username, request.Password);
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Gebruikersnaam bestaat al");

            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _auth.LoginAsync(request.Username, request.Password);
            if (string.IsNullOrWhiteSpace(token))
                return Unauthorized("Inloggegevens ongeldig");

            return Ok(new { token });
        }
    }
}
