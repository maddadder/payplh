using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lib;
using Microsoft.IdentityModel.Tokens;

namespace BlazorHosted.Services
{
    public class JwtTokenService
    {
        private readonly AppSecrets _appSecrets;
        private readonly HttpClient _httpClient;
        public JwtTokenService(
            AppSecrets appSecrets,
            HttpClient httpClient)
        {
            _appSecrets = appSecrets;
            _httpClient = httpClient;
        }
        public string GetLocalAdminAccessToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_appSecrets.JwtSigningKey); 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Role, "local.admin"),
                }),
                Issuer = _appSecrets.BaseAddress,
                Audience = _appSecrets.BaseAddress,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
