
                    // list count = 0; 
                    // syntaxTrees count = 6
                    using CliApp;
using DemoHandlers;
using CliDefinition;

namespace CliDefinition
{
    public class Program
    {

        static async Task<int> Main(string[] args)
        {
            return await CliDefinition.StarTrekApp.Run(args);
        }
    }
}



***************************************

using CliApp;
using DemoHandlers;
using System.CommandLine;

namespace CliDefinition
{
    internal partial class StarTrekApp : AppBase
    {
        public static async Task<int> Run(string[] args)
        {
            CreateWithRootCommand(Handlers.StarTrek);
            var cli = StarTrekApp.Create();
            cli.StarTrek.AddSubCommand(Handlers.NextGeneration);
            cli.StarTrek.NextGeneration.AddSubCommand(Handlers.Voyager);

            // Issue: With the following, is it confusing to require using System.CommandLiner
            return await cli.RootCommand.InvokeAsync(args);
        }
    }

}


***************************************

//using System;
//using System.CommandLine;
//using System.CommandLine.Invocation;
//using System.Threading.Tasks;
//using CommandBase;
//using CliApp;
//namespace CliDefinition
//{

//    internal partial class StarTrekApp : AppBase
//    {
//        private StarTrekApp()
//        {
//        }
//        public StarTrekCommand RootCommand { get; set; }
//        public static new StarTrekApp Create()
//        {
//            var newApp = new StarTrekApp();
//            newApp.RootCommand = StarTrekCommand.Create();
//            return newApp;
//        }
//    }

//    public partial class StarTrekCommand : CliRootCommand, ICommandHandler
//    {
//        private StarTrekCommand()
//        {
//        }
//        public static StarTrekCommand Create()
//        {
//            var command = new StarTrekCommand();
//            command.GreetingOption = new Option<string>("greeting");
//            command.Add(command.GreetingOption);
//            command.KirkOption = new Option<bool>("kirk");
//            command.Add(command.KirkOption);
//            command.SpockOption = new Option<bool>("spock");
//            command.Add(command.SpockOption);
//            command.UhuraOption = new Option<bool>("uhura");
//            command.Add(command.UhuraOption);
//            command.NextGeneration = NextGenerationCommand.Create();
//            command.Add(command.NextGeneration);
//            command.Handler = command;
//            return command;
//        }
//        public Option<string> GreetingOption { get; set; }
//        public string GreetingOptionResult(InvocationContext context)
//        {
//            return context.ParseResult.GetValueForOption<string>(GreetingOption);
//        }
//        public Option<bool> KirkOption { get; set; }
//        public bool KirkOptionResult(InvocationContext context)
//        {
//            return context.ParseResult.GetValueForOption<bool>(KirkOption);
//        }
//        public Option<bool> SpockOption { get; set; }
//        public bool SpockOptionResult(InvocationContext context)
//        {
//            return context.ParseResult.GetValueForOption<bool>(SpockOption);
//        }
//        public Option<bool> UhuraOption { get; set; }
//        public bool UhuraOptionResult(InvocationContext context)
//        {
//            return context.ParseResult.GetValueForOption<bool>(UhuraOption);
//        }
//        public NextGenerationCommand NextGeneration { get; set; }
//        public Task<int> InvokeAsync(InvocationContext context)
//        {
//            DemoHandlers.Handlers.StarTrek(GreetingOptionResult(context), KirkOptionResult(context), SpockOptionResult(context), UhuraOptionResult(context));
//            return Task.FromResult(context.ExitCode);
//        }
//    }

//    public partial class NextGenerationCommand : CliCommand, ICommandHandler
//    {
//        private NextGenerationCommand()
//        : base("NextGeneration")
//        {
//        }
//        public static NextGenerationCommand Create()
//        {
//            var command = new NextGenerationCommand();
//            command.GreetingOption = new Option<string>("greeting");
//            command.Add(command.GreetingOption);
//            command.PicardOption = new Option<bool>("picard");
//            command.Add(command.PicardOption);
//            command.Voyager = VoyagerCommand.Create();
//            command.Add(command.Voyager);
//            command.Handler = command;
//            return command;
//        }
//        public Option<string> GreetingOption { get; set; }
//        public string GreetingOptionResult(InvocationContext context)
//        {
//            return context.ParseResult.GetValueForOption<string>(GreetingOption);
//        }
//        public Option<bool> PicardOption { get; set; }
//        public bool PicardOptionResult(InvocationContext context)
//        {
//            return context.ParseResult.GetValueForOption<bool>(PicardOption);
//        }
//        public VoyagerCommand Voyager { get; set; }
//        public Task<int> InvokeAsync(InvocationContext context)
//        {
//            DemoHandlers.Handlers.NextGeneration(GreetingOptionResult(context), PicardOptionResult(context));
//            return Task.FromResult(context.ExitCode);
//        }
//    }

