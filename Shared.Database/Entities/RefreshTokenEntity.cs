using System.ComponentModel.DataAnnotations;

namespace Shared.Database.Entities
{
    public class RefreshTokenEntity
    {
        [Key] public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpireTime { get; set; }
        public Guid UserId { get; set; }

    }
}
