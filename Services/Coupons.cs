using System;
using System.Linq;
using System.Threading.Tasks;
using DotnetWebApiWithEFCodeFirst.Models;
using Microsoft.EntityFrameworkCore;

public interface ICouponService
{
    Task<string> GenerateUniqueCouponCode();
    // Task AddCouponAsync(Coupons coupon);
    // Task<Coupons> GetCouponByIdAsync(int id);
}

public class CouponService : ICouponService
{
    private readonly SampleDBContext _context;
    private static Random _random = new Random();

    public CouponService(SampleDBContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateUniqueCouponCode()
    {
        string couponCode;
        bool isUnique;

        do
        {
            couponCode = GenerateCouponCode();
            isUnique = !await _context.Coupons.AnyAsync(c => c.code == couponCode);
        } while (!isUnique);

        return couponCode;
    }

    private string GenerateCouponCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, 10)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }

    // public async Task AddCouponAsync(Coupons coupon)
    // {
    //     if (coupon == null) throw new ArgumentNullException(nameof(coupon));
    //     _context.Coupons.Add(coupon);
    //     await _context.SaveChangesAsync();
    // }

    // public async Task<Coupons> GetCouponByIdAsync(int id)
    // {
    //     return await _context.Coupons.FindAsync(id);
    // }
}