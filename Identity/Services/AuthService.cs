using Application.Contracts;
using Application.Models.Identity;
using System.Security.Claims;
using System.Text;
using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Application.Exceptions;

namespace Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtKey = _configuration.GetSection("Jwt:Key").Value;
            _jwtIssuer = _configuration.GetSection("Jwt:Issuer").Value;
            _jwtAudience = _configuration.GetSection("Jwt:Audience").Value;
        }
        public string GenerateToken(AuthResponse user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Username),
                new Claim(ClaimTypes.Role,user.Role)
            };
            var token = new JwtSecurityToken(_jwtIssuer,
                _jwtAudience,
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public AuthResponse Authenticate(AuthRequest userLogin)
        {
            var currentUser = AuthConstants.Users.FirstOrDefault(x => x.Username.ToLower() ==
                userLogin.Username.ToLower() && x.Password == userLogin.Password);

            return currentUser ?? throw new InvalidUserOrPassword(userLogin.Username);            
        }       
    }
}
