//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Yodol_telegram_bot_.Brokers.Telegrams
{
    public class TelegramBroker : ITelegramBroker
    {
        public ITelegramBotClient Client { get; }

        public TelegramBroker(IConfiguration configuration)
        {
            var token = configuration["TelegramBot:Token"]
                ?? Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidOperationException(
                    "TELEGRAM_BOT_TOKEN topilmadi.");
            }

            this.Client = new TelegramBotClient(token);
        }

        public async ValueTask SendMessageAsync(
            long chatId,
            string text,
            ReplyKeyboardMarkup? replyMarkup = null,
            ParseMode parseMode = ParseMode.None)
        {
            await this.Client.SendMessage(
                chatId: chatId,
                text: text,
                replyMarkup: replyMarkup,
                parseMode: parseMode);
        }

        public async ValueTask SendMessageWithInlineAsync(
            long chatId,
            string text,
            InlineKeyboardMarkup? replyMarkup = null,
            ParseMode parseMode = ParseMode.None)
        {
            await this.Client.SendMessage(
                chatId: chatId,
                text: text,
                replyMarkup: replyMarkup,
                parseMode: parseMode);
        }

        public async ValueTask AnswerCallbackQueryAsync(
            string callbackQueryId,
            string? text = null)
        {
            await this.Client.AnswerCallbackQuery(
                callbackQueryId: callbackQueryId,
                text: text);
        }

        public async ValueTask EditMessageTextAsync(
            long chatId,
            int messageId,
            string text,
            InlineKeyboardMarkup? replyMarkup = null,
            ParseMode parseMode = ParseMode.None)
        {
            await this.Client.EditMessageText(
                chatId: chatId,
                messageId: messageId,
                text: text,
                replyMarkup: replyMarkup,
                parseMode: parseMode);
        }
    }
}
