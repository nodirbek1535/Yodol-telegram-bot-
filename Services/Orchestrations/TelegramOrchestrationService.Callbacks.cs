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

            string text = $"👁 {word.Original} — {word.Translation}";
            string? audioUrl = null;

            try
            {
                var dictionaryEntry = await this.dictionaryService.RetrieveWordDetailsAsync(word.Original);
                
                if (dictionaryEntry is not null)
                {
                    string? example = dictionaryEntry.Meanings?
                        .SelectMany(m => m.Definitions)
                        .FirstOrDefault(d => !string.IsNullOrWhiteSpace(d.Example))?
                        .Example;

                    if (!string.IsNullOrWhiteSpace(example))
                    {
                        text += $"\n\n📝 Misol: {example}";
                    }

                    audioUrl = dictionaryEntry.Phonetics?
                        .FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.Audio))?
                        .Audio;
                }
            }
            catch (Exception ex)
            {
                this.loggingBroker.LogError(ex);
            }

            await this.telegramBroker.EditMessageTextAsync(
                chatId,
                messageId,
                text,
                replyMarkup: buttons);

            if (!string.IsNullOrWhiteSpace(audioUrl))
            {
                try
                {
                    await this.telegramBroker.SendAudioAsync(chatId, audioUrl, word.Original);
                }
                catch (Exception ex)
                {
                    this.loggingBroker.LogError(ex);
                }
            }

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

            var buttons = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        "🎯 Test ishlash",
                        $"t:{package.Id:N}")
                }
            });

            await this.telegramBroker.SendMessageWithInlineAsync(
                chatId,
                $"📦 {package.Name} ({words.Count} ta so'z)" +
                $"{reminderInfo}\n\n" +
                string.Join("\n", wordLines),
                replyMarkup: buttons);

            await this.telegramBroker.AnswerCallbackQueryAsync(callbackQuery.Id);
        }

        private async ValueTask HandleTodayWordsAsync(
            CallbackQuery callbackQuery)
        {
            long chatId = callbackQuery.Message?.Chat.Id ?? 0;

            await HandleShowTodayWordsAsync(chatId);

            await this.telegramBroker.AnswerCallbackQueryAsync(callbackQuery.Id);
        }


        private async ValueTask HandleStartTestAsync(
            CallbackQuery callbackQuery,
            string data)
        {
            if (!TryParseCallbackGuid(data, "t:", out Guid packageId))
            {
                await this.telegramBroker.AnswerCallbackQueryAsync(callbackQuery.Id, "Noto'g'ri so'rov.");
                return;
            }

            long chatId = callbackQuery.Message?.Chat.Id ?? 0;
            if (chatId == 0) return;

            await this.telegramBroker.AnswerCallbackQueryAsync(callbackQuery.Id, "Test boshlandi!");
            await SendQuizQuestionAsync(chatId, packageId, null);
        }

        private async ValueTask HandleQuizAnswerAsync(
            CallbackQuery callbackQuery,
            string data)
        {
            string[] parts = data.Split(':');
            if (parts.Length != 3 || !Guid.TryParse(parts[1], out Guid wordId))
            {
                await this.telegramBroker.AnswerCallbackQueryAsync(callbackQuery.Id, "Noto'g'ri so'rov.");
                return;
            }

            bool isCorrect = parts[2] == "1";
            long chatId = callbackQuery.Message?.Chat.Id ?? 0;
            int messageId = callbackQuery.Message?.MessageId ?? 0;

            if (chatId == 0 || messageId == 0) return;

            Word? word = await this.wordService.RetrieveWordByIdAsync(wordId);
            if (word is null)
            {
                await this.telegramBroker.AnswerCallbackQueryAsync(callbackQuery.Id, "So'z topilmadi.");
                return;
            }

            if (isCorrect)
            {
                await this.telegramBroker.AnswerCallbackQueryAsync(callbackQuery.Id, "To'g'ri! ✅");
                
                if (!word.IsLearned)
                {
                    word.IsLearned = true;
                    await this.wordService.ModifyWordAsync(word);
                }

                await SendQuizQuestionAsync(chatId, word.PackageId, messageId);
            }
            else
            {
                await this.telegramBroker.AnswerCallbackQueryAsync(callbackQuery.Id, "Xato! ❌");
                
                var buttons = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            "Keyingisi ➡️",
                            $"t:{word.PackageId:N}")
                    }
                });

                await this.telegramBroker.EditMessageTextAsync(
                    chatId,
                    messageId,
                    $"❌ Noto'g'ri!\n\n🇬🇧 {word.Original}\n🇺🇿 To'g'ri javob: {word.Translation}",
                    replyMarkup: buttons);
            }
        }

        private async ValueTask SendQuizQuestionAsync(long chatId, Guid packageId, int? messageIdToEdit)
        {
            List<Word> packageWords = await this.wordService.RetrieveWordsByPackageIdAsync(packageId);
            List<Word> allWords = await this.wordService.RetrieveWordsByUserTelegramIdAsync(chatId);

            var unlearned = packageWords.Where(w => !w.IsLearned).ToList();
            if (!unlearned.Any())
            {
                if (messageIdToEdit.HasValue)
                {
                    await this.telegramBroker.EditMessageTextAsync(chatId, messageIdToEdit.Value, "🎉 Barcha so'zlarni o'rgandingiz!");
                }
                else
                {
                    await this.telegramBroker.SendMessageAsync(chatId, "🎉 Barcha so'zlarni o'rgandingiz!");
                }
                return;
            }

            Word questionWord = unlearned[Random.Shared.Next(unlearned.Count)];
            
            var wrongOptions = allWords
                .Where(w => w.Id != questionWord.Id && w.Translation != questionWord.Translation)
                .OrderBy(w => Random.Shared.Next())
                .Take(3)
                .ToList();

            var options = wrongOptions.Select(w => new { Text = w.Translation, IsCorrect = false }).ToList();
            options.Add(new { Text = questionWord.Translation, IsCorrect = true });
            
            options = options.OrderBy(o => Random.Shared.Next()).ToList();

            var inlineKeyboard = new List<InlineKeyboardButton[]>();
            foreach (var opt in options)
            {
                inlineKeyboard.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        opt.Text,
                        $"qa:{questionWord.Id:N}:{(opt.IsCorrect ? "1" : "0")}")
                });
            }

            string text = $"🤔 Tarjima qiling:\n\n🇬🇧 {questionWord.Original}";

            if (messageIdToEdit.HasValue)
            {
                await this.telegramBroker.EditMessageTextAsync(
                    chatId,
                    messageIdToEdit.Value,
                    text,
                    replyMarkup: new InlineKeyboardMarkup(inlineKeyboard));
            }
            else
            {
                await this.telegramBroker.SendMessageWithInlineAsync(
                    chatId,
                    text,
                    replyMarkup: new InlineKeyboardMarkup(inlineKeyboard));
            }
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
