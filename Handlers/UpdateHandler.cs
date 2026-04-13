//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Yodol_telegram_bot_.Brokers.Loggings;
using Yodol_telegram_bot_.Brokers.Telegrams;
using Yodol_telegram_bot_.Services.Orchestrations;

namespace Yodol_telegram_bot_.Handlers
{
    public class UpdateHandler : BackgroundService
    {
        private readonly ITelegramBroker telegramBroker;
        private readonly IServiceProvider serviceProvider;
        private readonly ILoggingBroker loggingBroker;

        public UpdateHandler(
            ITelegramBroker telegramBroker,
            IServiceProvider serviceProvider,
            ILoggingBroker loggingBroker)
        {
            this.telegramBroker = telegramBroker;
            this.serviceProvider = serviceProvider;
            this.loggingBroker = loggingBroker;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                    UpdateType.CallbackQuery
                }
            };

            this.loggingBroker.LogInformation(
                "Bot ishga tushmoqda. Receiving updates...");

            this.telegramBroker.Client.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandleErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: stoppingToken);

            this.loggingBroker.LogInformation(
                "Bot muvaffaqiyatli ishga tushdi!");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task HandleUpdateAsync(
            ITelegramBotClient botClient,
            Update update,
            CancellationToken cancellationToken)
        {
            try
            {
                using var scope = this.serviceProvider.CreateScope();

                var orchestrationService = scope.ServiceProvider
                    .GetRequiredService<ITelegramOrchestrationService>();

                if (update.Type == UpdateType.CallbackQuery &&
                    update.CallbackQuery is not null)
                {
                    this.loggingBroker.LogInformation(
                        $"Callback query received. " +
                        $"ChatId: {update.CallbackQuery.Message?.Chat.Id}, " +
                        $"Data: {update.CallbackQuery.Data}.");

                    await orchestrationService
                        .ProcessCallbackQueryAsync(update.CallbackQuery);
                }
                else if (update.Type == UpdateType.Message &&
                    update.Message is not null)
                {
                    this.loggingBroker.LogInformation(
                        $"Message received. " +
                        $"ChatId: {update.Message.Chat.Id}, " +
                        $"Text: {update.Message.Text}.");

                    await orchestrationService.ProcessUpdateAsync(update);
                }
            }
            catch (Exception exception)
            {
                this.loggingBroker.LogError(exception);

                long? chatId = update.Message?.Chat.Id
                    ?? update.CallbackQuery?.Message?.Chat.Id;

                if (chatId is not null)
                {
                    try
                    {
                        await this.telegramBroker.SendMessageAsync(
                            chatId.Value,
                            "Xatolik yuz berdi. Iltimos qaytadan urinib ko'ring.");
                    }
                    catch
                    {
                        // xabar yuborishda ham xato bo'lsa, logga yozilgan
                    }
                }
            }
        }

        private Task HandleErrorAsync(
            ITelegramBotClient botClient,
            Exception exception,
            CancellationToken cancellationToken)
        {
            this.loggingBroker.LogCritical(exception);

            return Task.CompletedTask;
        }
    }
}
