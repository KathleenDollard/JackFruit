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
        let commandDef = CommandDef("A", [""], (Some "void"), Arbitrary)
        let members = [ MemberDef("one",commandDef, "string", ArbitraryMember, true) ]
        commandDef.Members <- members

        { MapInferredStatements = [ "builder.MapInferred(\"\", Handlers.A);" ]
          CommandNames = [ "" ]
          CommandDefs = [ commandDef ] }
        

    static member ThreeMappings =
        let package = CommandDef("package", [ "dotnet"; "add"; "package" ], Some "void", Arbitrary)
        package.Members <-
            [ MemberDef("packageName", package, "string", ArbitraryMember, true)
              MemberDef("version", package, "string", ArbitraryMember, true)
              MemberDef("framework",  package, "string", ArbitraryMember, true)
              MemberDef("noRestore",  package, "bool", ArbitraryMember, true)
              MemberDef("source",  package, "string", ArbitraryMember, true)
              MemberDef("packageDirectory",  package, "string", ArbitraryMember, true)
              MemberDef("interactive",  package, "bool", ArbitraryMember, true)
              MemberDef("prerelease",  package, "bool", ArbitraryMember, true) ] 
   
        let add = CommandDef("add", [ "dotnet"; "add" ], None, Arbitrary )
        add.SubCommands <- [package]

        let dotnet = CommandDef("dotnet", [ "dotnet" ], Some "void", Arbitrary)
        dotnet.Members <-
            [ MemberDef("project", dotnet, "string", ArbitraryMember, true) ]
        dotnet.SubCommands <- [add]

        { MapInferredStatements =
            [ "builder.MapInferred(\"dotnet <PROJECT>\", DotnetHandlers.Dotnet);"
              "builder.MapInferred(\"dotnet add\", null);"
              "builder.MapInferred(\"dotnet add package <PACKAGE_NAME> --source|-s\", DotnetHandlers.AddPackage);" ]
          CommandNames = [ "dotnet"; "add"; "package" ]

          CommandDefs = [ dotnet ] }