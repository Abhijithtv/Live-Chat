using MasterDB.Entity;
using Microsoft.EntityFrameworkCore;

namespace MasterDB.ModelBuilders
{
    public class UserBuilder : IMasterDbBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<User>();

            entity.HasKey(x => x.UserId);

            entity.Property(x => x.UserName)
                .HasMaxLength(256)
                .IsRequired();

            entity.Property(x => x.UserEmail)
                .HasMaxLength(256)
                .IsRequired();
        }
    }
}
