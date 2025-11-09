using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class PlayerScore
    {
        public Guid UserId { get; }
        public string Username { get; } = string.Empty;
        public uint Score { get; }

        private PlayerScore(Guid id, string username, uint score)
        {
            UserId = id;
            Username = username;
            Score = score;
        }

        public static Result<PlayerScore> Create(Guid id, string username, uint score)
        {
            var error = string.Empty;
            return Result<PlayerScore>.Success(new PlayerScore(id, username, score));
        }
    }
}
