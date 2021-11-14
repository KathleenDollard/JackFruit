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
        let commandDef = CommandDef.Create Arbitrary "A"
        let commandDef = 
            { commandDef with 
                Members = [ MemberDef.Create ArbitraryMember "one" "string"]
                Path = [""]
                Aliases = ["A"] }
        { MapInferredStatements = [ "builder.MapInferred(\"\", Handlers.A);" ]
          CommandNames = [ "" ]
          CommandDefs = [ commandDef ] }
        

    static member ThreeMappings =
        let package =
            { CommandId = "package"
              ReturnType = None
              CommandDefUsage = Arbitrary
              Path = [ "dotnet"; "add"; "package" ]
              Description = None
              Aliases = [ "package" ]
              Members = 
                [ (MemberDef.Create ArbitraryMember "packageName" "string") 
                  (MemberDef.Create ArbitraryMember "version" "string")
                  (MemberDef.Create ArbitraryMember "framework" "string")
                  (MemberDef.Create ArbitraryMember "noRestore" "bool")
                  (MemberDef.Create ArbitraryMember "source" "string")
                  (MemberDef.Create ArbitraryMember "packageDirectory" "string")
                  (MemberDef.Create ArbitraryMember "interactive" "bool")
                  (MemberDef.Create ArbitraryMember "prerelease" "bool")

                  ]
              SubCommands = [] 
              Pocket = []}

        let add =
            { CommandId = "add"
              ReturnType = None
              CommandDefUsage = Arbitrary
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
                CommandDefUsage = Arbitrary
                Path = [ "dotnet" ]
                Description = None
                Aliases = [ "dotnet" ]
                Members = [(MemberDef.Create ArbitraryMember "project" "string")]
                SubCommands = [ add ]
                Pocket = [] }  ] }
