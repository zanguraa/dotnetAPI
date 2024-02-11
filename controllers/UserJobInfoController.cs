

using DotnetApi.Data;
using DotnetApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class UserJobInfoController : ControllerBase
    {
        DataContextDapper _dapper;
        public UserJobInfoController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("GetUserJobInfo")]
        public IEnumerable<UserJobInfo> GetUserJobInfo()
        {
            string sql = @"
            SELECT [UserId],
                [JobTitle],
                [Department] 
            FROM TutorialAppSchema.UserJobInfo";

            IEnumerable<UserJobInfo> userJobInfo = _dapper.LoadData<UserJobInfo>(sql);
            return userJobInfo;
        }

        [HttpGet("GetSingleUserJobInfo/{userId}")]
        public UserJobInfo GetSingleUserJobInfo(int userId)
        {
            string sql = @"
            SELECT [UserId],
                [JobTitle],
                [Department]
                FROM TutorialAppSchema.UserJobInfo
                WHERE UserId = " + userId.ToString();

            UserJobInfo userJobInfo = _dapper.LoadDataSingle<UserJobInfo>(sql);
            return userJobInfo;
        }

        [HttpPut("EditUserJobInfo")]
        public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
        {
            string sql = @"
            UPDATE TutorialAppSchema.UserJobInfo
            SET JobTitle = '" + userJobInfo.JobTitle + @"',
                Department = '" + userJobInfo.Department + @"'
            WHERE UserId = " + userJobInfo.UserId.ToString();

            IEnumerable<UserJobInfo> userJobInfoList = _dapper.LoadData<UserJobInfo>(sql);

            return Ok();
        }

        [HttpPost("AddUserJobInfo")]
        public IActionResult AddUserJobInfo(UserJobInfo userJobInfo)
        {
            if (_dapper.UserExists(userJobInfo.UserId))
    {
        // UserId is not unique, return a conflict response
        return Conflict("User with the same UserId already exists.");
    }
    
            string sql = @"
            INSERT INTO TutorialAppSchema.UserJobInfo
            (UserId, JobTitle, Department)
            VALUES
            (" + userJobInfo.UserId.ToString() + @", '" + userJobInfo.JobTitle + @"', '" + userJobInfo.Department + @"')";
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
}


    