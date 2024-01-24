using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers;
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    DataContextDapper _dapper;
    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE() AS CurrentDateTime");
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        string sql = @"SELECT [UserId],
    [FirstName],
    [LastName],
    [Email],
    [Gender],
    [Active] 
FROM TutorialAppSchema.Users";

        IEnumerable<User> users = _dapper.LoadData<User>(sql);
        return users;
    }

    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        string sql = @"
        SELECT [UserId],
            [FirstName],
            [LastName],
            [Email],
            [Gender],
            [Active] 
        FROM TutorialAppSchema.Users
        WHERE UserId = " + userId.ToString();

        User user = _dapper.LoadDataSingle<User>(sql);
        return user;
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"
        UPDATE TutorialAppSchema.Users
             SET  [FirstName] = '" + user.FirstName +
             "', [LastName] = '" + user.LastName +
             "',[Email] = '" + user.Email +
             "',[Gender] ='" + user.Gender +
                "',[Active] ='" + user.Active +
                "' WHERE UserId = " + user.UserId.ToString();
        if (_dapper.ExexuteSql(sql))
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpPost("AddUser")]

    public IActionResult AddUser(User user)
    {
        string sql = @"
        INSERT INTO TutorialAppSchema.Users(
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active] 
            ) VALUES ( '" + user.FirstName +
                "',  '" + user.LastName +
                "','" + user.Email +
                "','" + user.Gender +
                "','" + user.Active +
                "')";

        System.Console.WriteLine(sql);
        if (_dapper.ExexuteSql(sql))
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }
}



