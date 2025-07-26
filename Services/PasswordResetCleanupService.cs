using ElectronicsStore.Services;

namespace ElectronicsStore.Services
{
    public class PasswordResetCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PasswordResetCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(1); // Run every hour

        public PasswordResetCleanupService(
            IServiceProvider serviceProvider,
            ILogger<PasswordResetCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Password Reset Cleanup Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredTokensAsync();
                    await Task.Delay(_cleanupInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in Password Reset Cleanup Service");
                    // Wait a bit before retrying
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }

            _logger.LogInformation("Password Reset Cleanup Service stopped");
        }

        private async Task CleanupExpiredTokensAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var passwordResetService = scope.ServiceProvider.GetRequiredService<IPasswordResetService>();

                await passwordResetService.CleanupExpiredTokensAsync();

                _logger.LogDebug("Password reset tokens cleanup completed successfully");
            }
            catch (Microsoft.Data.Sqlite.SqliteException sqlEx) when (sqlEx.Message.Contains("no such table"))
            {
                _logger.LogWarning("PasswordResetTokens table not found. This is normal on first startup before migration.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cleanup expired password reset tokens");
            }
        }
    }
}
