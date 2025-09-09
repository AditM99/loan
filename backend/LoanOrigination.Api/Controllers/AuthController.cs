using Microsoft.AspNetCore.Mvc;
using LoanOrigination.Api.DTOs;
using LoanOrigination.Api.Data;
using LoanOrigination.Api.Models;
using LoanOrigination.Api.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace LoanOrigination.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtService _jwt;
        private readonly IEmailService _emailService;

        public AuthController(AppDbContext db, JwtService jwt, IEmailService emailService)
        {
            _db = db;
            _jwt = jwt;
            _emailService = emailService;
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
                FirstName = req.FirstName,
                LastName = req.LastName,
                Email = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Role = "User"
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();


            var token = _jwt.GenerateToken(user);
            return new AuthResponse(token, $"{user.FirstName} {user.LastName}".Trim(), user.Email, user.Role);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest req)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
                return Unauthorized(new { error = "Invalid credentials" });
            var token = _jwt.GenerateToken(user);
            return new AuthResponse(token, $"{user.FirstName} {user.LastName}".Trim(), user.Email, user.Role);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest req)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
            if (user != null)
            {
                // Generate token
                var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
                user.PasswordResetToken = token;
                user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour
                await _db.SaveChangesAsync();

                // Send email (simulated)
                var resetLink = $"http://localhost:5173/reset-password/{token}"; // Use your frontend URL
                await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);
            }

            // Always return a positive response to prevent email enumeration
            return Ok(new { message = "If an account with that email exists, a password reset link has been sent." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest req)
        {
            var user = await _db.Users.FirstOrDefaultAsync(
                u => u.PasswordResetToken == req.Token && u.PasswordResetTokenExpires > DateTime.UtcNow);

            if (user == null)
            {
                return BadRequest(new { error = "Invalid or expired password reset token." });
            }

            var passwordValidation = ValidatePassword(req.Password);
            if (!passwordValidation.IsValid)
            {
                return BadRequest(new { error = passwordValidation.ErrorMessage });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpires = null;

            await _db.SaveChangesAsync();

            return Ok(new { message = "Password has been reset successfully." });
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
