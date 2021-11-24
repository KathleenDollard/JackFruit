using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CommandLine;

namespace GeneratedHandlers
{
    public class ACommandWrapper
    {
        private Func<string, void> _operation { get; set; }
        private Command _command { get; set; }
        private Argument<string> _oneArgument { get; set; }
        public ACommandWrapper(Func<string, void> operation)
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
                    _command.Handler = new GeneratedHandler(_method, OneArgument);
                }
                return _command;
            }
            set
            {
            }
        }
        private class GeneratedHandler
        {
            public GeneratedHandler(Func<string, void> operation, Argument<string> oneArgument)
            {
                _operation = operation;
                _oneArgument = oneArgument;
            }
            private Func<string, void> _operation { get; set; }
            private Argument<string> _oneArgument { get; set; }
            public void Invoke(InvocationContext context)
            {
                return operation.Invoke(context.ParseResult.GetValueForOption<string>(_oneArgument));
            }
        }
    }
}

