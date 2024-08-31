using DotnetWebApiWithEFCodeFirst.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations.Rules;

namespace coupon_be.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly SampleDBContext _context;

    public ProductController(SampleDBContext context)
    {
        _context = context;
    }

    [HttpGet("lists")]
    public ActionResult<List<string>> GetProducts()
    {
        return Ok(_context.Products.ToList());
    }

    [HttpGet("{id}")] //receive parameter id
    public ActionResult GetProductById(int id)
    {
        return Ok(new { productId = id, name = "product1" });
    }

    public class ProductRequestModel
    {
        public string name { get; set; }
        public string description { get; set; }

        public decimal price { get; set; }

        public int stock { get; set; }

        public IFormFile image { get; set; }
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult> CreateProduct([FromForm] ProductRequestModel products)
    {
        var pd = new Products();

        if (products == null)
        {
            return BadRequest();
        }

        // ตรวจสอบว่าผลิตภัณฑ์ที่ชื่อเดียวกันมีอยู่แล้วหรือไม่
        var existingProduct = await _context.Products
            .Where(x => x.name == products.name)
            .FirstOrDefaultAsync();

        pd.name = products.name;
        pd.description = products.description;
        pd.price = products.price;
        pd.stock = products.stock;

        if (existingProduct != null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error: Duplicate product name in the database.");
        }

        // ตรวจสอบและบันทึกภาพหากมีการอัปโหลด
        if (products.image != null && products.image.Length > 0)
        {
            var imagePath = Path.Combine("wwwroot/images", products.image.FileName);

            // ตรวจสอบว่าโฟลเดอร์มีอยู่แล้วหรือยัง ถ้าไม่มีให้สร้างขึ้นมา
            Directory.CreateDirectory(Path.GetDirectoryName(imagePath)!);

            // บันทึกภาพลงในเส้นทางที่กำหนด
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await products.image.CopyToAsync(stream);
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host}"; // Get base URL
            var fullImagePath = $"{baseUrl}/images/{products.image.FileName}"; // Full URL of the image

            // เก็บเส้นทางของภาพในฐานข้อมูล
            pd.image = fullImagePath;
        }
        // else
        // {
        //     products.ImagePath = ""; // หรือสามารถใช้ค่าเริ่มต้นอื่น ๆ ได้
        // }

        _context.Products.Add(pd);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProducts), new { id = pd.id }, pd);
    }

}
