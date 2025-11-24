using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class RefreshToken
    {
        public Guid Id { get; }
        public string Token { get; }
        public DateTime ExpireTime { get; }
        public Guid UserId { get; }

        private RefreshToken(Guid id, string token, DateTime expireTime, Guid userId)
        {
            Id = id;
            Token = token;
            ExpireTime = expireTime;
            UserId = userId;
        }

        public static Result<RefreshToken> Create(Guid id, string token, DateTime expireTime, Guid userId)
        {
            string error = string.Empty;
            return Result<RefreshToken>.Success(new RefreshToken(id, token, expireTime, userId));
        }
    }
}
