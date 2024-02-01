using DotnetApi.Data;
using DotnetApi.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers
{

    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
        }

        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto UserForRegistration)
        {
            if (UserForRegistration.Password != UserForRegistration.PasswordConfirm)
            {
                return BadRequest("Passwords do not match");
            }
            string SqlCheckUserExists = @"SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" +
                 UserForRegistration.Email + "'";

            IEnumerable<string> UserExists = _dapper.LoadData<string>(SqlCheckUserExists);
            if(UserExists.Count() > 0)
            {
                return BadRequest("User already exists");
            }

            return Ok();
        }

        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto UserForLogin)
        {

            return Ok();
        }

    }
}
