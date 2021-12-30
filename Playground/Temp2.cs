using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
namespace CliDefinition
{
    public class ACli
    {
        public Command ACommand()
        {
            var command = new Command("A");
            var oneOption = new Option<string>("one");
            command.Add(oneOption);
            // In the following, the hard coded handler name is wrong, but I am just getting code structure correct
            command.SetHandler(MyCommand, oneOption);
            return command;
        }

        public void MyCommand(Option oneOption) { }
    }
}