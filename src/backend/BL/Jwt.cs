using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BL.Interfaces;
using DAL.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BL
{
    public class Jwt : IJwt
    {
        private readonly IConfiguration _config;

        public Jwt(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim("idemail", user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddHours(2),
                claims: claims,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}
