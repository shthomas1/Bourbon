using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System;
using System.Data;
using API.Models;

namespace API.Controllers
{
    [Route("api/blog")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IDbConnection _db;

        public BlogController(IDbConnection db)
        {
            _db = db;

            if (_db.State == ConnectionState.Closed)
            {
                _db.Open();
            }
        }

        // GET: api/blog
        [HttpGet]
        public ActionResult<IEnumerable<UserBlog>> GetBlogs()
        {
            var blogs = new List<UserBlog>();

            using var command = _db.CreateCommand();
            command.CommandText = "SELECT BlogID, Title, Content, CreatedDate, UserID FROM BlogPosts WHERE Deleted = 0 ORDER BY CreatedDate DESC";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                blogs.Add(new UserBlog
                {
                    BlogID = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Content = reader.GetString(2),
                    CreatedDate = reader.GetDateTime(3),
                    UserID = reader.GetInt32(4)
                });
            }

            return Ok(blogs);
        }

        // GET: api/blog/{id}
        [HttpGet("{id}")]
        public ActionResult<UserBlog> GetBlog(int id)
        {
            using var command = _db.CreateCommand();
            command.CommandText = "SELECT BlogID, Title, Content, CreatedDate, UserID FROM BlogPosts WHERE BlogID = @id AND Deleted = 0";
            command.Parameters.Add(new MySqlParameter("@id", id));

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return Ok(new UserBlog
                {
                    BlogID = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Content = reader.GetString(2),
                    CreatedDate = reader.GetDateTime(3),
                    UserID = reader.GetInt32(4)
                });
            }

            return NotFound();
        }

        // POST: api/blog
        [HttpPost]
public IActionResult CreateBlog([FromBody] UserBlog blog)
{
    try
    {
        int? userId = HttpContext.Session.GetInt32("UserID");
        Console.WriteLine($"Session UserID: {userId}");

        if (userId == null || userId <= 0)
        {
            return Unauthorized("You must be logged in to create a blog post.");
        }

        if (blog == null || string.IsNullOrWhiteSpace(blog.Title) || string.IsNullOrWhiteSpace(blog.Content))
        {
            Console.WriteLine("Invalid Blog Data Received");
            return BadRequest("Title and content cannot be empty.");
        }

        using var command = _db.CreateCommand();
        command.CommandText = "INSERT INTO BlogPosts (UserID, Title, Content, CreatedDate) VALUES (@userID, @title, @content, @createdDate)";
        command.Parameters.Add(new MySqlParameter("@userID", userId.Value)); // ✅ Fix null issue
        command.Parameters.Add(new MySqlParameter("@title", blog.Title));
        command.Parameters.Add(new MySqlParameter("@content", blog.Content));
        command.Parameters.Add(new MySqlParameter("@createdDate", DateTime.UtcNow));

        int rowsAffected = command.ExecuteNonQuery();
        if (rowsAffected > 0)
        {
            return Ok(new { message = "Blog post created successfully." });
        }
        else
        {
            Console.WriteLine("Database Insert Failed");
            return StatusCode(500, "Failed to insert blog post.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in CreateBlog: {ex.Message}");
        return StatusCode(500, $"Internal Server Error: {ex.Message}");
    }
}






        // GET: api/blog/all
        [HttpGet("all")]
        public ActionResult<IEnumerable<object>> GetAllPosts()
        {
            var blogs = new List<object>();

            using var command = _db.CreateCommand();
            command.CommandText = @"
                SELECT 
                    b.BlogID, 
                    b.Title, 
                    b.Content, 
                    b.CreatedDate, 
                    u.FirstName AS AuthorFirstName, 
                    u.LastName AS AuthorLastName, 
                    b.UserID
                FROM BlogPosts b
                JOIN Users u ON b.UserID = u.UserID
                WHERE b.Deleted = 0
                ORDER BY b.CreatedDate DESC";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                blogs.Add(new
                {
                    BlogID = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Content = reader.GetString(2),
                    CreatedDate = reader.GetDateTime(3),
                    AuthorFirstName = reader.GetString(4),
                    AuthorLastName = reader.GetString(5),
                    UserID = reader.GetInt32(6)
                });
            }

            return Ok(blogs);
        }

        // DELETE: api/blog/{id}
        [HttpDelete("{id}")]
        public IActionResult DeletePost(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return Unauthorized("You must be logged in.");
            }

            using var checkCommand = _db.CreateCommand();
            checkCommand.CommandText = "SELECT UserID FROM BlogPosts WHERE BlogID = @id AND Deleted = 0";
            checkCommand.Parameters.Add(new MySqlParameter("@id", id));

            using var reader = checkCommand.ExecuteReader();
            if (!reader.Read()) return NotFound("Blog post not found.");

            int postOwnerId = reader.GetInt32(0);
            reader.Close();

            if (postOwnerId != userId)
            {
                return Unauthorized("You can only delete your own posts.");
            }

            using var deleteCommand = _db.CreateCommand();
            deleteCommand.CommandText = "UPDATE BlogPosts SET Deleted = 1 WHERE BlogID = @id";
            deleteCommand.Parameters.Add(new MySqlParameter("@id", id));

            var rowsAffected = deleteCommand.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                return Ok(new { message = "Blog post deleted successfully." });
            }

            return StatusCode(500, "Failed to delete blog post.");
        }



        // Helper function to get UserID from session or authentication
        private int GetUserIdFromSession()
        {
            if (User.Identity.IsAuthenticated)
            {
                return int.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value ?? "0");
            }
            return 0;
        }
    }
}
