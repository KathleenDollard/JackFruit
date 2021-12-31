namespace Jackfruit.Tests

open Generator.Models
open Common

type MapData =
    { MapInferredStatements: string list
      HandlerStatements: string list
      CommandNames: string list
      CommandDefs: CommandDef list }

    static member NoMapping =
        { MapInferredStatements = []
          HandlerStatements = []
          CommandNames = []
          CommandDefs = [ ] }

    static member OneMapping =
        let commandDef = CommandDef("A", [""], Void, Arbitrary "MyCommand")
        let members = [ MemberDef("one",commandDef, (SimpleNamedItem "string"), ArbitraryMember, true) ]
        commandDef.Members <- members

        { MapInferredStatements = [ "builder.MapInferred(\"\", Handlers.A);" ]
          HandlerStatements = [ "public static void A(string one) {}" ]
          CommandNames = [ "" ]
          CommandDefs = [ commandDef ] }
        

    static member ThreeMappings =
        let package = CommandDef("package", [ "dotnet"; "add"; "package" ], Void, Arbitrary "MyCommand")
        package.Members <-
            [ MemberDef("packageName", package, (SimpleNamedItem "string"), ArbitraryMember, true)
              MemberDef("version", package, (SimpleNamedItem "string"), ArbitraryMember, true)
              MemberDef("framework",  package, (SimpleNamedItem "string"), ArbitraryMember, true)
              MemberDef("noRestore",  package, (SimpleNamedItem "bool"), ArbitraryMember, true)
              MemberDef("source",  package, (SimpleNamedItem "string"), ArbitraryMember, true)
              MemberDef("packageDirectory",  package, (SimpleNamedItem "string"), ArbitraryMember, true)
              MemberDef("interactive",  package, (SimpleNamedItem "bool"), ArbitraryMember, true)
              MemberDef("prerelease",  package, (SimpleNamedItem "bool"), ArbitraryMember, true) ] 
   
        let add = CommandDef("add", [ "dotnet"; "add" ], Void, Arbitrary "MyCommand")
        add.SubCommands <- [package]

        let dotnet = CommandDef("dotnet", [ "dotnet" ], Void, Arbitrary "MyCommand")
        dotnet.Members <-
            [ MemberDef("project", dotnet, (SimpleNamedItem "string"), ArbitraryMember, true) ]
        dotnet.SubCommands <- [add]

        { MapInferredStatements =
            [ "builder.MapInferred(\"dotnet <PROJECT>\", DotnetHandlers.Dotnet);"
              "builder.MapInferred(\"dotnet add\", null);"
              "builder.MapInferred(\"dotnet add package <PACKAGE_NAME> --source|-s\", DotnetHandlers.AddPackage);" ]
          HandlerStatements = 
              [ "public static void A() { }"
                "public static void B() { }"
                "public static void C() { }" ]
          CommandNames = [ "dotnet"; "add"; "package" ]

          CommandDefs = [ dotnet ] }