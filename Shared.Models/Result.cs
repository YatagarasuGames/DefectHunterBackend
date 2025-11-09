namespace Shared.Models
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Error { get; }

        protected Result(T value, bool isSuccess, string error)
        {
            this.Value = value;
            this.IsSuccess = isSuccess;
            this.Error = error;
        }

        public static Result<T> Success(T value) => new Result<T>(value, true, string.Empty);
        public static Result<T> Failure(string error) => new Result<T>(default(T), false, error);
    }

    public class Result
    {
        public bool IsSuccess { get; }
        public string Error;

        protected Result(bool isSuccess, string error)
        {
            this.IsSuccess = isSuccess;
            this.Error = error;
        }
        public static Result Success() => new Result(true, string.Empty);
        public static Result Failure(string error) => new Result(false, error);
    }
}
