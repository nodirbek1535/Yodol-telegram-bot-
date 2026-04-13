//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Telegram.Bot.Types;

namespace Yodol_telegram_bot_.Services.Orchestrations
{
    public interface ITelegramOrchestrationService
    {
        ValueTask ProcessUpdateAsync(Update update);
        ValueTask ProcessCallbackQueryAsync(CallbackQuery callbackQuery);
        ValueTask ProcessActiveRemindersAsync();
    }
}
