using Microsoft.EntityFrameworkCore;
using Shared.Database.Entities;

namespace Shared.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<PlayerScoreEntity> PlayerScores { get; set; }
        public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    }
}
