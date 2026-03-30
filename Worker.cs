//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Yodol_telegram_bot_.Models.Word;
using Yodol_telegram_bot_.Services.WordService;

namespace Yodol_telegram_bot_
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITelegramBotClient _bot;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;

            var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");

            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("TOKEN TOPILMADI ❌");

            _bot = new TelegramBotClient(token);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // hammasini oladi
            };

            _bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: stoppingToken
            );

            _logger.LogInformation("🤖 Bot ishga tushdi!");

            // app yopilmaguncha ishlaydi
            await Task.Delay(-1, stoppingToken);
        }

        private async Task HandleUpdateAsync(
            ITelegramBotClient bot,
            Update update,
            CancellationToken cancellationToken)
        {
            try
            {
                var service = new WordService();
                if (update.Message is not { } message)
                    return;

                var chatId = message.Chat.Id;
                var text = message.Text;

                _logger.LogInformation($"📩 Message: {text}");

                if (string.IsNullOrWhiteSpace(text))
                    return;

                // 🔥 COMMANDLAR
                if (text.StartsWith("/start"))
                {
                    await bot.SendMessage(
                        chatId,
                        "Salom 👋\nSo‘z yuboring:\n\nApple - olma",
                        cancellationToken: cancellationToken);
                }
                else if (text == "/all")
                {
                    var words = service.GetAllWords();

                    if (!words.Any())
                    {
                        await bot.SendMessage(
                            chatId,
                            "Hozircha so'zlar yo'q 😔");
                        return;
                    }

                    int i = 1;
                    foreach (var word in words)
                    {
                        var textMessage = $"{i}• ||{Escape(word.English)}|| → {Escape(word.Uzbek)}";

                        await bot.SendMessage(
                            chatId,
                            textMessage,
                            parseMode: ParseMode.MarkdownV2,
                            cancellationToken: cancellationToken);

                        i++;
                    }
                }
                else
                {
                    if (text.Contains("-"))
                    {
                        var parts = text.Split("-");

                        var english = parts[0].Trim();
                        var uzbek = parts[1].Trim();

                        var result = service.AddWord(english, uzbek);

                        string status = result.isNew
                            ? "✅ Yangi so‘z qo‘shildi"
                            : "🔁 So‘z yangilandi";

                        var total = service.GetAllWords().Count;

                        var response = $"||{Escape(english)}|| → {Escape(uzbek)}\n\n{status}\n📊 Jami: {total} ta";

                        await bot.SendMessage(
                            chatId,
                            response,
                            parseMode: ParseMode.MarkdownV2,
                            cancellationToken: cancellationToken);
                    }
                    else
                    {
                        await bot.SendMessage(
                            chatId,
                            "!Format noto'g'ri.\nMisol:\nApple - olma",
                            cancellationToken: cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Update handle xato");
            }
        }

        private Task HandleErrorAsync(
            ITelegramBotClient bot,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "💥 Telegram xato");
            return Task.CompletedTask;
        }

        private string Escape(string text)
        {
            // MarkdownV2 uchun maxsus belgilarni qochirish
            return text
                .Replace("_", "\\_")
                .Replace("*", "\\*")
                .Replace("[", "\\[")
                .Replace("]", "\\]")
                .Replace("(", "\\(")
                .Replace(")", "\\)")
                .Replace("~", "\\~")
                .Replace("`", "\\`")
                .Replace(">", "\\>")
                .Replace("#", "\\#")
                .Replace("+", "\\+")
                .Replace("-", "\\-")
                .Replace("=", "\\=")
                .Replace("|", "\\|")
                .Replace("{", "\\{")
                .Replace("}", "\\}")
                .Replace(".", "\\.");
        }
    }
}