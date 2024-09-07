using DotnetWebApiWithEFCodeFirst.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICouponRepository
{
    Task<List<Coupons>> GetCouponsAsync();
    Task<Coupons> GetCouponByIdAsync(long id);
    Task<Coupons> GetCouponByCodeAsync(string code);
    Task AddCouponAsync(Coupons coupon);
    Task UpdateCouponAsync(Coupons coupon);
}

public class CouponRepository : ICouponRepository
{
    private readonly SampleDBContext _context;

    public CouponRepository(SampleDBContext context)
    {
        _context = context;
    }

    public async Task<List<Coupons>> GetCouponsAsync()
    {
        return await _context.Coupons.ToListAsync();
    }

    public async Task<Coupons> GetCouponByIdAsync(long id)
    {
        return await _context.Coupons.FindAsync(id);
    }

    public async Task<Coupons> GetCouponByCodeAsync(string code)
    {
        return await _context.Coupons.FirstOrDefaultAsync(c => c.code == code);
    }

    public async Task AddCouponAsync(Coupons coupon)
    {
        await _context.Coupons.AddAsync(coupon);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCouponAsync(Coupons coupon)
    {
        _context.Coupons.Update(coupon);
        await _context.SaveChangesAsync();
    }
}