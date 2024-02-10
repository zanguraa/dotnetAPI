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

        [HttpGet("Posts")]
        public IEnumerable<Post> Posts()
        {
            string sql = @" SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] FROM TutorialAppSchema.Posts";

            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("PostSingle/{postId}")]
        public IEnumerable<Post> PostSingle(int postId)
        {
            string sql = @" SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] FROM TutorialAppSchema.Posts
                     WHERE PostId = " + postId.ToString();

            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("PostsByUser/{userId}")]
        public IEnumerable<Post> PostsByUser(int userId)
        {
            string sql = @" SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] FROM TutorialAppSchema.Posts
                     WHERE userId = " + userId.ToString();

            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyposts()
        {
            string sql = @" SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] FROM TutorialAppSchema.Posts
                     WHERE userId = " + this.User.FindFirst("userid")?.Value;

            return _dapper.LoadData<Post>(sql);
        }

        [HttpPost("AddPost")]
        public IActionResult AddPost(PostToAddDto postAdd)
        {
            string sql = @" INSERT INTO TutorialAppSchema.Posts(
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] ) VALUES ("
                    + this.User.FindFirst("userid")?.Value
                    + ",'" + postAdd.PostTitle
                    + "','" + postAdd.PostContent
                    + "',GETDATE(),GETDATE())";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to Create post");
        }

        [HttpPut("EditPost")]
        public IActionResult EditPost(PostToEditDto postEdit)
        {
            string sql = @"UPDATE TutorialAppSchema.Posts
                   SET PostContent = '" + postEdit.PostContent + @"',
                       PostTitle = '" + postEdit.PostTitle + @"',
                       PostUpdated = GETDATE()
                   WHERE PostId = " + postEdit.PostId.ToString() + @"
                   AND UserId = " + this.User.FindFirst("userid")?.Value;

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to Edit post");
        }

        [HttpDelete("DeletePost/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = @" DELETE FROM TutorialAppSchema.Posts
                     WHERE PostId = " + postId.ToString() +
                        " AND UserId = " + this.User.FindFirst("userid")?.Value;

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }
            throw new Exception("Failed to Delete post");
        }


    }
}