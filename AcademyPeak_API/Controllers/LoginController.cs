using AcademyPeak_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AcademyPeak_API.Controllers
{
    [Route("login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly TokenService tokenService;

        public LoginController(TokenService tokenService)
        {
            this.tokenService = tokenService;
        }


        [HttpGet("{username}/{password}")]
        public async Task<string> Get(string username, string password)
        {
            string token = await tokenService.ApiLogin(username, password);
            return token;
        }
    }
}
