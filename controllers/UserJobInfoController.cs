

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
    }
}