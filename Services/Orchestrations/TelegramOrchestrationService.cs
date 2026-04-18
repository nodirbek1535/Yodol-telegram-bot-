//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using System.Globalization;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Yodol_telegram_bot_.Brokers.DateTimes;
using Yodol_telegram_bot_.Brokers.Loggings;
using Yodol_telegram_bot_.Brokers.Telegrams;
using Yodol_telegram_bot_.Models.Reminders;
using Yodol_telegram_bot_.Models.Users;
using Yodol_telegram_bot_.Models.WordPackages;
using Yodol_telegram_bot_.Models.Words;
using Yodol_telegram_bot_.Services.Foundations.Reminders;
using Yodol_telegram_bot_.Services.Foundations.Users;
using Yodol_telegram_bot_.Services.Foundations.WordPackages;
using Yodol_telegram_bot_.Services.Foundations.Words;

namespace Yodol_telegram_bot_.Services.Orchestrations
{
    public partial class TelegramOrchestrationService : ITelegramOrchestrationService
    {
        private readonly IUserService userService;
        private readonly IWordService wordService;
        private readonly IWordPackageService wordPackageService;
        private readonly IReminderService reminderService;
        private readonly ITelegramBroker telegramBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public TelegramOrchestrationService(
            IUserService userService,
            IWordService wordService,
            IWordPackageService wordPackageService,
            IReminderService reminderService,
            ITelegramBroker telegramBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.userService = userService;
            this.wordService = wordService;
            this.wordPackageService = wordPackageService;
            this.reminderService = reminderService;
            this.telegramBroker = telegramBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public async ValueTask ProcessUpdateAsync(Update update)
        {
            if (update.Message?.Text is null)
            {
                return;
            }

            string text = update.Message.Text;
            long chatId = update.Message.Chat.Id;
            string firstName = update.Message.Chat.FirstName ?? "User";

            this.loggingBroker.LogInformation(
                $"Processing update. ChatId: {chatId}, " +
                $"FirstName: {firstName}, Text: {text}.");

            Models.Users.User user =
                await this.userService.RetrieveOrCreateUserAsync(chatId, firstName);

            if (text == "\ud83d\udd19 Bekor qilish")
            {
                await HandleCancelAsync(chatId, user);
                return;
            }

            switch (user.State)
            {
                case UserState.None:
                    await HandleMenuCommandAsync(text, chatId, user);
                    break;

                case UserState.WaitingForWordCount:
                    await HandleWordCountInputAsync(text, chatId, user);
                    break;

                case UserState.WaitingForWords:
                    await HandleWordsInputAsync(text, chatId, user);
                    break;

                case UserState.WaitingForInterval:
                    await HandleIntervalInputAsync(text, chatId, user);
                    break;

                case UserState.WaitingForEndDateTime:
                    await HandleEndDateTimeInputAsync(text, chatId, user);
                    break;
            }
        }

        public async ValueTask ProcessCallbackQueryAsync(
            CallbackQuery callbackQuery)
        {
            string data = callbackQuery.Data ?? string.Empty;
            long chatId = callbackQuery.Message?.Chat.Id ?? 0;

            this.loggingBroker.LogInformation(
                $"Processing callback. ChatId: {chatId}, Data: {data}.");

            if (data.StartsWith("r:") && !data.StartsWith("ra:"))
            {
                await HandleRevealWordAsync(callbackQuery, data);
            }
            else if (data.StartsWith("rl:"))
            {
                await HandleLearnWordsAsync(callbackQuery, data);
            }
            else if (data.StartsWith("ra:"))
            {
                await HandleRevealAllWordsAsync(callbackQuery, data);
            }
            else if (data.StartsWith("m:"))
            {
                await HandleMarkWordLearnedAsync(callbackQuery, data);
            }
            else if (data.StartsWith("p:"))
            {
                await HandleViewPackageAsync(callbackQuery, data);
            }
            else if (data == "pt")
            {
                await HandleTodayWordsAsync(callbackQuery);
            }
        }

        public async ValueTask ProcessActiveRemindersAsync()
        {
            List<Reminder> activeReminders =
                await this.reminderService.RetrieveActiveRemindersAsync();

            DateTime now = this.dateTimeBroker.GetCurrentDateTime();

            this.loggingBroker.LogInformation(
                $"Processing {activeReminders.Count} active reminders. " +
                $"CurrentTime: {now:dd.MM.yyyy HH:mm}.");

            foreach (Reminder reminder in activeReminders)
            {
                await ProcessSingleReminderAsync(reminder, now);
            }
        }

        //MENU COMMANDS
        private async ValueTask HandleMenuCommandAsync(
            string text, long chatId, Models.Users.User user)
        {
            this.loggingBroker.LogInformation(
                $"Handling menu command. ChatId: {chatId}, Command: {text}.");

            switch (text)
            {
                case "/start":
                    await HandleStartCommandAsync(chatId, user);
                    break;

                case "\u2795 So'z qo'shish":
                    await HandleStartWordAddingAsync(chatId, user);
                    break;

                case "\ud83d\udcda So'zlarim":
                    await HandleShowPackagesAsync(chatId);
                    break;

                case "\ud83d\udcc5 Bugungi so'zlar":
                    await HandleShowTodayWordsAsync(chatId);
                    break;

                case "\ud83d\udcca Statistika":
                    await HandleShowStatisticsAsync(chatId);
                    break;

                default:
                    await this.telegramBroker.SendMessageAsync(
                        chatId,
                        "Noto'g'ri buyruq. Menyudan tanlang.",
                        replyMarkup: GetMainMenuKeyboard());
                    break;
            }
        }

        private async ValueTask HandleStartCommandAsync(
            long chatId, Models.Users.User user)
        {
            this.loggingBroker.LogInformation(
                $"/start command received. ChatId: {chatId}.");

            await this.telegramBroker.SendMessageAsync(
                chatId,
                $"Salom, {user.FirstName}! \ud83d\udc4b\n\n" +
                "Men sizga ingliz tilini o'rganishda yordam beraman.\n" +
                "So'zlar qo'shing va men ularni eslatib turaman!",
                replyMarkup: GetMainMenuKeyboard());
        }

        //WORD ADDING FLOW
        private async ValueTask HandleStartWordAddingAsync(
            long chatId, Models.Users.User user)
        {
            this.loggingBroker.LogInformation(
                $"Starting word adding flow. ChatId: {chatId}.");

            user.State = UserState.WaitingForWordCount;
            await this.userService.ModifyUserAsync(user);

            this.loggingBroker.LogInformation(
                $"User state changed to WaitingForWordCount. ChatId: {chatId}.");

            await this.telegramBroker.SendMessageAsync(
                chatId,
                "Nechta so'z qo'shmoqchisiz?",
                replyMarkup: new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton[] { "5", "10", "15", "20" },
                    new KeyboardButton[] { "\ud83d\udd19 Bekor qilish" }
                })
                { ResizeKeyboard = true });
        }

