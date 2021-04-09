using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStore.Database;
using WebStore.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using WebStore.Models.Configs;

namespace WebStore.Application.CreateUser
{
    public class CreateUser
    {
        private JWTConfig _jwtConfig;
        private ApplicationDbContext _context;

        public CreateUser(JWTConfig config, ApplicationDbContext context)
        {
            _jwtConfig = config;
            _context = context;
        }
        async public Task<Response> Do(Request request)
        {
            User user = _context.Users.Where(x => x.Email == request.Email.ToLower()).FirstOrDefault();

            if(user != null)
            {
                return new Response {Status = 409 };
            }
            else
            {
                return new Response {
                   Status = 201,
                   Token = GenerateJWT(await AddUser(request.Email.ToLower(), request.Password)) 
                };
            }
        }

        async private Task<User> AddUser(string email, string password)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

            User user = new User
            {
                Email = email,
                PasswordHash = hashedPassword,
                Salt = Convert.ToBase64String(salt),
                UserRole = "User",
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
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
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodedToken;
        }


        public class Response
        {
            public int Status { get; set; }
            public string Token { get; set; }
        }
        public class Request
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }

}
