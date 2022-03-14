using System; // Count = 2;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using CommandBase;
using Generator.ConsoleSupport;
namespace CliDefinition
{
   
   internal partial class StarTrekApp : ConsoleApplication
   {
      private StarTrekApp(StarTrekCommand rootCommand)
      : base(rootCommand)
      {
         StarTrek = rootCommand;
      }
      public StarTrekCommand StarTrek {get; set;}
      public static new StarTrekApp Create()
      {
         var newApp = new StarTrekApp(StarTrekCommand.Create());
         return newApp;
      }
   }
   
   public partial class StarTrekCommand : CliRootCommand, ICommandHandler
   {
      private StarTrekCommand()
      {
      }
      public static StarTrekCommand Create()
      {
         var command = new StarTrekCommand();
         command.GreetingOption = new Option<string>("greeting");
         command.Add(command.GreetingOption);
         command.KirkOption = new Option<bool>("kirk");
         command.Add(command.KirkOption);
         command.SpockOption = new Option<bool>("spock");
         command.Add(command.SpockOption);
         command.UhuraOption = new Option<bool>("uhura");
         command.Add(command.UhuraOption);
         command.Handler = command;
         return command;
      }
      public Option<string> GreetingOption {get; set;}
      public string GreetingOptionResult(InvocationContext context)
      {
         return context.ParseResult.GetValueForOption<string>(GreetingOption);
      }
      public Option<bool> KirkOption {get; set;}
      public bool KirkOptionResult(InvocationContext context)
      {
         return context.ParseResult.GetValueForOption<bool>(KirkOption);
      }
      public Option<bool> SpockOption {get; set;}
      public bool SpockOptionResult(InvocationContext context)
      {
         return context.ParseResult.GetValueForOption<bool>(SpockOption);
      }
      public Option<bool> UhuraOption {get; set;}
      public bool UhuraOptionResult(InvocationContext context)
      {
         return context.ParseResult.GetValueForOption<bool>(UhuraOption);
      }
      public Task<int> InvokeAsync(InvocationContext context)
      {
         DemoHandlers.Handlers.StarTrek(GreetingOptionResult(context), KirkOptionResult(context), SpockOptionResult(context), UhuraOptionResult(context));
         return Task.FromResult(context.ExitCode);
      }
   }
   
   public partial class NextGenerationCommand : CliRootCommand, ICommandHandler
   {
      private NextGenerationCommand()
      {
      }
      public static NextGenerationCommand Create()
      {
         var command = new NextGenerationCommand();
         command.GreetingOption = new Option<string>("greeting");
         command.Add(command.GreetingOption);
         command.PicardOption = new Option<bool>("picard");
         command.Add(command.PicardOption);
         command.Voyager = VoyagerCommand.Create();
         command.Add(command.Voyager);
         command.DeepSpaceNine = DeepSpaceNineCommand.Create();
         command.Add(command.DeepSpaceNine);
         command.Handler = command;
         return command;
      }
      public Option<string> GreetingOption {get; set;}
      public string GreetingOptionResult(InvocationContext context)
      {
         return context.ParseResult.GetValueForOption<string>(GreetingOption);
      }
      public Option<bool> PicardOption {get; set;}
      public bool PicardOptionResult(InvocationContext context)
      {
         return context.ParseResult.GetValueForOption<bool>(PicardOption);
      }
      public VoyagerCommand Voyager {get; set;}
      public DeepSpaceNineCommand DeepSpaceNine {get; set;}
      public Task<int> InvokeAsync(InvocationContext context)
      {
         DemoHandlers.Handlers.NextGeneration(GreetingOptionResult(context), PicardOptionResult(context));
         return Task.FromResult(context.ExitCode);
      }
   }
   
