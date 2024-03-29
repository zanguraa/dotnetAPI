using DotnetApi.Data;
using DotnetApi.Dtos;
using DotnetApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
        public IEnumerable<Post> Posts(int postId = 0, int userId = 0, string searchParam = "None")
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get";
            string parameters = "";

            if (postId != 0)
            {
                parameters += ", @postId=" + postId.ToString();
            }
            if (userId != 0)
            {
                parameters += ", @userId=" + userId.ToString();
            }
            if (searchParam.ToLower() != "none")
            {
                parameters += ", @searchValue='" + searchParam + "'";
            }

            if (parameters.Length > 0)
            {

                sql += parameters.Substring(1);
            }


            return _dapper.LoadData<Post>(sql);
        }


        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyposts()
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get = " +
             this.User.FindFirst("userid")?.Value;

            return _dapper.LoadData<Post>(sql);
        }



        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(Post postToUpsert)
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Upsert
                @UserId=" + this.User.FindFirst("userid")?.Value +
                ", @PostTitle ='" + postToUpsert.PostTitle +
                "', @PostContent ='" + postToUpsert.PostContent + "'";

            if (postToUpsert.PostId > 0)
            {

                sql += ", @PostId = " + postToUpsert.PostId;
            }

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to Upsert post");
        }

        [HttpDelete("DeletePost/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = @"EXEC TutorialAppSchema.spPost_Delete @PostId = " +
                postId.ToString() +
                ", @UserId = " + this.User.FindFirst("userid")?.Value;

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to Delete post");
        }


    }
}