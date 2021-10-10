// KAD-Don: What is the difference between a type being in a namespace and in a module?

namespace Generator.Tests.MapData

open Generator.Models


type MapData =
    { MapInferredStatements: string list
      CommandNames: string list
      CommandDef: CommandDef }

    static member NoMapping =
        { MapInferredStatements = []
          CommandNames = []
          CommandDef = CommandDef.CreateRoot }

    static member OneMapping =
        { MapInferredStatements = [ "builder.MapInferred(\"\", Handlers.A);" ]
          CommandNames = [ "" ]
          CommandDef =
            { CommandId = ""
              Path = [ "" ]
              Name = ""
              Description = None
              Aliases = []
              Arg = None
              Options = []
              SubCommands = [] } }

    static member ThreeMappings =
        let package =
            { CommandId = "package"
              Path = [ "dotnet"; "add"; "package" ]
              Name = "package"
              Description = None
              Aliases = []
              Arg = Some(ArgDef.Create "packageName" "string")
              Options = [ (OptionDef.Create "other" "int") ]
              SubCommands = [] }

        let add =
            { CommandId = "add"
              Path = [ "dotnet"; "add" ]
              Name = "add"
              Description = None
              Aliases = []
              Arg = None
              Options = []
              SubCommands = [ package ] }

        { MapInferredStatements =
            [ "builder.MapInferred(\"dotnet <PROJECT>\", DotnetHandlers.Dotnet);"
              "builder.MapInferred(\"dotnet add\", null);"
              "builder.MapInferred(\"dotnet add package <PACKAGE_NAME>\", DotnetHandlers.AddPackage);" ]
          CommandNames = [ "dotnet"; "add"; "package" ]
          CommandDef =
            { CommandId = "dotnet"
              Path = [ "dotnet" ]
              Name = "dotnet"
              Description = None
              Aliases = []
              Arg = Some(ArgDef.Create "project" "string")
              Options = []
              SubCommands = [ add ] } }
