//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Yodol_telegram_bot_.Brokers.Loggings;
using Yodol_telegram_bot_.Services.Orchestrations;

namespace Yodol_telegram_bot_.Workers
{
    public class ReminderWorker : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ILoggingBroker loggingBroker;

        public ReminderWorker(
            IServiceProvider serviceProvider,
            ILoggingBroker loggingBroker)
        {
            this.serviceProvider = serviceProvider;
            this.loggingBroker = loggingBroker;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            this.loggingBroker.LogInformation(
                "ReminderWorker ishga tushdi. " +
                "Har 30 sekundda eslatmalar tekshiriladi.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = this.serviceProvider.CreateScope();

                    var orchestrationService = scope.ServiceProvider
                        .GetRequiredService<ITelegramOrchestrationService>();

                    await orchestrationService.ProcessActiveRemindersAsync();
                }
                catch (Exception exception)
                {
                    this.loggingBroker.LogError(exception);
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(30),
                    stoppingToken);
            }

            this.loggingBroker.LogInformation(
                "ReminderWorker to'xtatildi.");
        }
    }
}
