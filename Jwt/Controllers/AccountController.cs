using Azure.Core;
using Jwt.Common;
using Jwt.Contexts;
using Jwt.Dtos;
using Jwt.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JwtDbContext _context;
        private readonly IGenerateToken _generateToken;

        public AccountController(JwtDbContext context, IGenerateToken generateToken)
        {
            this._context = context;
            this._generateToken = generateToken;
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            var userIsExist = await _context.Users.Where(u => u.Username == request.Username).AnyAsync();
            if (userIsExist)
            {
                return Conflict("User already exists");
            }
            var user = new User
            {
                Uid = Guid.NewGuid(),
                Username = request.Username,
                Password = PasswordManager.GetHash(request.Password), //request.Password,
                FullName = request.FullName,
                Role = request.Role,
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var user = await _context.Users.Where(u => u.Username == request.Username).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound("User not found");
            }
            if (user.Password !=PasswordManager.GetHash(request.Password)) //request.Password)
            {
                return NotFound("User not found");
            }
            
            var token = _generateToken.GetToken(user);
            var newToken = new Token
            {
                RefreshToken = token.RefreshToken,
                RefreshTokenExpireTime = DateTime.Now.AddDays(1),
                UserUid = user.Uid,
            };
            await _context.Tokens.AddAsync(newToken);
            await _context.SaveChangesAsync();

            return Ok(token);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> RgenerateToken([FromBody] RgenerateTokenDto request)
        {
            var currentToken = await _context.Tokens.Where(t => t.RefreshToken == request.refreshToken).FirstOrDefaultAsync();
            if (currentToken == null)
            {
                return NotFound("Invalid refresh token");
            }
            else
            {
                var user = await _context.Users.Where(u => u.Uid == currentToken.UserUid).FirstOrDefaultAsync();
                if (user == null)
                {
                    return NotFound("User not found");
                }
                
                var token = _generateToken.GetToken(user);

                currentToken.RefreshToken = token.RefreshToken;
                currentToken.RefreshTokenExpireTime = DateTime.Now.AddDays(1);
                await _context.SaveChangesAsync();

                return Ok(token);
            }
        }
    }
}

