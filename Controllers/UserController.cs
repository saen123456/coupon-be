using DotnetWebApiWithEFCodeFirst.Models;
using Microsoft.AspNetCore.Mvc;
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

    [HttpPatch("{id}/update")]
    public ActionResult UpdateUser(long id, Users users)
    {
        if (id == null)
        {
            return BadRequest();
        }
        var user = _context.Users.Find(id);

        if (user == null)
        {
            return NotFound();
        }

        user.username = users.username;
        user.password = users.password;
        user.email = users.email;
        user.is_admin = users.is_admin;

        _context.Users.Update(user);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetUsers), user);
    }
}
