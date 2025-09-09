using System.ComponentModel.DataAnnotations;

namespace LoanOrigination.Api.DTOs
{
    public record RegisterRequest(
        [Required] string FirstName,
        [Required] string LastName, 
        [Required, EmailAddress] string Email, 
        [Required] string Password);
    
    public record LoginRequest([Required, EmailAddress] string Email, [Required] string Password);
    public record AuthResponse(string Token, string FirstName, string Email, string Role);

    public record ForgotPasswordRequest([Required, EmailAddress] string Email);
    public record ResetPasswordRequest([Required] string Token, [Required] string Password);

}
