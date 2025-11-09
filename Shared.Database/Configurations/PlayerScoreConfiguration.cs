using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Database.Entities;

namespace Shared.Database.Configurations
{
    public class PlayerScoreConfiguration : IEntityTypeConfiguration<PlayerScoreEntity>
    {
        public void Configure(EntityTypeBuilder<PlayerScoreEntity> builder)
        {
            builder.HasKey(p => p.UserId);

            builder.Property(p => p.Username)
                .HasMaxLength(Shared.Models.User.MAX_USERNAME_LENGTH)
                .IsRequired();

            builder.Property(p => p.Score)
                .IsRequired();
        }
    }
}
