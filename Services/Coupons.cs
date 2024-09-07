using DotnetWebApiWithEFCodeFirst.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public interface ICouponService
{
    Task<string> GenerateUniqueCouponCode();
    Task<List<Coupons>> GetCouponsAsync(); // แก้ไขให้มี return type ที่ถูกต้อง
    Task<Coupons> GetCouponByIdAsync(long id); // แก้ไขให้มี return type ที่ถูกต้อง

    Task<Coupons> GenerateCouponAsync();

    Task<Coupons?> UpdateCouponUsageAsync(Coupons coupon);
}
public class CouponService : ICouponService
{
    private readonly ICouponRepository _couponRepository;
    private readonly SampleDBContext _context;
    private static readonly Random _random = new Random();

    private readonly ILogger<CouponService> _logger;



    public CouponService(ICouponRepository couponRepository, ILogger<CouponService> logger, SampleDBContext context)
    {
        _couponRepository = couponRepository;
        _logger = logger;
        _context = context;
    }

    public async Task<List<Coupons>> GetCouponsAsync()
    {
        return await _couponRepository.GetCouponsAsync();
    }

    public async Task<Coupons> GetCouponByIdAsync(long id)
    {
        return await _couponRepository.GetCouponByIdAsync(id);
    }

    public async Task<Coupons> GenerateCouponAsync()
    {
        var coupon = new Coupons
        {
            code = await GenerateUniqueCouponCode(),
            expires_at = DateTimeOffset.UtcNow.AddDays(30),
            usage_count = 0
        };

        _logger.LogInformation(coupon.ToString());

        await _couponRepository.AddCouponAsync(coupon);

        return coupon;
    }

    public async Task<Coupons?> UpdateCouponUsageAsync(Coupons coupon)
    {
        var existingCoupon = await _couponRepository.GetCouponByCodeAsync(coupon.code);
        if (existingCoupon == null)
        {
            return null;
        }

        existingCoupon.usage_count++;
        await _couponRepository.UpdateCouponAsync(existingCoupon);

        return existingCoupon;
    }

    public async Task<string> GenerateUniqueCouponCode()
    {
        string couponCode;
        bool isUnique;

        do
        {
            couponCode = await GenerateCouponCodeAsync();
            isUnique = !await _context.Coupons.AnyAsync(c => c.code == couponCode);
        } while (!isUnique);

        return couponCode;
    }

    public async Task<string> GenerateCouponCodeAsync()
    {
        return await Task.Run(() =>
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        });
    }
}
