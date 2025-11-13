using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Zammad.Sdk.Core;
using Zammad.Sdk.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(logging => logging.AddConsole());
builder.Services.AddZammadClient(options =>
{
    options.BaseUri = new Uri(builder.Configuration["Zammad:Url"] ?? "https://zammad.example.com/");
    options.Token = builder.Configuration["Zammad:Token"] ?? "token-placeholder";
    options.Cache = new CacheOptions { Enabled = true, Duration = TimeSpan.FromMinutes(2) };
});
builder.Services.AddZammadWebhooks(options =>
{
    options.Path = "/webhooks/zammad";
    options.Secret = builder.Configuration["Zammad:WebhookSecret"] ?? "change-me";
});
builder.Services.AddZammadPolling(TimeSpan.FromMinutes(1));
builder.Services.AddHostedService<SampleWorker>();

var host = builder.Build();
await host.RunAsync();
