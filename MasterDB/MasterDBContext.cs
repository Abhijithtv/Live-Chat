using MasterDB.Entity;
using MasterDB.ModelBuilders;
using Microsoft.EntityFrameworkCore;

namespace MasterDB
{
    public class MasterDBContext : DbContext
    {
        public DbSet<ChatMessage> ChatMessage { get; set; }
        public DbSet<ChatMessageLog> ChatMessageLog { get; set; }
        public DbSet<GroupChat> GroupChat { get; set; }
        public DbSet<GroupChatMessageLog> GroupChatMessageLog { get; set; }
        public DbSet<GroupChatUser> GroupChatUser { get; set; }
        public DbSet<User> User { get; set; }
        //public DbSet<UserCommunicationsMeta> UserCommunicationsMeta { get; set; }
        public DbSet<ClientMessage> ClientMessage { get; set; } //todo - modal building
        public DbSet<SentEventToQueueMapping> SentEventToQueueMappings { get; set; }

        public MasterDBContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var types = typeof(IMasterDbBuilder).Assembly
                .GetTypes()
                .Where(t => typeof(IMasterDbBuilder).IsAssignableFrom(t)
               && !t.IsInterface
               && !t.IsAbstract);

            foreach (var type in types)
            {
                var instance = (IMasterDbBuilder)Activator.CreateInstance(type)!;
                instance.Build(modelBuilder);
            }
        }
    }
}
