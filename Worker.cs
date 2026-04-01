//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Yodol_telegram_bot_.Options;
using Yodol_telegram_bot_.Services.WordService;

namespace Yodol_telegram_bot_;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ITelegramBotClient _bot;
    private readonly IWordService _wordService;
    private readonly BotOptions _botOptions;

    public Worker(
        ILogger<Worker> logger,
        IWordService wordService,
        IOptions<BotOptions> botOptions)
    {
        _logger = logger;
        _wordService = wordService;
        _botOptions = botOptions.Value;

        var token = _botOptions.Token ?? Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Telegram bot token topilmadi.");
        }

        _bot = new TelegramBotClient(token);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions { AllowedUpdates = [] };

        _bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, stoppingToken);
        _logger.LogInformation("Bot ishga tushdi.");

        _ = Task.Run(() => ReminderLoop(stoppingToken), stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        try
        {
            if (update.Message is not { } message || string.IsNullOrWhiteSpace(message.Text))
            {
                return;
            }

            var chatId = message.Chat.Id;
            var text = message.Text.Trim();

            if (text.StartsWith("/start", StringComparison.OrdinalIgnoreCase))
            {
                await bot.SendMessage(chatId, "Salom 👋\nSo‘z yuboring:\nApple - olma", cancellationToken: cancellationToken);
                return;
            }

            if (string.Equals(text, "/all", StringComparison.OrdinalIgnoreCase))
            {
                var words = _wordService.GetChatWords(chatId);
                if (words.Count == 0)
                {
                    await bot.SendMessage(chatId, "Hozircha so'zlar yo'q 😔", cancellationToken: cancellationToken);
                    return;
                }

                for (var i = 0; i < words.Count; i++)
                {
                    var word = words[i];
                    var textMessage = $"{i + 1}• ||{Escape(word.English)}|| → {Escape(word.Uzbek)}";

                    await bot.SendMessage(chatId, textMessage, parseMode: ParseMode.MarkdownV2, cancellationToken: cancellationToken);
                }

                return;
            }

            if (!text.Contains('-', StringComparison.Ordinal))
            {
                await bot.SendMessage(chatId, "Format noto'g'ri. Misol: Apple - olma", cancellationToken: cancellationToken);
                return;
            }

            var parts = text.Split('-', 2, StringSplitOptions.TrimEntries);
            if (parts.Length != 2)
            {
                await bot.SendMessage(chatId, "Format noto'g'ri. Misol: Apple - olma", cancellationToken: cancellationToken);
                return;
            }

            var (word, isNew) = _wordService.AddWord(chatId, parts[0], parts[1]);
            var total = _wordService.GetChatWords(chatId).Count;
            var status = isNew ? "✅ Yangi so‘z qo‘shildi" : "🔁 So‘z yangilandi";
            var response = $"||{Escape(word.English)}|| → {Escape(word.Uzbek)}\n\n{status}\n📊 Jami: {total} ta";

            await bot.SendMessage(chatId, response, parseMode: ParseMode.MarkdownV2, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Update handle xato.");
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Telegram xato.");
        return Task.CompletedTask;
    }

    private async Task ReminderLoop(CancellationToken token)
    {
        var reminderInterval = Math.Max(_botOptions.ReminderIntervalSeconds, 60);

        while (!token.IsCancellationRequested)
        {
            try
            {
                var allWords = _wordService.GetAllWords();
                var chatGroups = allWords.GroupBy(w => w.ChatId);

                foreach (var group in chatGroups)
                {
                    var today = DateTime.Now.Date;
                    var count = group.Count(w =>
                        w.CreatedDate.Date == today &&
                        !w.IsLearned &&
                        (w.Deadline is null || w.Deadline >= DateTime.Now));

                    if (count == 0)
                    {
                        continue;
                    }

                    await _bot.SendMessage(group.Key, $"⏰ Bugun {count} ta so‘z yodlashing kerak edi", cancellationToken: token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reminder xatosi.");
            }

            await Task.Delay(TimeSpan.FromSeconds(reminderInterval), token);
        }
    }

    private static string Escape(string text)
    {
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
