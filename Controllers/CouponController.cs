using DotnetWebApiWithEFCodeFirst.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace coupon_be.Controllers;

[ApiController]
[Route("[controller]")]
public class CouponController : ControllerBase
{
    private readonly SampleDBContext _context;
    private readonly ICouponService _couponService;

    public CouponController(ICouponService couponService, SampleDBContext context)
    {
        _context = context;
        _couponService = couponService;
    }

    [HttpGet("lists")]
    public ActionResult<List<string>> GetCoupons()
    {
        return Ok(_context.Coupons.ToList());
    }

    [HttpGet("{id}")] //receive parameter id
    public ActionResult GetUserById(long id)
    {
        return Ok(_context.Users.Find(id));
    }

    [HttpGet("generate")]
    public async Task<IActionResult> GenerateCoupon()
    {
        var coupon = new Coupons
        {
            code = await _couponService.GenerateUniqueCouponCode(), // Await the asynchronous method
            expires_at = DateTimeOffset.UtcNow.AddDays(30), // Set an expiry date (example)
            usage_count = 0
        };

        _context.Coupons.Add(coupon);
        await _context.SaveChangesAsync(); // Await the asynchronous SaveChanges method

        return CreatedAtAction(nameof(GetCoupons), new { id = coupon.id }, coupon);
    }

    [HttpPut("update/usage")]
    public async Task<IActionResult> UpdateUser([FromBody] Coupons coupon)
    {
        if (coupon.code == null)
        {
            return BadRequest("Coupon Code is required.");
        }

        // Validate the model
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var cp = await _context.Coupons.Where(x => x.code.Contains(coupon.code)).FirstOrDefaultAsync();
            if (cp == null)
            {
                return NotFound($"Coupon with {coupon.code} not found.");
            }

            cp.usage_count++;
            _context.Coupons.Update(cp);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetCoupons), new { id = cp.id }, cp);
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
