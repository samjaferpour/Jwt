using Jwt.Contexts;
using Jwt.Dtos;
using Jwt.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly JwtDbContext _context;

        public ProductController(JwtDbContext context)
        {
            this._context = context;
        }
        [Authorize(Roles ="Admin ,Public")]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }
        [Authorize(Roles ="Admin")]
        [HttpPost("[action]")]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto request)
        {
            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }
    }
}
