using DotnetWebApiWithEFCodeFirst.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ICouponService, CouponService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SampleDBContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "SSS .NET 8 Demo", Version = "v1" })
);

// Add Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("146d7a0344376048a7440ea3381d3680753f923e4a33f49334b093985d4cc33d")) // ควรใช้ key เดียวกับที่ใช้สร้าง token
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder
     .AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader());

app.UseHttpsRedirection();

app.UsePathBase("/api");

app.UseStaticFiles(); // ใช้สำหรับเสิร์ฟไฟล์สาธารณะ

app.UseRouting();

app.UseAuthentication();  // เรียกใช้ก่อน Authorization

app.UseAuthorization();

app.MapControllers();

app.Run();
