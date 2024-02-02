using System.Data;
using System.Security.Cryptography;
using System.Text;
using DotnetApi.Data;
using DotnetApi.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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
            if (UserExists.Count() > 0)
            {
                return BadRequest("User already exists");
            }
            byte[] passwordSalt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(passwordSalt);
            }
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordSalt").Value +
            Convert.ToBase64String(passwordSalt);

            byte[] passwordHash = KeyDerivation.Pbkdf2(
                password: UserForRegistration.Password,
                salt: Encoding.UTF8.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            );

            string SqlAddAuth = @"INSERT INTO TutorialAppSchema.Auth (
                    [Email],
                    [PasswordHash],
                    [PasswprdSalt],
                    ) VALUES ( '" + UserForRegistration.Email +
                     "', @passwordHash, @passwordSalt)";

            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            SqlParameter passwordSaltParameter = new SqlParameter("@passwordSalt", SqlDbType.VarBinary);
            passwordSaltParameter.Value = passwordSalt;
            SqlParameter passwordHashParameter = new SqlParameter("@passwordHash", SqlDbType.VarBinary);
            passwordHashParameter.Value = passwordHash;

            sqlParameters.Add(passwordSaltParameter);
            sqlParameters.Add(passwordHashParameter);

            if(_dapper.ExecuteSqlWithParameters(SqlAddAuth, sqlParameters) == 0)
            {
                return BadRequest("Failed to add user");
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
