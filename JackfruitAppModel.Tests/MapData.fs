// KAD-Don: What is the difference between a type being in a namespace and in a module?

namespace Jackfruit.Tests

open Generator.Models
open Common

type MapData =
    { MapInferredStatements: string list
      CommandNames: string list
      CommandDefs: CommandDef list }

    static member NoMapping =
        { MapInferredStatements = []
          CommandNames = []
          CommandDefs = [ ] }

    static member OneMapping =
        let commandDef = CommandDef("A", [""], Void, Arbitrary)
        let members = [ MemberDef("one",commandDef, (SimpleNamedItem "string"), ArbitraryMember, true) ]
        commandDef.Members <- members

        { MapInferredStatements = [ "builder.MapInferred(\"\", Handlers.A);" ]
          CommandNames = [ "" ]
          CommandDefs = [ commandDef ] }
        

    static member ThreeMappings =
        let package = CommandDef("package", [ "dotnet"; "add"; "package" ], Void, Arbitrary)
        package.Members <-
            [ MemberDef("packageName", package, (SimpleNamedItem "string"), ArbitraryMember, true)
              MemberDef("version", package, (SimpleNamedItem "string"), ArbitraryMember, true)
              MemberDef("framework",  package, (SimpleNamedItem "string"), ArbitraryMember, true)
              MemberDef("noRestore",  package, (SimpleNamedItem "bool"), ArbitraryMember, true)
              MemberDef("source",  package, (SimpleNamedItem "string"), ArbitraryMember, true)
              MemberDef("packageDirectory",  package, (SimpleNamedItem "string"), ArbitraryMember, true)
              MemberDef("interactive",  package, (SimpleNamedItem "bool"), ArbitraryMember, true)
              MemberDef("prerelease",  package, (SimpleNamedItem "bool"), ArbitraryMember, true) ] 
   
        let add = CommandDef("add", [ "dotnet"; "add" ], Void, Arbitrary )
        add.SubCommands <- [package]

        let dotnet = CommandDef("dotnet", [ "dotnet" ], Void, Arbitrary)
        dotnet.Members <-
            [ MemberDef("project", dotnet, (SimpleNamedItem "string"), ArbitraryMember, true) ]
        dotnet.SubCommands <- [add]

        { MapInferredStatements =
            [ "builder.MapInferred(\"dotnet <PROJECT>\", DotnetHandlers.Dotnet);"
              "builder.MapInferred(\"dotnet add\", null);"
              "builder.MapInferred(\"dotnet add package <PACKAGE_NAME> --source|-s\", DotnetHandlers.AddPackage);" ]
          CommandNames = [ "dotnet"; "add"; "package" ]

          CommandDefs = [ dotnet ] }