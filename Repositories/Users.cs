using DotnetWebApiWithEFCodeFirst.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using static coupon_be.Controllers.UserController;

public interface IUserRepository
{
    Task<List<Users>> GetUsersAsync();
    Task<Users?> GetUserByIdAsync(long id);
    Task<Users?> LoginAsync(LoginRequest request);
    // Task<Coupons> GetCouponByCodeAsync(string code);
    // Task AddCouponAsync(Coupons coupon);
    // Task UpdateCouponAsync(Coupons coupon);
}

public class UserRepository : IUserRepository
{
    private readonly SampleDBContext _context;

    public UserRepository(SampleDBContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<List<Users>> GetUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<Users?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.username == request.username && u.password == request.password);
        return user;
    }

    public async Task<Users?> GetUserByIdAsync(long id)
    {
        return await _context.Users.FindAsync(id);
    }

    // public async Task<Coupons> GetCouponByIdAsync(long id)
    // {
    //     return await _context.Coupons.FindAsync(id);
    // }

    // public async Task<Coupons> GetCouponByCodeAsync(string code)
    // {
    //     return await _context.Coupons.FirstOrDefaultAsync(c => c.code == code);
    // }

    // public async Task AddCouponAsync(Coupons coupon)
    // {
    //     await _context.Coupons.AddAsync(coupon);
    //     await _context.SaveChangesAsync();
    // }

    // public async Task UpdateCouponAsync(Coupons coupon)
    // {
    //     _context.Coupons.Update(coupon);
    //     await _context.SaveChangesAsync();
    // }
}