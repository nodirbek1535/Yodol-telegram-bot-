//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Yodol_telegram_bot_.Brokers.Telegrams
{
    public interface ITelegramBroker
    {
        ITelegramBotClient Client { get; }

        ValueTask SendMessageAsync(
            long chatId,
            string text,
            ReplyKeyboardMarkup? replyMarkup = null,
            ParseMode parseMode = ParseMode.None);

        ValueTask SendMessageWithInlineAsync(
            long chatId,
            string text,
            InlineKeyboardMarkup? replyMarkup = null,
            ParseMode parseMode = ParseMode.None);

        ValueTask AnswerCallbackQueryAsync(
            string callbackQueryId,
            string? text = null);

        ValueTask EditMessageTextAsync(
            long chatId,
            int messageId,
            string text,
            InlineKeyboardMarkup? replyMarkup = null,
            ParseMode parseMode = ParseMode.None);
    }
}
