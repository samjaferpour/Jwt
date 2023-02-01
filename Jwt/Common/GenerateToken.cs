using Azure.Core;
using Jwt.Dtos;
using Jwt.Dtos.Settings;
using Jwt.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Jwt.Common
{
    public class GenerateToken : IGenerateToken
    {
        private readonly SiteSetting _siteSetting;
        public GenerateToken(IOptions<SiteSetting> options)
        {
            _siteSetting = options.Value;
        }
        public TokenDto GetToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Uid.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role),
            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_siteSetting.JwtConfig.Key)); //("Top Secret Key 112358")); 
            var signingCredential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer =  _siteSetting.JwtConfig.Issuer,//"SampleJwtServer",
                Audience = _siteSetting.JwtConfig.Audience, //"SampleJwtClient",
                IssuedAt = DateTime.Now,
                Expires= DateTime.Now.AddMinutes(2),
                NotBefore= DateTime.Now,
                SigningCredentials = signingCredential,
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var createdToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(createdToken);
            var refreshToken = GetRefreshToken();
            var token = new TokenDto { AccessToken = accessToken, RefreshToken = refreshToken };
            return token;
        }
        private static string GetRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        public static void GetAccessTokenFromRefreshToken(string refreshToken)
        {

        }
    }
}
