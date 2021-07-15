namespace ConsoleSupport
{
    public class Results
    {

        public static Results Ok<T>(T value)
        {
            return new Results<T>(value);
        }

        public static Results NotFound()
        {
            return new Results();
        }

    }

    public class Results<T> : Results
    {
        public Results(T value)
        {
            Value = value;
        }
        public T Value { get; }
    }
}
