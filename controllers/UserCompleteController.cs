using DotnetApi.Data;
using DotnetApi.Dtos;
using DotnetApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers;
[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{

    DataContextDapper _dapper;
    public UserCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }


    [HttpGet("GetUsers{UserId}")]
    public IEnumerable<UserComplete> GetUsers(int UserId)
    {
        string sql = @"EXEC TutorialAppSchema.spUsers_Get";

        if (UserId != 0)
        {
            sql += " @Userid= " + UserId.ToString();
        }

        IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
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
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpPost("AddUser")]

    public IActionResult AddUser(UserToAddDto user)
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
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"
        DELETE FROM TutorialAppSchema.Users
        WHERE UserId = " + userId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }

}



