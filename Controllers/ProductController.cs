using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations.Rules;

namespace coupon_be.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    [HttpGet("lists")]
    public ActionResult<List<string>> GetProducts()
    {
        var products = new List<string>();
        products.Add("product1");
        products.Add("product2");
        products.Add("product3");

        return Ok(products);
    }

    [HttpGet("{id}")] //receive parameter id
    public ActionResult GetProductById(int id)
    {
        return Ok(new { productId = id, name = "product1" });
    }
}
