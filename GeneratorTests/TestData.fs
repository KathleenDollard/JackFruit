module Generator.Tests.TestData

open Generator.Models

type CommandDef with
    static member Empty =
        { CommandId = ""
          Path = []
          Name = ""
          Description = None
          Aliases = []
          Arg = None
          Options = []
          SubCommands = [] }

type ArgDef with
    static member Empty =
        { ArgId = ""
          Name = ""
          Description = None
          Required = None
          TypeName = "" }

type OptionDef with
    static member Empty =
        { OptionId = ""
          Name = ""
          Description = None
          Aliases = []
          Required = None
          TypeName = "" }

type testData =
    { MapInferredStatements: string list
      CommandNames: string list
      CommandDef: CommandDef }

let BadMappingStatement = "builder.MapInferredX(\"\", Handlers.X);"

let BadMapping = 
    { MapInferredStatements = 
        [];
      CommandNames = [BadMappingStatement]
      CommandDef = CommandDef.Empty }

let NoMapping = 
    { MapInferredStatements = 
        [];
      CommandNames = []
      CommandDef = CommandDef.Empty }

let OneMapping = 
    { MapInferredStatements = 
        ["builder.MapInferred(\"\", Handlers.A);"];
      CommandNames = [""]
      CommandDef = 
        { CommandId = ""
          Path = [""]
          Name = ""
          Description = None
          Aliases = []
          Arg = None
          Options = []
          SubCommands = [] }}

let ThreeMappings = 
    let package =
        { CommandId = "package"
          Path = ["dotnet"; "add"; "package"]
          Name = "package"
          Description = None
          Aliases = []
          Arg = Some { ArgDef.Empty with ArgId="packageName"; Name ="packageName"; TypeName = "string"}
          Options = []
          SubCommands = [] }
    let add =
        { CommandId = "add"
          Path = ["dotnet"; "add"]
          Name = "add"
          Description = None
          Aliases = []
          Arg = Some { ArgDef.Empty with ArgId="project"; Name ="project"; TypeName = "string"}
          Options = [{ OptionDef.Empty with OptionId="other"; Name ="other"; TypeName = "int"}]
          SubCommands = [package] }
    { MapInferredStatements = 
        [ "builder.MapInferred(\"dotnet\", DotnetHandlers.Dotnet);"
          "builder.MapInferred(\"dotnet add <PROJECT>\", null);"
          "builder.MapInferred(\"dotnet add package <PACKAGE_NAME>\", DotnetHandlers.AddPackage);" ];
      CommandNames = ["dotnet"; "add"; "package"]
      CommandDef = 
        { CommandId = "dotnet"
          Path = ["dotnet"]
          Name = "dotnet"
          Description = None
          Aliases = []
          Arg = None
          Options = []
          SubCommands = [add] }}



