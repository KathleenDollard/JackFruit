// KAD-Don: What is the difference between a type being in a namespace and in a module?

namespace Jackfruit.Tests

open Generator.Models

type MapData =
    { MapInferredStatements: string list
      CommandNames: string list
      CommandDefs: CommandDef list }

    static member NoMapping =
        { MapInferredStatements = []
          CommandNames = []
          CommandDefs = [ ] }

    static member OneMapping =
        let commandDef = CommandDef.Create "A"
        let commandDef = {commandDef with Members = [ MemberDef.Create "one" "string"]}
        { MapInferredStatements = [ "builder.MapInferred(\"\", Handlers.A);" ]
          CommandNames = [ "" ]
          CommandDefs = [ commandDef ] }
        

    static member ThreeMappings =
        let package =
            { CommandId = "package"
              ReturnType = None
              GenerateSetHandler = true
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
              ReturnType = None
              GenerateSetHandler = true
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
          CommandDefs =
            [ { CommandId = "dotnet"
                ReturnType = None
                GenerateSetHandler = true
                Path = [ "dotnet" ]
                Description = None
                Aliases = [ "dotnet" ]
                Members = [(MemberDef.Create "project" "string")]
                SubCommands = [ add ]
                Pocket = [] }  ] }
