using MasterDB.Entity;
using Microsoft.EntityFrameworkCore;

namespace MasterDB.ModelBuilders
{
    public class SentEventToQueueMappingBuilder : IMasterDbBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SentEventToQueueMapping>()
                .HasKey(x => x.ClientTransactionId);

            modelBuilder.Entity<SentEventToQueueMapping>()
                .Property(x => x.AzureMessageId)
                .HasMaxLength(256);

            modelBuilder.Entity<SentEventToQueueMapping>()
                .Property(x => x.AzureCorrelationId)
                .HasMaxLength(256);

            modelBuilder.Entity<SentEventToQueueMapping>()
                .Property(x => x.ProcessCount)
                .HasDefaultValue(0);
        }
    }
}
