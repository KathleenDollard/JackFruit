using System;
using System.CommandLine;
using System.CommandLine.Invocation;
namespace GeneratedHandlers
{
    public class ACommandWrapper
    {
        private Action<string> _operation { get; set; }
        private Command _command { get; set; }
        private Argument<string> _oneArgument { get; set; }
        public ACommandWrapper(Action<string> operation)
        {
            _operation = operation;
        }
        public Argument<string> OneArgument
        {
            get
            {
                if (_oneArgument == null)
                {
                    _oneArgument = new Argument<string>("one");
                }
                return _oneArgument;
            }
            set
            {
            }
        }
        public Command Command
        {
            get
            {
                if (_command == null)
                {
                    _command = new Command("A");
                    _command.Add(_oneArgument);
                    _command.Handler = new GeneratedHandler(_operation, OneArgument);
                }
                return _command;
            }
            set
            {
            }
        }
        private class GeneratedHandler : ICommandHandler
        {
            public GeneratedHandler(Action<string> operation, Argument<string> oneArgument)
            {
                _operation = operation;
                _oneArgument = oneArgument;
            }
            private Action<string> _operation { get; set; }
            private Argument<string> _oneArgument { get; set; }
            public void Invoke(InvocationContext context)
            {
                return _operation.Invoke(context.ParseResult.GetValueForOption<string>(_oneArgument));
            }
        }
    }
}
