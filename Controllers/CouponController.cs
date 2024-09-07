using DotnetWebApiWithEFCodeFirst.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace coupon_be.Controllers;

[ApiController]
[Route("[controller]")]
public class CouponController : ControllerBase
{
    private readonly ICouponService _couponService;
    private readonly ICouponRepository _couponRepository;
    private readonly SampleDBContext _context;

    public CouponController(ICouponService couponService, ICouponRepository couponRepository, SampleDBContext context)
    {
        _couponService = couponService;
        _couponRepository = couponRepository;
        _context = context;
    }

    [HttpGet("lists")]
    public async Task<ActionResult<List<Coupons>>> GetCoupons()
    {
        var coupons = await _couponService.GetCouponsAsync();
        return Ok(coupons);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetCouponById(long id)
    {
        var coupon = await _couponService.GetCouponByIdAsync(id);
        if (coupon == null)
        {
            return NotFound($"Coupon with ID {id} not found.");
        }
        return Ok(coupon);
    }

    [HttpGet("generate")]
    public async Task<IActionResult> GenerateCoupon()
    {
        try
        {
            var coupon = await _couponService.GenerateCouponAsync();
            return Ok(coupon);
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while generating the coupon." + ex.ToString());
        }
    }

    [HttpPut("update/usage")]
    public async Task<IActionResult> UpdateUsage([FromBody] Coupons coupon)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedCoupon = await _couponService.UpdateCouponUsageAsync(coupon);
        if (updatedCoupon == null)
        {
            return NotFound($"Coupon with code {coupon.code} not found.");
        }

        return Ok(updatedCoupon);
    }
}
