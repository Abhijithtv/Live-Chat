using ChatOpsAzFunction.ChatEventHandlers;
using ChatOpsAzFunction.ChatSentEventHandlers.Implementations;
using ChatOpsAzFunction.Services;
using MasterDB;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddDbContext<MasterDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChatMasterDB"));
});

builder.Services
    .AddScoped<UserSentEventProcessor>()
    .AddScoped<ChatSentEventHandlerFactory>()
    .AddScoped<GroupChatSentEventHandler>()
    .AddSingleton<ChatServerClient>();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
