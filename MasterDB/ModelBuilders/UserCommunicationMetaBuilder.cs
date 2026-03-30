using MasterDB.Entity;
using Microsoft.EntityFrameworkCore;

namespace MasterDB.ModelBuilders
{
    public class UserCommunicationMetaBuilder : IMasterDbBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<UserCommunicationsMeta>();

            entity.HasKey(x => new { x.UserId1, x.UserId2 });

            entity.HasOne(x => x.User1)
                .WithMany()
                .HasForeignKey(x => x.UserId1)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.User2)
                .WithMany()
                .HasForeignKey(x => x.UserId2)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(x => x.CurrentSequenceNumber)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(x => x.StartedOnUTC)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(x => x.LastUpdatedOnUTC)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
