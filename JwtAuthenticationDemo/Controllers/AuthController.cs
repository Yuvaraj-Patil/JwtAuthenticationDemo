using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtAuthenticationDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;

namespace JwtAuthenticationDemo.Controllers
{
    [Route("Authenticate")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EShopConfigDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(EShopConfigDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] UserCreds creds)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u =>
                u.Username == creds.Username && u.PasswordHash == creds.Password);

            if (user == null)
                return Unauthorized("Invalid credentials.");

            var accessToken = GenerateJwtToken(user);
            var refreshToken = Guid.NewGuid().ToString();

            var session = new RefreshSession
            {
                UserId = user.UserId,
                RefreshToken = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(2),
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshSessions.Add(session);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                accessToken,
                refreshToken
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var session = await _context.RefreshSessions
                .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken && s.ExpiryDate > DateTime.UtcNow);

            if (session == null)
                return Unauthorized("Invalid or expired refresh token.");

            var user = await _context.Users.FindAsync(session.UserId);
            if (user == null)
                return Unauthorized("User not found.");

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = Guid.NewGuid().ToString();

            session.RefreshToken = newRefreshToken;
            session.ExpiryDate = DateTime.UtcNow.AddDays(2);
            session.CreatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
#pragma warning disable CS8604 // Possible null reference argument.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
#pragma warning restore CS8604 // Possible null reference argument.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var token = new JwtSecurityToken(
                //issuer: jwtSettings["Issuer"],
                //audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(jwtSettings["AccessTokenExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
