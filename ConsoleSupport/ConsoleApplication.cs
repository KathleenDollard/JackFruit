using System.Linq.Expressions;

namespace ConsoleSupport
{
    public class ConsoleApplication
    {
        private Command? rootCommand;
        private readonly Dictionary<string, string> descriptions = new();

        public static ConsoleBuilder CreateBuilder()
            => new ConsoleBuilder();

        public Command RootCommand
                => rootCommand ??= new RootCommand();

        public Command MapInferred(string def, Delegate del)
            => BuilderInferredParser.MapInferred(def,RootCommand, del);

        public Command Map(string def,
                       Delegate del)
           => BuilderParser.Map(def, RootCommand, del);

        public Command Map(string def,
                        Expression<Action> handler)
            => BuilderParser.Map(def, RootCommand, handler);

        public Command Map<TRet>(string def,
                                 Expression<Func<TRet>> handler)
            => BuilderParser.Map(def, RootCommand, handler);

        public Command Map<T1, TRet>(string def,
                   Expression<Func<T1, TRet>> handler)
            => BuilderParser.Map(def, RootCommand, handler);

        public Command Map<T1, T2, TRet>(string def,
                     Expression<Func<T1, T2, TRet>> handler)
            => BuilderParser.Map(def, RootCommand, handler);

        public Command Map<T1, T2, T3,TRet>(string def,
             Expression<Func<T1, T2, T3,TRet>> handler)   
            => BuilderParser.Map(def, RootCommand, handler);

        public Command Map<T1, T2, T3, T4,TRet>(string def,
             Expression<Func<T1, T2, T3, T4,TRet>> handler)
            => BuilderParser.Map(def, RootCommand, handler);


        public Command Map<T1, T2, T3, T4,T5, TRet>(string def,
             Expression<Func<T1, T2, T3, T4, T5,TRet>> handler)
            => BuilderParser.Map(def, RootCommand, handler);

        public Command Map<T1, T2, T3, T4, T5,T6, TRet>(string def,
             Expression<Func<T1, T2, T3, T4, T5, T6,TRet>> handler)
            => BuilderParser.Map(def, RootCommand, handler);

        public Command Map<T1, T2, T3, T4, T5, T6, T7,TRet>(string def,
             Expression<Func<T1, T2, T3, T4, T5, T6, T7,TRet>> handler)
            => BuilderParser.Map(def, RootCommand, handler);

        public Command Map<T1, T2, T3, T4, T5, T6, T7, T8,TRet>(string def,
             Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8,TRet>> handler)
            => BuilderParser.Map(def, RootCommand, handler);

        public Command Map<T1, T2, T3, T4, T5, T6, T7, T8,T9, TRet>(string def,
             Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8,T9, TRet>> handler)
            => BuilderParser.Map(def, RootCommand, handler);


        public void UseExceptionHandler(string? path = null)
        {
        }

        public int Run(string[] args)
           => (rootCommand ?? throw new InvalidOperationException("No CLI commands defined"))
              .Invoke(args);

        public int Run(string? args)
           => (rootCommand ?? throw new InvalidOperationException("No CLI commands defined"))
              .Invoke(args ?? "");

        public int RunInLoop(string[]? args = null, string? message = null)
        {
            var ret = 0;
            if (args is not null)
            {
                ret = Run(args);
            }
            message ??= "Experiment with your CLI! Ctl-Z or Ctl-C to exit.";
            while (true)
            {
                Console.WriteLine(message);
                var input = Console.ReadLine();
                if (input is null)
                {
                    break;
                }
                ret = Run(input);
            }
            return ret;
        }

        public void Throw<T>(T exception)
            where T : Exception
            => throw exception;

        public void AddDescriptions(Dictionary<string, string> descriptions)
            => this.descriptions.AddRange(descriptions);
    }
}
