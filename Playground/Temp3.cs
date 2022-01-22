using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
namespace GeneratedHandlers
{
    public class ACommandWrapper2 : ICommandHandler
    {
        private Action<string> _operation { get; set; }
        public Command Command { get;  }
        public Argument<string> OneArgument { get;  }
        public ACommandWrapper2(Action<string> operation)
        {
            _operation = operation; 
            Command = new Command("A");
            OneArgument = new Argument<string>("one");
            Command.Add(OneArgument);
            Command.Handler = this;
        }

        public string OneArgumentResult(InvocationContext context)
            => context.ParseResult.GetValueForArgument<string>(OneArgument);

        public Task<int> InvokeAsync(InvocationContext context)
        {
            _operation(OneArgumentResult(context));
            return Task.FromResult(context.ExitCode);
        }
    }
}
