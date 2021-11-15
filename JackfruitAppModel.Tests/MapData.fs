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
        let members = [ MemberDef("one", "string", ArbitraryMember, true) ]
        let commandDef = CommandDef("A", [""], None, Arbitrary, members, [])

        { MapInferredStatements = [ "builder.MapInferred(\"\", Handlers.A);" ]
          CommandNames = [ "" ]
          CommandDefs = [ commandDef ] }
        

    static member ThreeMappings =
        let packageMembers = 
            [ MemberDef("packageName", "string", ArbitraryMember, true)
              MemberDef("version", "string", ArbitraryMember, true)
              MemberDef("framework", "string", ArbitraryMember, true)
              MemberDef("noRestore", "bool", ArbitraryMember, true)
              MemberDef("source", "string", ArbitraryMember, true)
              MemberDef("packageDirectory", "string", ArbitraryMember, true)
              MemberDef("interactive", "bool", ArbitraryMember, true)
              MemberDef("prerelease", "bool", ArbitraryMember, true) ] 
        let package = CommandDef("package", [ "dotnet"; "add"; "package" ], None, Arbitrary, packageMembers, [])
   
        let add = CommandDef("add", [ "dotnet"; "add" ], None, Arbitrary, [], [package])

        let dotnetMembers = [ MemberDef("project", "string", ArbitraryMember, true) ]
        let dotnet = CommandDef("dotnet", [ "dotnet" ], None, Arbitrary, dotnetMembers, [add])

        { MapInferredStatements =
            [ "builder.MapInferred(\"dotnet <PROJECT>\", DotnetHandlers.Dotnet);"
              "builder.MapInferred(\"dotnet add\", null);"
              "builder.MapInferred(\"dotnet add package <PACKAGE_NAME>\", DotnetHandlers.AddPackage);" ]
          CommandNames = [ "dotnet"; "add"; "package" ]

          CommandDefs = [ dotnet ] }