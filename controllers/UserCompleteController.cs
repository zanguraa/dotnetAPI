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


    [HttpGet("GetUsers{UserId}/{isActive}")]
    public IEnumerable<UserComplete> GetUsers(int UserId, bool isActive)
    {
        string sql = @"EXEC TutorialAppSchema.spUsers_Get";
        string parameters = "";

        if (UserId != 0)
        {
            parameters += ", @Userid= " + UserId.ToString();
        }
        if (isActive)
        {
            parameters += ", @Active= " + isActive.ToString();
        }

        sql += parameters.Substring(1);

        IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
        return users;
    }



    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(UserComplete user)
    {
        string sql = @"EXEC TutorialAppSchema.spUser_Upsert
             @FirstName = '" + user.FirstName +
             "', @LastName = '" + user.LastName +
             "', @Email = '" + user.Email +
             "', @Gender ='" + user.Gender +
             "', @Active ='" + user.Active +
             "', @JobTitle ='" + user.JobTitle +
             "', @Department ='" + user.Department +
             "', @Salary ='" + user.Salary +
             "', @UserId ='" + user.UserId;

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Failed to Update User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"TutorialAppSchema.spUser_Delete
       @UserId = " + userId.ToString();

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



