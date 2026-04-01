using Yodol_telegram_bot_;
using Yodol_telegram_bot_.Options;
using Yodol_telegram_bot_.Services.WordService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<BotOptions>(builder.Configuration.GetSection(BotOptions.SectionName));
builder.Services.AddSingleton<IWordService, WordService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
