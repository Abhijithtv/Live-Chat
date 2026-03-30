
using ChatServer.EventHandlers;
using ChatServer.SocketHandler;
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

            builder.Services.AddSingleton<ConnectionManager>();
            builder.Services.AddScoped<UserSentEvent>();

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


            app.MapControllers();

            app.Run();
        }
    }
}
