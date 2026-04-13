//===============================================================
//NODIRBEKNING telegram uchun shaxsiy boti!!!
//===============================================================

using Serilog;
using Yodol_telegram_bot_.Brokers.DateTimes;
using Yodol_telegram_bot_.Brokers.Loggings;
using Yodol_telegram_bot_.Brokers.Storages;
using Yodol_telegram_bot_.Brokers.Telegrams;
using Yodol_telegram_bot_.Handlers;
using Yodol_telegram_bot_.Services.Foundations.Reminders;
using Yodol_telegram_bot_.Services.Foundations.Users;
using Yodol_telegram_bot_.Services.Foundations.WordPackages;
using Yodol_telegram_bot_.Services.Foundations.Words;
using Yodol_telegram_bot_.Services.Orchestrations;
using Yodol_telegram_bot_.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog(config =>
{
    config
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File(
            path: "Logs/yodla-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30);
});

AddBrokers(builder.Services);
AddFoundationServices(builder.Services);
AddOrchestrationServices(builder.Services);
AddHostedServices(builder.Services);

var host = builder.Build();
host.Run();

static void AddBrokers(IServiceCollection services)
{
    services.AddTransient<ILoggingBroker, LoggingBroker>();
    services.AddTransient<IDateTimeBroker, DateTimeBroker>();
    services.AddSingleton<IStorageBroker, StorageBroker>();
    services.AddSingleton<ITelegramBroker, TelegramBroker>();
}

static void AddFoundationServices(IServiceCollection services)
{
    services.AddTransient<IUserService, UserService>();
    services.AddTransient<IWordService, WordService>();
    services.AddTransient<IWordPackageService, WordPackageService>();
    services.AddTransient<IReminderService, ReminderService>();
}

static void AddOrchestrationServices(IServiceCollection services)
{
    services.AddTransient<ITelegramOrchestrationService, TelegramOrchestrationService>();
}

static void AddHostedServices(IServiceCollection services)
{
    services.AddHostedService<UpdateHandler>();
    services.AddHostedService<ReminderWorker>();
}
