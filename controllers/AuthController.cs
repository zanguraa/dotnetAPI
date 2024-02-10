using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetApi.Data;
using DotnetApi.Dtos;
using DotnetApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DotnetApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        private readonly AuthHelper _authHelper;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
            _authHelper = new AuthHelper(config);
        }

        [AllowAnonymous]
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
            if (UserExists.Count() > 0)
            {
                return BadRequest("User already exists");
            }
            byte[] passwordSalt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(passwordSalt);
            }


            byte[] passwordHash = _authHelper.GetPasswordHash(UserForRegistration.Password, passwordSalt);

            string SqlAddAuth = @"INSERT INTO TutorialAppSchema.Auth (
            [Email],
            [PasswordHash],
            [PasswordSalt]
            ) VALUES ( '" + UserForRegistration.Email +
                     "', @passwordHash, @passwordSalt)";

            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            SqlParameter passwordSaltParameter = new SqlParameter("@passwordSalt", SqlDbType.VarBinary);
            passwordSaltParameter.Value = passwordSalt;
            SqlParameter passwordHashParameter = new SqlParameter("@passwordHash", SqlDbType.VarBinary);
            passwordHashParameter.Value = passwordHash;

            sqlParameters.Add(passwordSaltParameter);
            sqlParameters.Add(passwordHashParameter);

            if (_dapper.ExecuteSqlWithParameters(SqlAddAuth, sqlParameters) == 0)
            {
                return BadRequest("Failed to add user");
            }
            string sqlAddUser = @"
        INSERT INTO TutorialAppSchema.Users(
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active] 
            ) VALUES ( '" + UserForRegistration.FirstName +
               "',  '" + UserForRegistration.LastName +
               "','" + UserForRegistration.Email +
               "','" + UserForRegistration.Gender +
               "', 1)";

            if (_dapper.ExexuteSql(sqlAddUser))
            {
                return Ok();
            }
            else
            {
                throw new Exception("Failed to add user");
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = @"SELECT 
                [PasswordHash],
                [PasswordSalt] FROM TutorialAppSchema.Auth WHERE Email = '" +
                userForLogin.Email + "'";

            UserForLoginConfirmationDto userForConfirmation = _dapper
                .LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);


            for (int index = 0; index < passwordHash.Length; index++)
            {
                if (passwordHash[index] != userForConfirmation.PasswordHash[index])
                {
                    return StatusCode(401, "Incorrect password!");
                }
            }

            string userIdSql = @"
                SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '" +
                userForLogin.Email + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string> {
                {"token", _authHelper.CreateToken(userId)}
            });
        }

        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            string userIdSql = @"
                SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = '" +
                User.FindFirst("userId")?.Value + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return _authHelper.CreateToken(userId);
        }




    }
}
