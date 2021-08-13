namespace ConsoleSupport
{
    public class Results
    {
        public string? Message { get; }

        public Results(string? message)
            => Message = message;

        public static Results Ok<T>(T value)
            => new ResultsOk<T>(value);

        public static Results NotFound(string? message = null)
            => new ResultsError(message ?? "Not found");

        public static Results Error(string? message = null)
            => new ResultsError(message ?? "Unknown error");

    }

    public class ResultsOk<T> : Results
    {
        public ResultsOk(T value)
            : base(null)
            => Value = value;

        public T Value { get; }
    }
    public class ResultsError : Results
    {
        public ResultsError(string message)
            : base(message) { }
    }
}
