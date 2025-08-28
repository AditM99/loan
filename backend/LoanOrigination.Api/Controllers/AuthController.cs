using Microsoft.AspNetCore.Mvc;
using LoanOrigination.Api.DTOs;
using LoanOrigination.Api.Data;
using LoanOrigination.Api.Models;
using LoanOrigination.Api.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LoanOrigination.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtService _jwt;

        public AuthController(AppDbContext db, JwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest req)
        {
            // Validate password policy
            var passwordValidation = ValidatePassword(req.Password);
            if (!passwordValidation.IsValid)
            {
                return BadRequest(new { error = passwordValidation.ErrorMessage });
            }

            if (await _db.Users.AnyAsync(u => u.Email == req.Email))
                return BadRequest(new { error = "Email already exists" });

            var user = new User
            {
                Name = req.Name,
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Role = "User"
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = _jwt.GenerateToken(user);
            return new AuthResponse(token, user.Name, user.Email, user.Role);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest req)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
                return Unauthorized(new { error = "Invalid credentials" });

            var token = _jwt.GenerateToken(user);
            return new AuthResponse(token, user.Name, user.Email, user.Role);
        }

        private (bool IsValid, string ErrorMessage) ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return (false, "Password is required");

            if (password.Length < 8)
                return (false, "Password must be at least 8 characters long");

            if (!password.Any(char.IsLetter))
                return (false, "Password must contain at least one letter");

            if (!password.Any(char.IsDigit))
                return (false, "Password must contain at least one number");

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                return (false, "Password must contain at least one special character");

            return (true, string.Empty);
        }
    }
}
