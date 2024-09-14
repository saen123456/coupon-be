using DotnetWebApiWithEFCodeFirst.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using static coupon_be.Controllers.UserController;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

public interface IUserService
{
    // Task<string> GenerateUniqueCouponCode();
    Task<List<Users>> GetUsersAsync(); // แก้ไขให้มี return type ที่ถูกต้อง

    Task<Users?> GetUserByIdAsync(long id);

    Task<Users?> LoginAsync(LoginRequest request);

    Task<string> GenerateTokenUserLoginAsync(Users user);
    // Task<Coupons> GetCouponByIdAsync(long id); // แก้ไขให้มี return type ที่ถูกต้อง

    // Task<Coupons> GenerateCouponAsync();

    // Task<Coupons?> UpdateCouponUsageAsync(Coupons coupon);
}
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly SampleDBContext _context;
    private static readonly Random _random = new Random();

    private readonly ILogger<UserService> _logger;

    private readonly string _jwtKey;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger, SampleDBContext context, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _logger = logger;
        _context = context;
        _jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT_SECRET_KEY is not set");
    }

    public async Task<List<Users>> GetUsersAsync()
    {
        return await _userRepository.GetUsersAsync();
    }

    public async Task<Users?> GetUserByIdAsync(long id)
    {
        return await _userRepository.GetUserByIdAsync(id);
    }

    public async Task<Users?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.LoginAsync(request);
        return user;
    }

    public async Task<string> GenerateTokenUserLoginAsync(Users user)
    {

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

        return tokenString;
    }

    // public async Task<Coupons> GetCouponByIdAsync(long id)
    // {
    //     return await _couponRepository.GetCouponByIdAsync(id);
    // }

    // public async Task<Coupons> GenerateCouponAsync()
    // {
    //     var coupon = new Coupons
    //     {
    //         code = await GenerateUniqueCouponCode(),
    //         expires_at = DateTimeOffset.UtcNow.AddDays(30),
    //         usage_count = 0
    //     };

    //     _logger.LogInformation(coupon.ToString());

    //     await _couponRepository.AddCouponAsync(coupon);

    //     return coupon;
    // }

    // public async Task<Coupons?> UpdateCouponUsageAsync(Coupons coupon)
    // {
    //     var existingCoupon = await _couponRepository.GetCouponByCodeAsync(coupon.code);
    //     if (existingCoupon == null)
    //     {
    //         return null;
    //     }

    //     existingCoupon.usage_count++;
    //     await _couponRepository.UpdateCouponAsync(existingCoupon);

    //     return existingCoupon;
    // }

    // public async Task<string> GenerateUniqueCouponCode()
    // {
    //     string couponCode;
    //     bool isUnique;

    //     do
    //     {
    //         couponCode = await GenerateCouponCodeAsync();
    //         isUnique = !await _context.Coupons.AnyAsync(c => c.code == couponCode);
    //     } while (!isUnique);

    //     return couponCode;
    // }

    // public async Task<string> GenerateCouponCodeAsync()
    // {
    //     return await Task.Run(() =>
    //     {
    //         const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    //         return new string(Enumerable.Repeat(chars, 10)
    //             .Select(s => s[_random.Next(s.Length)]).ToArray());
    //     });
    // }
}