   public partial class VoyagerCommand : CliCommand, ICommandHandler
   {
      private VoyagerCommand()
      : base("Voyager")
      {
      }
      public static VoyagerCommand Create()
      {
         var command = new VoyagerCommand();
         command.GreetingOption = new Option<string>("greeting");
         command.Add(command.GreetingOption);
         command.JanewayOption = new Option<bool>("janeway");
         command.Add(command.JanewayOption);
         command.ChakotayOption = new Option<bool>("chakotay");
         command.Add(command.ChakotayOption);
         command.TorresOption = new Option<bool>("torres");
         command.Add(command.TorresOption);
         command.TuvokOption = new Option<bool>("tuvok");
         command.Add(command.TuvokOption);
         command.SevenOfNineOption = new Option<bool>("sevenOfNine");
         command.Add(command.SevenOfNineOption);
         command.Handler = command;
         return command;
      }
      public Option<string> GreetingOption {get; set;}
      public string GreetingOptionResult(InvocationContext context)
      {
         return context.ParseResult.GetValueForOption<string>(GreetingOption);
      }
      public Option<bool> JanewayOption {get; set;}
      public bool JanewayOptionResult(InvocationContext context)
      {
         return context.ParseResult.GetValueForOption<bool>(JanewayOption);
      }
      public Option<bool> ChakotayOption {get; set;}
      public bool ChakotayOptionResult(InvocationContext context)
      {
         return context.ParseResult.GetValueForOption<bool>(ChakotayOption);
      }
      public Option<bool> TorresOption {get; set;}
      public bool TorresOptionResult(InvocationContext context)
      {
         return context.ParseResult.GetValueForOption<bool>(TorresOption);
      }
      public Option<bool> TuvokOption {get; set;}
      public bool TuvokOptionResult(InvocationContext context)
      {
         return context.ParseResult.GetValueForOption<bool>(TuvokOption);
      }
      public Option<bool> SevenOfNineOption {get; set;}
      public bool SevenOfNineOptionResult(InvocationContext context)
      {
         return context.ParseResult.GetValueForOption<bool>(SevenOfNineOption);
      }
      public Task<int> InvokeAsync(InvocationContext context)
      {
         DemoHandlers.Handlers.Voyager(GreetingOptionResult(context), JanewayOptionResult(context), ChakotayOptionResult(context), TorresOptionResult(context), TuvokOptionResult(context), SevenOfNineOptionResult(context));
         return Task.FromResult(context.ExitCode);
      }
   }
   
   public partial class DeepSpaceNineCommand : CliCommand, ICommandHandler
   {
      private DeepSpaceNineCommand()
      : base("DeepSpaceNine")
      {
      }
      public static DeepSpaceNineCommand Create()
      {
         var command = new DeepSpaceNineCommand();
         command.GreetingOption = new Option<string>("greeting");
         command.Add(command.GreetingOption);
         command.SiskoOption = new Option<bool>("sisko");
         command.Add(command.SiskoOption);
         command.OdoOption = new Option<bool>("odo");
         command.Add(command.OdoOption);
         command.DaxOption = new Option<bool>("dax");
         command.Add(command.DaxOption);
         command.WorfOption = new Option<bool>("worf");
         command.Add(command.WorfOption);
         command.OBrienOption = new Option<bool>("oBrien");
         command.Add(command.OBrienOption);
         command.Handler = command;
         return command;
      }
      public Option<string> GreetingOption {get; set;}
      public string GreetingOptionResult(InvocationContext context)
      {
         return context.ParseResult.GetValueForOption<string>(GreetingOption);
      }
      public Option<bool> SiskoOption {get; set;}
      public bool SiskoOptionResult(InvocationContext context)
      {
         return context.ParseResult.GetValueForOption<bool>(SiskoOption);
      }
      public Option<bool> OdoOption {get; set;}
      public bool OdoOptionResult(InvocationContext context)
      {
         return context.ParseResult.GetValueForOption<bool>(OdoOption);
      }
      public Option<bool> DaxOption {get; set;}
      public bool DaxOptionResult(InvocationContext context)
      {
         return context.ParseResult.GetValueForOption<bool>(DaxOption);
      }
      public Option<bool> WorfOption {get; set;}
      public bool WorfOptionResult(InvocationContext context)
      {
         return context.ParseResult.GetValueForOption<bool>(WorfOption);
      }
      public Option<bool> OBrienOption {get; set;}
      public bool OBrienOptionResult(InvocationContext context)
      {
         return context.ParseResult.GetValueForOption<bool>(OBrienOption);
      }
      public Task<int> InvokeAsync(InvocationContext context)
      {
         DemoHandlers.Handlers.DeepSpaceNine(GreetingOptionResult(context), SiskoOptionResult(context), OdoOptionResult(context), DaxOptionResult(context), WorfOptionResult(context), OBrienOptionResult(context));
         return Task.FromResult(context.ExitCode);
      }
   }
}
