//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using System.Globalization;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace Yodol_telegram_bot_.Services.Orchestrations
{
    public partial class TelegramOrchestrationService
    {
        //HELPERS
        private static string BuildHiddenWordList(List<Models.Words.Word> words)
        {
            var builder = new StringBuilder();

            for (int i = 0; i < words.Count; i++)
            {
                string original = EscapeMarkdownV2(words[i].Original);
                string translation = EscapeMarkdownV2(words[i].Translation);

                builder.AppendLine(
                    $"{i + 1}\\. ||{original}|| \\- {translation}");
            }

            return builder.ToString().TrimEnd();
        }

        private static string EscapeMarkdownV2(string text)
        {
            char[] reserved = new[]
            {
                '_', '*', '[', ']', '(', ')', '~', '`', '>',
                '#', '+', '-', '=', '|', '{', '}', '.', '!'
            };

            var builder = new StringBuilder();

            foreach (char c in text)
            {
                if (c == '\\' || Array.IndexOf(reserved, c) >= 0)
                {
                    builder.Append('\\');
                }

                builder.Append(c);
            }

            return builder.ToString();
        }

        private static TimeSpan? ParseCustomInterval(string input)
        {
            input = input.Trim().ToLower();

            if (input.EndsWith("daq"))
            {
                string num = input.Replace("daq", "").Trim();

                if (double.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out double minutes) ||
                    double.TryParse(num, out minutes))
                {
                    return TimeSpan.FromMinutes(minutes);
                }
            }

            if (input.EndsWith("soat"))
            {
                string num = input.Replace("soat", "").Trim();

                if (double.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out double hours) ||
                    double.TryParse(num, out hours))
                {
                    return TimeSpan.FromHours(hours);
                }
            }

            if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double rawMinutes) ||
                double.TryParse(input, out rawMinutes))
            {
                return TimeSpan.FromMinutes(rawMinutes);
            }

            return null;
        }

        private static string FormatInterval(TimeSpan interval)
        {
            if (interval.TotalMinutes < 60)
            {
                return $"{(int)interval.TotalMinutes} daqiqada";
            }

            if (interval.TotalHours % 1 == 0)
            {
                return $"{(int)interval.TotalHours} soatda";
            }

            return $"{interval.TotalHours:F1} soatda";
        }

        private static ReplyKeyboardMarkup GetMainMenuKeyboard()
        {
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "\u2795 So'z qo'shish", "\ud83d\udcda So'zlarim" },
                new KeyboardButton[] { "\ud83d\udcc5 Bugungi so'zlar", "\ud83d\udcca Statistika" },
            })
            { ResizeKeyboard = true };
        }
    }
}
