//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Yodol_telegram_bot_.Models.Reminders;
using Yodol_telegram_bot_.Models.WordPackages;
using Yodol_telegram_bot_.Models.Words;

namespace Yodol_telegram_bot_.Services.Orchestrations
{
    public partial class TelegramOrchestrationService
    {
        //CALLBACK HANDLERS
        private async ValueTask HandleRevealWordAsync(
            CallbackQuery callbackQuery,
            string data)
        {
            if (!TryParseCallbackGuid(data, "r:", out Guid wordId))
            {
                await this.telegramBroker.AnswerCallbackQueryAsync(
                    callbackQuery.Id,
                    "Noto'g'ri so'rov.");

                return;
            }

            long chatId = callbackQuery.Message?.Chat.Id ?? 0;
            int messageId = callbackQuery.Message?.MessageId ?? 0;

            if (chatId == 0 || messageId == 0)
            {
                await this.telegramBroker.AnswerCallbackQueryAsync(
                    callbackQuery.Id,
                    "Xabar topilmadi.");

                return;
            }

            Word? word = await this.wordService.RetrieveWordByIdAsync(wordId);

            if (word is null)
            {
                await this.telegramBroker.AnswerCallbackQueryAsync(
                    callbackQuery.Id,
                    "So'z topilmadi.");

                return;
            }

            this.loggingBroker.LogInformation(
                $"Word revealed. WordId: {wordId}, Original: {word.Original}.");

            var buttons = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        "✅ O'rganildi",
                        $"m:{word.Id:N}")
                }
            });

            await this.telegramBroker.EditMessageTextAsync(
                chatId,
                messageId,
                $"👁 {word.Original} — {word.Translation}",
                replyMarkup: buttons);

            await this.telegramBroker.AnswerCallbackQueryAsync(
                callbackQuery.Id,
                "So'z ochildi.");
        }

        private async ValueTask HandleRevealAllWordsAsync(
            CallbackQuery callbackQuery,
            string data)
        {
            if (!TryParseCallbackGuid(data, "ra:", out Guid packageId))
            {
                await this.telegramBroker.AnswerCallbackQueryAsync(
                    callbackQuery.Id,
                    "Noto'g'ri so'rov.");

                return;
            }

            long chatId = callbackQuery.Message?.Chat.Id ?? 0;
            int messageId = callbackQuery.Message?.MessageId ?? 0;

            if (chatId == 0 || messageId == 0)
            {
                await this.telegramBroker.AnswerCallbackQueryAsync(
                    callbackQuery.Id,
                    "Xabar topilmadi.");

                return;
            }

            List<Word> words =
                await this.wordService.RetrieveWordsByPackageIdAsync(packageId);

            this.loggingBroker.LogInformation(
                $"Revealing all words. PackageId: {packageId}, WordCount: {words.Count}.");

            var wordLines = words.Select((w, i) =>
                $"{i + 1}. {w.Original} — {w.Translation}");

            await this.telegramBroker.EditMessageTextAsync(
                chatId,
                messageId,
                "📝 So'zlarni eslang!\n\n" +
                string.Join("\n", wordLines) +
                "\n\n✅ Hammasi ochildi!");

            await this.telegramBroker.AnswerCallbackQueryAsync(callbackQuery.Id);
        }

        private async ValueTask HandleLearnWordsAsync(
            CallbackQuery callbackQuery,
            string data)
        {
            if (!TryParseCallbackGuid(data, "rl:", out Guid packageId))
            {
                await this.telegramBroker.AnswerCallbackQueryAsync(
                    callbackQuery.Id,
                    "Noto'g'ri so'rov.");

                return;
            }

            long chatId = callbackQuery.Message?.Chat.Id ?? 0;

            if (chatId == 0)
            {
                await this.telegramBroker.AnswerCallbackQueryAsync(
                    callbackQuery.Id,
                    "Xabar topilmadi.");

                return;
            }

            List<Word> words =
                await this.wordService.RetrieveWordsByPackageIdAsync(packageId);

            List<Word> unlearnedWords = words
                .Where(word => !word.IsLearned)
                .OrderBy(_ => Random.Shared.Next())
                .ToList();

            this.loggingBroker.LogInformation(
                $"Sending learn words list. PackageId: {packageId}, " +
                $"AllWords: {words.Count}, UnlearnedWords: {unlearnedWords.Count}.");

            if (!unlearnedWords.Any())
            {
                await this.telegramBroker.AnswerCallbackQueryAsync(
                    callbackQuery.Id,
                    "Bu to'plamdagi hamma so'zlar o'rganilgan.");

                return;
            }

            await this.telegramBroker.AnswerCallbackQueryAsync(
                callbackQuery.Id,
                "Har bir so'z alohida xabar bo'lib yuborildi.");

            for (int i = 0; i < unlearnedWords.Count; i++)
            {
                Word word = unlearnedWords[i];

                var buttons = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            "👁 So'zni ochish",
                            $"r:{word.Id:N}")
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            "✅ O'rganildi",
                            $"m:{word.Id:N}")
                    }
                });

                await this.telegramBroker.SendMessageWithInlineAsync(
                    chatId,
                    $"{i + 1}\\. ||{EscapeMarkdownV2(word.Original)}|| " +
                    $"\\- {EscapeMarkdownV2(word.Translation)}",
                    replyMarkup: buttons,
                    parseMode: ParseMode.MarkdownV2);
            }
        }

        private async ValueTask HandleMarkWordLearnedAsync(
            CallbackQuery callbackQuery,
            string data)
        {
            if (!TryParseCallbackGuid(data, "m:", out Guid wordId))
            {
                await this.telegramBroker.AnswerCallbackQueryAsync(
                    callbackQuery.Id,
                    "Noto'g'ri so'rov.");

                return;
            }

            long chatId = callbackQuery.Message?.Chat.Id ?? 0;
            int messageId = callbackQuery.Message?.MessageId ?? 0;

            if (chatId == 0 || messageId == 0)
            {
                await this.telegramBroker.AnswerCallbackQueryAsync(
                    callbackQuery.Id,
                    "Xabar topilmadi.");

                return;
            }

            Word? word = await this.wordService.RetrieveWordByIdAsync(wordId);

            if (word is null)
            {
                await this.telegramBroker.AnswerCallbackQueryAsync(
                    callbackQuery.Id,
                    "So'z topilmadi.");

                return;
            }

            if (!word.IsLearned)
            {
                word.IsLearned = true;
                word.LastAskedTime = this.dateTimeBroker.GetCurrentDateTime();
                word.RepeatCount += 1;

                await this.wordService.ModifyWordAsync(word);
            }

            await this.telegramBroker.AnswerCallbackQueryAsync(
                callbackQuery.Id,
                "So'z o'rganilgan deb belgilandi.");

            await this.telegramBroker.EditMessageTextAsync(
                chatId,
                messageId,
                $"✅ O'rganildi: {word.Original} — {word.Translation}");
        }

        private async ValueTask HandleViewPackageAsync(
            CallbackQuery callbackQuery,
            string data)
        {
            if (!TryParseCallbackGuid(data, "p:", out Guid packageId))
            {
                await this.telegramBroker.AnswerCallbackQueryAsync(
                    callbackQuery.Id,
                    "Noto'g'ri so'rov.");

                return;
            }

            long chatId = callbackQuery.Message?.Chat.Id ?? 0;

            if (chatId == 0)
            {
                await this.telegramBroker.AnswerCallbackQueryAsync(
                    callbackQuery.Id,
                    "Xabar topilmadi.");

                return;
            }

            WordPackage? package =
                await this.wordPackageService.RetrieveWordPackageByIdAsync(packageId);

            if (package is null)
            {
                await this.telegramBroker.AnswerCallbackQueryAsync(
                    callbackQuery.Id,
                    "To'plam topilmadi.");

                return;
            }

            List<Word> words =
                await this.wordService.RetrieveWordsByPackageIdAsync(packageId);

            this.loggingBroker.LogInformation(
                $"Viewing package. PackageId: {packageId}, Name: {package.Name}, WordCount: {words.Count}.");

            List<Reminder> reminders =
                await this.reminderService.RetrieveRemindersByUserTelegramIdAsync(chatId);

            Reminder? activeReminder = reminders.FirstOrDefault(r =>
                r.PackageId == packageId && r.IsActive);

            var wordLines = words.Select((w, i) =>
                $"{i + 1}. {w.Original} — {w.Translation}");

            string reminderInfo = activeReminder is not null
                ? $"\n⏱ Har {FormatInterval(activeReminder.Interval)} | " +
                  $"{activeReminder.EndDateTime:dd.MM.yyyy HH:mm} gacha"
                : "\n⚠️ Eslatma o'rnatilmagan";

            await this.telegramBroker.SendMessageAsync(
                chatId,
                $"📦 {package.Name} ({words.Count} ta so'z)" +
                $"{reminderInfo}\n\n" +
                string.Join("\n", wordLines));

            await this.telegramBroker.AnswerCallbackQueryAsync(callbackQuery.Id);
        }

        private async ValueTask HandleTodayWordsAsync(
            CallbackQuery callbackQuery)
        {
            long chatId = callbackQuery.Message?.Chat.Id ?? 0;

            await HandleShowTodayWordsAsync(chatId);

            await this.telegramBroker.AnswerCallbackQueryAsync(callbackQuery.Id);
        }

        private static bool TryParseCallbackGuid(
            string data,
            string prefix,
            out Guid id)
        {
            id = Guid.Empty;

            if (!data.StartsWith(prefix))
            {
                return false;
            }

            string rawId = data[prefix.Length..];
            return Guid.TryParse(rawId, out id);
        }
    }
}
