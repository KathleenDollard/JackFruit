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
              Description = None
              Aliases = []
              Members = []
              SubCommands = []
              Pocket = []} }

    static member ThreeMappings =
        let package =
            { CommandId = "package"
              Path = [ "dotnet"; "add"; "package" ]
              Description = None
              Aliases = [ "package" ]
              Members = 
                [ (MemberDef.Create "other" "int") 
                  (MemberDef.Create "packageName" "string")]
              SubCommands = [] 
              Pocket = []}

        let add =
            { CommandId = "add"
              Path = [ "dotnet"; "add" ]
              Description = None
              Aliases = [ "add" ]
              Members = []
              SubCommands = [ package ]
              Pocket = []}

        { MapInferredStatements =
            [ "builder.MapInferred(\"dotnet <PROJECT>\", DotnetHandlers.Dotnet);"
              "builder.MapInferred(\"dotnet add\", null);"
              "builder.MapInferred(\"dotnet add package <PACKAGE_NAME>\", DotnetHandlers.AddPackage);" ]
          CommandNames = [ "dotnet"; "add"; "package" ]
          CommandDef =
            { CommandId = "dotnet"
              Path = [ "dotnet" ]
              Description = None
              Aliases = [ "dotnet" ]
              Members = [(MemberDef.Create "project" "string")]
              SubCommands = [ add ]
              Pocket = [] } }
