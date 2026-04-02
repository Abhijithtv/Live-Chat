
using ChatServer.EventHandlers;
using ChatServer.Handlers.SocketHandler;
using ChatServer.Managers;
using ChatServer.Middlewares;
using ChatServer.Queue;
using ChatServer.Services;
using MasterDB;
using Microsoft.EntityFrameworkCore;

namespace ChatServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<MasterDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("ChatMasterDB"));
            });

            //skipping Abstraction to save time - :)
            builder.Services.AddSingleton<ConnectionManager>();
            builder.Services.AddScoped<UserSentEvent>();
            builder.Services.AddScoped<WebSocketProcessor>();
            builder.Services.AddScoped<GroupService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddSingleton<UserSentMsgQueue>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseWebSockets(new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromMinutes(10)
            });

            app.UseMiddleware<ChatWebSocketMiddleware>();
            app.MapControllers();

            app.Run();
        }
    }
}
