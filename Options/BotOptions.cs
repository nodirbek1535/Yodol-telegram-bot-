//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

namespace Yodol_telegram_bot_.Options;

public sealed class BotOptions
{
    public const string SectionName = "Bot";

    public string? Token { get; set; }
    public int ReminderIntervalSeconds { get; set; } = 3600;
}
