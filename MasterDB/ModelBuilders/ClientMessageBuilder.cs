using MasterDB.Entity;
using Microsoft.EntityFrameworkCore;

namespace MasterDB.ModelBuilders
{
    public class ClientMessageBuilder : IMasterDbBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<ClientMessage>();

            entity.HasKey(x => new { x.TransactionId, x.SenderId });

            entity.Property(x => x.ChatMessageId)
                .IsRequired(false);

            entity.HasOne(x => x.ChatMessage)
                .WithOne(x => x.ClientMessage)
                .HasForeignKey<ClientMessage>(x => x.ChatMessageId)
                .HasPrincipalKey<ChatMessage>(x => x.MessageId);

            entity.Property(x => x.SenderId)
                .HasMaxLength(256);
        }
    }
}
