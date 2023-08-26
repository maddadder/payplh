using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Lib;
using Microsoft.AspNetCore.Components.Authorization;

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
        public string GetAccessToken()
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = System.Text.Encoding.UTF8.GetBytes(_appSecrets.JwtSigningKey); 
            var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Issuer = _appSecrets.BaseAddress,
                Audience = _appSecrets.BaseAddress,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key), Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
