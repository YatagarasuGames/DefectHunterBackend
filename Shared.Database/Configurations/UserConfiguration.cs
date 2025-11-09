using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Database.Entities;
using Shared.Models;

namespace Shared.Database.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(b => b.Username)
                .IsRequired()
                .HasMaxLength(User.MAX_USERNAME_LENGTH);

            builder.Property(b => b.Password)
                .IsRequired();
                

            builder.Property(b => b.Email)
                .IsRequired();

        }
    }
}