        private async ValueTask HandleWordCountInputAsync(
            string text, long chatId, Models.Users.User user)
        {
            this.loggingBroker.LogInformation(
                $"Processing word count input. ChatId: {chatId}, Input: {text}.");

            bool isValid = int.TryParse(text, out int count);

            if (!isValid || count <= 0 || count > 50)
            {
                this.loggingBroker.LogWarning(
                    $"Invalid word count input. ChatId: {chatId}, Input: {text}.");

                await this.telegramBroker.SendMessageAsync(
                    chatId,
                    "1 dan 50 gacha son kiriting.");

                return;
            }

            List<WordPackage> existingPackages =
                await this.wordPackageService
                    .RetrieveWordPackagesByUserTelegramIdAsync(chatId);

            var package = new WordPackage
            {
                Id = Guid.NewGuid(),
                UserTelegramId = chatId,
                Name = $"To'plam #{existingPackages.Count + 1}",
                WordCount = count,
                CreatedDate = this.dateTimeBroker.GetCurrentDateTime()
            };

            WordPackage addedPackage =
                await this.wordPackageService.AddWordPackageAsync(package);

            user.State = UserState.WaitingForWords;
            user.PendingWordCount = count;
            user.PendingPackageId = addedPackage.Id;
            await this.userService.ModifyUserAsync(user);

            this.loggingBroker.LogInformation(
                $"Package created. ChatId: {chatId}, " +
                $"PackageId: {addedPackage.Id}, Count: {count}. " +
                $"State changed to WaitingForWords.");

            await this.telegramBroker.SendMessageAsync(
                chatId,
                $"\u270f\ufe0f {count} ta so'zni shu formatda yozing:\n\n" +
                "apple - olma\nbook - kitob\n\n" +
                "Har bir so'zni yangi qatordan yozing:",
                replyMarkup: new ReplyKeyboardMarkup(
                    new KeyboardButton[] { "\ud83d\udd19 Bekor qilish" })
                { ResizeKeyboard = true });
        }

