using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotnetWebApiWithEFCodeFirst.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Validations.Rules;

namespace coupon_be.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly SampleDBContext _context;
    private readonly string _jwtKey;

    public UserController(SampleDBContext context, IConfiguration configuration)
    {
        _context = context;
        _jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT_SECRET_KEY is not set");
    }

    [Authorize]
    [HttpGet("lists")]
    public ActionResult<List<string>> GetUsers()
    {
        return Ok(_context.Users.ToList());
    }

    [Authorize]
    [HttpGet("{id}")] //receive parameter id
    public ActionResult GetUserById(long id)
    {
        return Ok(_context.Users.Find(id));
    }

    [Authorize]
    [HttpPost("register")]
    public ActionResult CreateUser(Users users)
    {
        if (users == null)
        {
            return BadRequest();
        }
        _context.Users.Add(users);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetUsers), users);
    }

    [Authorize]
    [HttpPut("{id}/update")]
    public IActionResult UpdateUser(long? id, [FromBody] Users users)
    {
        // Check if ID is null
        if (id == null)
        {
            return BadRequest("User ID is required.");
        }

        // Check if the user object is null
        if (users == null)
        {
            return BadRequest("User data is required.");
        }

        // Validate the model
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Find the existing user by ID
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            // Update user properties
            user.username = users.username;
            user.password = users.password;
            user.email = users.email;
            user.is_admin = users.is_admin;

            // Update the user in the database
            _context.Users.Update(user);
            _context.SaveChanges();

            // Return no content to indicate success
            return NoContent();
        }
        catch (DbUpdateException ex)
        {
            // Handle specific database update exceptions if needed
            // Log the exception or handle accordingly
            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating the user in the database.");
        }
        catch (Exception ex)
        {
            // Handle any other exceptions
            // Log the exception or handle accordingly
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        }
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _context.Users.SingleOrDefault(u => u.username == request.username && u.password == request.password);
        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        // Generate JWT token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                 new Claim(ClaimTypes.Name, user.username),
                 new Claim(ClaimTypes.Email, user.email),
                 new Claim("UserId", user.id.ToString()),
                 new Claim("IsAdmin", user.is_admin.ToString())
             }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new { Token = tokenString });
    }

    // Define the LoginRequest model
    public class LoginRequest
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
