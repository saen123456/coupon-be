using DotnetWebApiWithEFCodeFirst.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations.Rules;

namespace coupon_be.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly SampleDBContext _context;
    public UserController(SampleDBContext context)
    {
        _context = context;
    }

    [HttpGet("lists")]
    public ActionResult<List<string>> GetUsers()
    {
        return Ok(_context.Users.ToList());
    }

    [HttpGet("{id}")] //receive parameter id
    public ActionResult GetUserById(long id)
    {
        return Ok(_context.Users.Find(id));
    }

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

}
