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

    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;

    public UserController(SampleDBContext context, IConfiguration configuration, IUserService userService)
    {
        _context = context;
        _userService = userService; // Initialize the service
        _jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT_SECRET_KEY is not set");
    }


    [Authorize]
    [HttpGet("lists")]
    public async Task<ActionResult<List<string>>> GetUsersAsync()
    {
        var users = await _userService.GetUsersAsync();
        return Ok(users);
    }

    [Authorize]
    [HttpGet("{id}")] //receive parameter id
    public async Task<ActionResult> GetUserByIdAsync(long id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
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
        return CreatedAtAction(nameof(GetUsersAsync), users);
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
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        try
        {
            var userLogin = await _userService.LoginAsync(request);
            if (userLogin == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var userToken = await _userService.GenerateTokenUserLoginAsync(userLogin);
            return Ok(new { Token = userToken });
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while login." + ex.ToString());
        }
    }

    // Define the LoginRequest model
    public class LoginRequest
    {
        public required string username { get; set; }
        public required string password { get; set; }
    }
}
