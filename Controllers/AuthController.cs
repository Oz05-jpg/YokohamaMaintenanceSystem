using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using YokohamaMaintenanceSystem.Data;
using YokohamaMaintenanceSystem.DTOs;
using YokohamaMaintenanceSystem.Models;
namespace YokohamaMaintenanceSystem.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        private readonly AppDbContext _db;
        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config, AppDbContext db)
        {
            _userManager = userManager;
            _config = config;
            _db = db;
        }

        // Login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized();

            var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!valid) return Unauthorized();

            var accessToken = GenerateToken(user);
            var refreshToken = GenerateRefreshToken();// สร้าง random token

            // บันทึกลง Db
            _db.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });
            await _db.SaveChangesAsync();
            return Ok(new { accessToken, refreshToken });
        }

        //refresh
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            // หา Token ใน Db
            var stored = await _db.RefreshTokens.
                FirstOrDefaultAsync(r => r.Token == refreshToken);

            // ตรวจ: ไม่มี / revoked / หมดอายุ → 401
            if (stored == null || stored.IsRevoked || stored.ExpiresAt < DateTime.UtcNow)
                return Unauthorized();

            // หา user 
            var user = await _userManager.FindByIdAsync(stored.UserId);
            if (user == null) return Unauthorized();

            // Token Rotation: revoke ตัวเก่า
            stored.IsRevoked = true;

            // ออก Token ใหม่
            var newAccessToken = GenerateToken(user);
            var newRefreshToken = GenerateRefreshToken();

            _db.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });

            await _db.SaveChangesAsync();
            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }
        //logout
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            var stored = await _db.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == refreshToken);
            if (stored != null)
            {
                stored.IsRevoked = true;
                await _db.SaveChangesAsync();

            }
            return Ok(new { message = "Logged out" });
        }

        // helper — สร้าง random string 64 bytes
        private string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        // GenerateToken
        private string GenerateToken(ApplicationUser user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _config["JwtSettings:SecretKey"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Email,user.Email!)
            };

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }


    }

}
