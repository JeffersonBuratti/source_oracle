using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace source_oracle.Services
{
    public class TokenService
    {
        private readonly string _secretKey;

        public TokenService(string secretKey)
        {
            _secretKey = secretKey;
        }

        public string GenerateToken(string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "yourApp",
                audience: "yourApp",
                claims: claims,
                expires: DateTime.Now.AddHours(1),  // Expiração em 1 hora
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
