using Jwt.Contexts;
using Jwt.Dtos;
using Jwt.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Jwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly JwtDbContext _context;

        public ResourceController(JwtDbContext context)
        {
            this._context = context;
        }
        [Authorize(Roles = "Admin ,Public")]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllProducts()
        {
            return Ok();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("[action]")]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto request)
        {
            return Ok();
        }
    }
}
