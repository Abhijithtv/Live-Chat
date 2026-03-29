using MasterDB.Entity;
using Microsoft.EntityFrameworkCore;

namespace MasterDB.ModelBuilders
{
    public class ChatMessageBuilder : IMasterDbBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<ChatMessage>();

            entity.HasKey(x => x.MessageId);

            entity.Property(x => x.SequenceNumber)
                .IsRequired();

            entity.Property(x => x.Message)
                .HasMaxLength(256);

            entity.Property(x => x.Status)
                .HasMaxLength(50);
        }
    }
}