        private async ValueTask HandleWordsInputAsync(
            string text, long chatId, Models.Users.User user)
        {
            this.loggingBroker.LogInformation(
                $"Processing words input. ChatId: {chatId}, " +
                $"ExpectedCount: {user.PendingWordCount}.");

            string[] lines = text.Split('\n',
                StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length != user.PendingWordCount)
            {
                this.loggingBroker.LogWarning(
                    $"Wrong word count. ChatId: {chatId}, " +
                    $"Expected: {user.PendingWordCount}, " +
                    $"Received: {lines.Length}.");

                await this.telegramBroker.SendMessageAsync(
                    chatId,
                    $"{user.PendingWordCount} ta so'z kerak, " +
                    $"siz {lines.Length} ta yozdingiz. Qaytadan yozing:");

                return;
            }

            var words = new List<Word>();

            foreach (string line in lines)
            {
                string[] parts = line.Split('-', 2);

                if (parts.Length != 2 ||
                    string.IsNullOrWhiteSpace(parts[0]) ||
                    string.IsNullOrWhiteSpace(parts[1]))
                {
                    this.loggingBroker.LogWarning(
                        $"Invalid word format. ChatId: {chatId}, " +
                        $"Line: {line}.");

                    await this.telegramBroker.SendMessageAsync(
                        chatId,
                        $"Noto'g'ri format: \"{line}\"\n" +
                        "To'g'ri format: apple - olma");

                    return;
                }

                words.Add(new Word
                {
                    Id = Guid.NewGuid(),
                    UserTelegramId = chatId,
                    PackageId = user.PendingPackageId,
                    Original = parts[0].Trim(),
                    Translation = parts[1].Trim(),
                    CreatedDate = this.dateTimeBroker.GetCurrentDateTime()
                });
            }

            foreach (Word word in words)
            {
                await this.wordService.AddWordAsync(word);
            }

            this.loggingBroker.LogInformation(
                $"All {words.Count} words saved. ChatId: {chatId}, " +
                $"PackageId: {user.PendingPackageId}.");

            user.State = UserState.WaitingForInterval;
            await this.userService.ModifyUserAsync(user);

            string hiddenList = BuildHiddenWordList(words);

            await this.telegramBroker.SendMessageAsync(
                chatId,
                $"\u2705 {words.Count} ta so'z saqlandi!");

            await this.telegramBroker.SendMessageAsync(
                chatId,
                hiddenList,
                parseMode: ParseMode.MarkdownV2);

            await this.telegramBroker.SendMessageAsync(
                chatId,
                "\u23f1 Har qancha vaqtda eslatilsin?",
                replyMarkup: new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton[] { "30 daq", "1 soat", "2 soat", "3 soat" },
                    new KeyboardButton[] { "\ud83d\udd19 Bekor qilish" }
                })
                { ResizeKeyboard = true });
        }

        private async ValueTask HandleIntervalInputAsync(
            string text, long chatId, Models.Users.User user)
        {
            this.loggingBroker.LogInformation(
                $"Processing interval input. ChatId: {chatId}, Input: {text}.");

            TimeSpan? interval = text switch
            {
                "30 daq" => TimeSpan.FromMinutes(30),
                "1 soat" => TimeSpan.FromHours(1),
                "2 soat" => TimeSpan.FromHours(2),
                "3 soat" => TimeSpan.FromHours(3),
                _ => ParseCustomInterval(text)
            };

            if (interval is null || interval.Value.TotalMinutes < 10)
            {
                this.loggingBroker.LogWarning(
                    $"Invalid interval input. ChatId: {chatId}, Input: {text}.");

                await this.telegramBroker.SendMessageAsync(
                    chatId,
                    "Kamida 10 daqiqa bo'lishi kerak.\n" +
                    "Masalan: 30 daq, 1 soat, yoki 45 (daqiqa)");

                return;
            }

            user.PendingInterval = interval.Value;
            user.State = UserState.WaitingForEndDateTime;
            await this.userService.ModifyUserAsync(user);

            this.loggingBroker.LogInformation(
                $"Interval set. ChatId: {chatId}, " +
                $"Interval: {interval.Value}. " +
                $"State changed to WaitingForEndDateTime.");

            await this.telegramBroker.SendMessageAsync(
                chatId,
                "\ud83d\udcc5 Qachongacha eslatilsin?\n" +
                "Sana va vaqtni kiriting:\n" +
                "Masalan: 14.04.2026 14:00",
                replyMarkup: new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton[] { "\ud83d\udd19 Bekor qilish" }
                })
                { ResizeKeyboard = true });
        }

        private async ValueTask HandleEndDateTimeInputAsync(
            string text, long chatId, Models.Users.User user)
        {
            this.loggingBroker.LogInformation(
                $"Processing end datetime input. ChatId: {chatId}, " +
                $"Input: {text}.");

            // ko'p probellarni bitta probelga aylantirish
            string cleanedInput = System.Text.RegularExpressions.Regex
                .Replace(text.Trim(), @"\s+", " ");

            bool parsed = DateTime.TryParseExact(
                cleanedInput,
                new[]
                {
                    "dd.MM.yyyy HH:mm",
                    "dd.MM.yyyy H:mm",
                    "d.MM.yyyy HH:mm",
                    "d.MM.yyyy H:mm",
                    "dd.MM.yyyy",
                    "d.MM.yyyy"
                },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime endDateTime);

            DateTime now = this.dateTimeBroker.GetCurrentDateTime();

            if (!parsed)
            {
                this.loggingBroker.LogWarning(
                    $"Invalid datetime format. ChatId: {chatId}, Input: {text}.");

                await this.telegramBroker.SendMessageAsync(
                    chatId,
                    "Noto'g'ri format.\n" +
                    "To'g'ri format: 14.04.2026 14:00\n" +
                    "Qaytadan kiriting:");

                return;
            }

            if (endDateTime <= now)
            {
                this.loggingBroker.LogWarning(
                    $"Past datetime entered. ChatId: {chatId}, " +
                    $"Input: {text}, Now: {now:dd.MM.yyyy HH:mm}.");

                await this.telegramBroker.SendMessageAsync(
                    chatId,
                    "Vaqt kelajakda bo'lishi kerak.\n" +
                    $"Hozir: {now:dd.MM.yyyy HH:mm}\n" +
                    "Qaytadan kiriting:");

                return;
            }

            TimeSpan totalTime = endDateTime - now;

            if (totalTime < user.PendingInterval)
            {
                this.loggingBroker.LogWarning(
                    $"Insufficient time for interval. ChatId: {chatId}, " +
                    $"TotalTime: {totalTime}, Interval: {user.PendingInterval}.");

                await this.telegramBroker.SendMessageAsync(
                    chatId,
                    $"Vaqt juda qisqa. " +
                    $"Har {FormatInterval(user.PendingInterval)} " +
                    "eslatish uchun kamida shu vaqt kerak.\n" +
                    "Boshqa vaqt kiriting:");

                return;
            }

            var reminder = new Reminder
            {
                Id = Guid.NewGuid(),
                UserTelegramId = chatId,
                PackageId = user.PendingPackageId,
                Interval = user.PendingInterval,
                EndDateTime = endDateTime,
                LastSentAt = now,
                IsActive = true,
                CreatedDate = now
            };

            await this.reminderService.AddReminderAsync(reminder);

            int estimatedCount = (int)(totalTime / reminder.Interval);
            DateTime nextReminder = now.Add(reminder.Interval);

            int wordCount = user.PendingWordCount;

            this.loggingBroker.LogInformation(
                $"Reminder created. ChatId: {chatId}, " +
                $"ReminderId: {reminder.Id}, " +
                $"PackageId: {reminder.PackageId}, " +
                $"Interval: {reminder.Interval}, " +
                $"EndDateTime: {endDateTime:dd.MM.yyyy HH:mm}, " +
                $"EstimatedCount: {estimatedCount}, " +
                $"WordCount: {wordCount}.");

            await this.userService.ResetUserStateAsync(user);

            await this.telegramBroker.SendMessageAsync(
                chatId,
                $"\u2705 Tayyor!\n\n" +
                $"\ud83d\udcda {wordCount} ta so'z\n" +
                $"\u23f1 Har {FormatInterval(reminder.Interval)}\n" +
                $"\ud83d\udcc5 {endDateTime:dd.MM.yyyy HH:mm} gacha\n" +
                $"\ud83d\udd04 Taxminan {estimatedCount} marta eslatiladi\n" +
                $"\ud83d\udd50 Keyingi eslatma: ~{nextReminder:HH:mm} da",
                replyMarkup: GetMainMenuKeyboard());
        }

        //CANCEL
        private async ValueTask HandleCancelAsync(
            long chatId, Models.Users.User user)
        {
            this.loggingBroker.LogInformation(
                $"Cancel requested. ChatId: {chatId}, " +
                $"PreviousState: {user.State}.");

            if (user.PendingPackageId != Guid.Empty)
            {
                WordPackage? package =
                    await this.wordPackageService
                        .RetrieveWordPackageByIdAsync(user.PendingPackageId);

                if (package is not null)
                {
                    List<Word> packageWords =
                        await this.wordService
                            .RetrieveWordsByPackageIdAsync(package.Id);

                    foreach (Word word in packageWords)
                    {
                        await this.wordService.RemoveWordAsync(word);
                    }

                    await this.wordPackageService.RemoveWordPackageAsync(package);

                    this.loggingBroker.LogInformation(
                        $"Cancelled package cleaned up. ChatId: {chatId}, " +
                        $"PackageId: {package.Id}, " +
                        $"WordsRemoved: {packageWords.Count}.");
                }
            }

            await this.userService.ResetUserStateAsync(user);

            await this.telegramBroker.SendMessageAsync(
                chatId,
                "Bekor qilindi.",
                replyMarkup: GetMainMenuKeyboard());
        }

        //PACKAGES VIEW
        private async ValueTask HandleShowPackagesAsync(long chatId)
        {
            this.loggingBroker.LogInformation(
                $"Showing packages. ChatId: {chatId}.");

            List<WordPackage> packages =
                await this.wordPackageService
                    .RetrieveWordPackagesByUserTelegramIdAsync(chatId);

            if (!packages.Any())
            {
                await this.telegramBroker.SendMessageAsync(
                    chatId,
                    "\ud83d\udced Hali so'z qo'shmagansiz.\n" +
                    "\"\u2795 So'z qo'shish\" tugmasini bosing.",
                    replyMarkup: GetMainMenuKeyboard());

                return;
            }

            var lines = packages.Select((p, i) =>
                $"{i + 1}. {p.Name} \u2014 {p.WordCount} ta so'z " +
                $"({p.CreatedDate:dd.MM.yyyy})");

            int totalWords = packages.Sum(p => p.WordCount);

            var buttons = packages.Select(p => new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    p.Name, $"p:{p.Id:N}")
            }).ToList();

            buttons.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "\ud83d\udcc5 Bugungi so'zlar", "pt")
            });

            await this.telegramBroker.SendMessageWithInlineAsync(
                chatId,
                $"\ud83d\udcda Sizning to'plamlaringiz:\n\n" +
                string.Join("\n", lines) +
                $"\n\n\ud83d\udcca Jami: {totalWords} ta so'z",
                replyMarkup: new InlineKeyboardMarkup(buttons));
        }

        private async ValueTask HandleShowTodayWordsAsync(long chatId)
        {
            this.loggingBroker.LogInformation(
                $"Showing today's words. ChatId: {chatId}.");

            DateTime today = this.dateTimeBroker.GetCurrentDateTime().Date;

            List<WordPackage> packages =
                await this.wordPackageService
                    .RetrieveWordPackagesByUserTelegramIdAsync(chatId);

            var todayPackages = packages
                .Where(p => p.CreatedDate.Date == today)
                .ToList();

            if (!todayPackages.Any())
            {
                await this.telegramBroker.SendMessageAsync(
                    chatId,
                    "\ud83d\udced Bugun hali so'z qo'shmagansiz.",
                    replyMarkup: GetMainMenuKeyboard());

                return;
            }

            foreach (WordPackage package in todayPackages)
            {
                List<Word> words =
                    await this.wordService
                        .RetrieveWordsByPackageIdAsync(package.Id);

                var wordLines = words.Select((w, i) =>
                    $"{i + 1}. {w.Original} \u2014 {w.Translation}");

                await this.telegramBroker.SendMessageAsync(
                    chatId,
                    $"\ud83d\udce6 {package.Name}\n\n" +
                    string.Join("\n", wordLines));
            }
        }

        private async ValueTask HandleShowStatisticsAsync(long chatId)
        {
            this.loggingBroker.LogInformation(
                $"Showing statistics. ChatId: {chatId}.");

            List<WordPackage> packages =
                await this.wordPackageService
                    .RetrieveWordPackagesByUserTelegramIdAsync(chatId);

            List<Word> allWords =
                await this.wordService
                    .RetrieveWordsByUserTelegramIdAsync(chatId);

            List<Reminder> reminders =
                await this.reminderService
                    .RetrieveRemindersByUserTelegramIdAsync(chatId);

            int activeReminders = reminders.Count(r => r.IsActive);
            int learnedWords = allWords.Count(w => w.IsLearned);
            DateTime today = this.dateTimeBroker.GetCurrentDateTime().Date;
            int todayWords = allWords.Count(w => w.CreatedDate.Date == today);

            await this.telegramBroker.SendMessageAsync(
                chatId,
                $"\ud83d\udcca Statistika:\n\n" +
                $"\ud83d\udcda To'plamlar: {packages.Count} ta\n" +
                $"\ud83d\udcdd Jami so'zlar: {allWords.Count} ta\n" +
                $"\u2705 O'rganilgan: {learnedWords} ta\n" +
                $"\ud83d\udcc5 Bugun qo'shilgan: {todayWords} ta\n" +
                $"\ud83d\udd14 Faol eslatmalar: {activeReminders} ta",
                replyMarkup: GetMainMenuKeyboard());
        }

        //REMINDER PROCESSING
        private async ValueTask ProcessSingleReminderAsync(
            Reminder reminder, DateTime now)
        {
            if (now >= reminder.EndDateTime)
            {
                reminder.IsActive = false;
                await this.reminderService.ModifyReminderAsync(reminder);

                this.loggingBroker.LogInformation(
                    $"Reminder expired. ReminderId: {reminder.Id}, " +
                    $"UserTelegramId: {reminder.UserTelegramId}.");

                await this.telegramBroker.SendMessageAsync(
                    reminder.UserTelegramId,
                    "\u2705 So'z to'plami muddati tugadi! Yaxshi ish!",
                    replyMarkup: GetMainMenuKeyboard());

                return;
            }

            TimeSpan elapsed = now - reminder.LastSentAt;

            if (elapsed < reminder.Interval)
            {
                return;
            }

            List<Word> words =
                await this.wordService
                    .RetrieveWordsByPackageIdAsync(reminder.PackageId);

            if (!words.Any())
            {
                return;
            }

            this.loggingBroker.LogInformation(
                $"Sending reminder. ReminderId: {reminder.Id}, " +
                $"UserTelegramId: {reminder.UserTelegramId}, " +
                $"WordCount: {words.Count}, " +
                $"Elapsed: {elapsed}.");

            var buttons = new List<InlineKeyboardButton[]>
            {
                InlineKeyboardButton.WithCallbackData(
                    "\ud83d\udcd6 So'zlarni o'rganish",
                    $"rl:{reminder.PackageId:N}")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "\ud83d\udc40 Hammasini och",
                    $"ra:{reminder.PackageId:N}")
            });

            await this.telegramBroker.SendMessageWithInlineAsync(
                reminder.UserTelegramId,
                "\ud83d\udd14 Eslatma vaqti\\!\n" +
                "\ud83d\udcdd So'zlarni ko'rish uchun tugmani bosing\\.",
                replyMarkup: new InlineKeyboardMarkup(buttons),
                parseMode: ParseMode.MarkdownV2);

            reminder.LastSentAt = now;
            await this.reminderService.ModifyReminderAsync(reminder);

            this.loggingBroker.LogInformation(
                $"Reminder sent successfully. ReminderId: {reminder.Id}, " +
                $"NextReminderAt: ~{now.Add(reminder.Interval):HH:mm}.");
        }

    }
}
