using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using API.Models;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;

[Route("api/Users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IDbConnection _connection;
    private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

    public UsersController(IDbConnection connection)
    {
        _connection = connection;
        if (_connection.State == ConnectionState.Closed)
        {
            _connection.Open();
        }
    }

    // ✅ POST: api/Users/register
    // Registers a new user with a hashed password
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("All fields are required.");
        }

        // Hash the password before storing it
        string hashedPassword = _passwordHasher.HashPassword(new User(), request.Password);

        using var command = _connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO Users (Email, FirstName, LastName, PasswordHash, CreatedDate) 
            VALUES (@Email, @FirstName, @LastName, @PasswordHash, @CreatedDate)";
        command.Parameters.Add(new MySqlParameter("@Email", request.Email));
        command.Parameters.Add(new MySqlParameter("@FirstName", request.FirstName));
        command.Parameters.Add(new MySqlParameter("@LastName", request.LastName));
        command.Parameters.Add(new MySqlParameter("@PasswordHash", hashedPassword));
        command.Parameters.Add(new MySqlParameter("@CreatedDate", DateTime.UtcNow));

        command.ExecuteNonQuery();

        return Ok("User registered successfully.");
    }

    // ✅ POST: api/Users/login
    // Verifies user credentials and logs in
[HttpPost("login")]
public IActionResult Login([FromBody] LoginRequest loginRequest)
{
    if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
    {
        return BadRequest("Email and password are required.");
    }

    using var command = _connection.CreateCommand();
    command.CommandText = "SELECT UserID, Email, FirstName, LastName, PasswordHash FROM Users WHERE Email = @Email";
    command.Parameters.Add(new MySqlParameter("@Email", loginRequest.Email));

    using var reader = command.ExecuteReader();
    if (!reader.Read())
    {
        return Unauthorized("User not found.");
    }

    var user = new User
    {
        UserID = reader.GetInt32(0),
        Email = reader.GetString(1),
        FirstName = reader.GetString(2),
        LastName = reader.GetString(3),
        PasswordHash = reader.GetString(4)
    };

    reader.Close();

    var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password);
    if (verifyResult != PasswordVerificationResult.Success)
    {
        return Unauthorized("Invalid password.");
    }

    // ✅ Store UserID in session
    HttpContext.Session.SetInt32("UserID", user.UserID);
    
    // ✅ Debug: Log session value
    Console.WriteLine($"🔹 Session UserID set to: {user.UserID}");

    return Ok(new 
    { 
        userID = user.UserID,
        firstName = user.FirstName,
        message = $"Hello, {user.FirstName}"
    });
}




    // ✅ GET: api/Users/logout
    // Logs out the user by clearing the session
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Ok("Logged out successfully.");
    }

    // ✅ GET: api/Users/current
    // Gets the currently logged-in user
    [HttpGet("current")]
    public IActionResult GetCurrentUser()
    {
        int? userId = HttpContext.Session.GetInt32("UserID");

        // ✅ Debug: Log session value
        Console.WriteLine($"🔹 Session UserID retrieved: {userId}");

        if (userId == null)
        {
            return Unauthorized("No user is currently logged in.");
        }

        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT UserID, FirstName, LastName, Email FROM Users WHERE UserID = @UserID";
        command.Parameters.Add(new MySqlParameter("@UserID", userId));

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return NotFound("User not found.");
        }

        var user = new
        {
            UserID = reader.GetInt32(0),
            FirstName = reader.GetString(1),
            LastName = reader.GetString(2),
            Email = reader.GetString(3)
        };

        return Ok(user);
    }
}