//    public partial class VoyagerCommand : CliCommand, ICommandHandler
//    {
//        private VoyagerCommand()
//        : base("Voyager")
//        {
//        }
//        public static VoyagerCommand Create()
//        {
//            var command = new VoyagerCommand();
//            command.GreetingOption = new Option<string>("greeting");
//            command.Add(command.GreetingOption);
//            command.JanewayOption = new Option<bool>("janeway");
//            command.Add(command.JanewayOption);
//            command.ChakotayOption = new Option<bool>("chakotay");
//            command.Add(command.ChakotayOption);
//            command.TorresOption = new Option<bool>("torres");
//            command.Add(command.TorresOption);
//            command.TuvokOption = new Option<bool>("tuvok");
//            command.Add(command.TuvokOption);
//            command.SevenOfNineOption = new Option<bool>("sevenOfNine");
//            command.Add(command.SevenOfNineOption);
//            command.Handler = command;
//            return command;
//        }
//        public Option<string> GreetingOption { get; set; }
//        public string GreetingOptionResult(InvocationContext context)
//        {
//            return context.ParseResult.GetValueForOption<string>(GreetingOption);
//        }
//        public Option<bool> JanewayOption { get; set; }
//        public bool JanewayOptionResult(InvocationContext context)
//        {
//            return context.ParseResult.GetValueForOption<bool>(JanewayOption);
//        }
//        public Option<bool> ChakotayOption { get; set; }
//        public bool ChakotayOptionResult(InvocationContext context)
//        {
//            return context.ParseResult.GetValueForOption<bool>(ChakotayOption);
//        }
//        public Option<bool> TorresOption { get; set; }
//        public bool TorresOptionResult(InvocationContext context)
//        {
//            return context.ParseResult.GetValueForOption<bool>(TorresOption);
//        }
//        public Option<bool> TuvokOption { get; set; }
//        public bool TuvokOptionResult(InvocationContext context)
//        {
//            return context.ParseResult.GetValueForOption<bool>(TuvokOption);
//        }
//        public Option<bool> SevenOfNineOption { get; set; }
//        public bool SevenOfNineOptionResult(InvocationContext context)
//        {
//            return context.ParseResult.GetValueForOption<bool>(SevenOfNineOption);
//        }
//        public Task<int> InvokeAsync(InvocationContext context)
//        {
//            DemoHandlers.Handlers.Voyager(GreetingOptionResult(context), JanewayOptionResult(context), ChakotayOptionResult(context), TorresOptionResult(context), TuvokOptionResult(context), SevenOfNineOptionResult(context));
//            return Task.FromResult(context.ExitCode);
//        }
//    }
//}


***************************************

// <auto-generated/>
global using global::System;
global using global::System.Collections.Generic;
global using global::System.IO;
global using global::System.Linq;
global using global::System.Net.Http;
global using global::System.Threading;
global using global::System.Threading.Tasks;


***************************************

// <autogenerated />
using System;
using System.Reflection;
[assembly: global::System.Runtime.Versioning.TargetFrameworkAttribute(".NETCoreApp,Version=v6.0", FrameworkDisplayName = "")]


***************************************

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Reflection;

[assembly: System.Reflection.AssemblyCompanyAttribute("Microsoft")]
[assembly: System.Reflection.AssemblyConfigurationAttribute("Debug")]
[assembly: System.Reflection.AssemblyFileVersionAttribute("0.0.1")]
[assembly: System.Reflection.AssemblyInformationalVersionAttribute("0.0.3-alpha")]
[assembly: System.Reflection.AssemblyProductAttribute("Temp1")]
[assembly: System.Reflection.AssemblyTitleAttribute("Temp1")]
[assembly: System.Reflection.AssemblyVersionAttribute("0.0.1")]

// Generated by the MSBuild WriteCodeFragment class.

