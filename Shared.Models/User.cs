namespace Shared.Models
{
    public class User
    {
        public const int MAX_USERNAME_LENGTH = 15;
        public const int MIN_USERNAME_LENGTH = 3;
        public const int MIN_PASSWORD_LENGTH = 8;
        public Guid Id { get; }
        public string Username { get; } = string.Empty;
        public string Email { get; } = string.Empty;
        public string Password { get; } = string.Empty;

        private User(Guid id, string username, string email, string password)
        {
            Id = id;
            Username = username;
            Email = email;
            Password = password;
        }

        public static Result<User> Create(Guid id, string username, string email, string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(username))
                errors.Add("Username is required");

            else if (username.Length < MIN_USERNAME_LENGTH)
                errors.Add($"Username must be at least {MIN_USERNAME_LENGTH} characters long");

            else if (username.Length > MAX_USERNAME_LENGTH)
                errors.Add($"Username cannot exceed {MAX_USERNAME_LENGTH} characters");

            if (string.IsNullOrWhiteSpace(email))
                errors.Add("Email is required");

            else if (!IsValidEmail(email))
                errors.Add("Email format is invalid");

            if (string.IsNullOrWhiteSpace(password))
                errors.Add("Password is required");

            else if (password.Length < MIN_PASSWORD_LENGTH)
                errors.Add($"Password must be at least {MIN_PASSWORD_LENGTH} characters long");

            if (errors.Any())
                return Result<User>.Failure(string.Join("; ", errors));

            return Result<User>.Success(new User(id, username, email.Trim().ToLower(), password));
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }
}
