namespace LoanOrigination.Api.Services
{
    public class ConsoleEmailService : IEmailService
    {
        private readonly ILogger<ConsoleEmailService> _logger;

        public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendPasswordResetEmailAsync(string email, string resetLink)
        {
            _logger.LogInformation("--- Sending Password Reset Email ---");
            _logger.LogInformation("To: {Email}", email);
            _logger.LogInformation("Please reset your password by clicking the following link:");
            _logger.LogInformation("Link: {ResetLink}", resetLink);
            _logger.LogInformation("------------------------------------");
            return Task.CompletedTask;
        }
    }
}