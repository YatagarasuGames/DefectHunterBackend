using System.ComponentModel.DataAnnotations;

namespace Shared.Database.Entities
{
    public class PlayerScoreEntity
    {
        [Key] public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public uint Score { get; set; }
    }
}
