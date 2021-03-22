using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebStore.Database;
using WebStore.Models;
using WebStore.Models.Configs;

namespace WebStore.Application.LoginUser
{
    public class LoginUser
    {
        private JWTConfig _jwtConfig;
        private ApplicationDbContext _context;

        public LoginUser(JWTConfig config, ApplicationDbContext context)
        {
            _jwtConfig = config;
            _context = context;
        }

        public Response Do(Request request)
        {

            var user = AuthenticateUser(request.Email, request.Password);
            if(user != null)
            {
                var tokenStr = GenerateJWT(user);
                return new Response { Token = tokenStr};
            }
            return new Response();
        }

        private User AuthenticateUser(string email, string password)
        {
            User user = _context.Users.Where(x => x.Email == email).Select(x => new User
            {
                Id = x.Id,
                Email = x.Email,
                PasswordHash = x.PasswordHash,
                UserRole = x.UserRole,
                Salt = x.Salt
            }).FirstOrDefault();
            if (user == null) return null;
            byte[] salt = Convert.FromBase64String(user.Salt);

            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
              password: password,
              salt: salt,
              prf: KeyDerivationPrf.HMACSHA1,
              iterationCount: 10000,
              numBytesRequested: 256 / 8));

            if (email == user.Email && hashedPassword == user.PasswordHash)
            {
                return user;
            }
            
            return null;
        }

        private string GenerateJWT(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {                
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("role", user.UserRole)
            };
            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Issuer,
                claims,
                expires: DateTime.Now.AddMinutes(60*24),
                signingCredentials: credentials
            );

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodedToken;
        }

        public class Response
        {
            public string Token { get; set; }
        }
        public class Request
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
