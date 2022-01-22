module Jackfruit.Tests.MapData

open Generator.Models
open Generator.Tests.UtilsForTests
open System
open Common
open Generator.LanguageModel

type  Data =
    { MapInferredStatements: string
      HandlerClassName: string
      HandlerMethods: string
      CommandNames: string list
      CommandDefs: CommandDef list }
    member this.CliCode =
        let handlerClassName = if String.IsNullOrWhiteSpace(this.HandlerClassName) then "Handlers" else this.HandlerClassName
        $@"
        using System;

        public class Builder
        {{
            public static void MapInferred(string archetype, Delegate handler) {{}}
        }}

        public class Cli
        {{
            public void Definition()
            {{ {this.MapInferredStatements} }}
        }}

        public class {handlerClassName}
        {{
            { this.HandlerMethods }
        }}"

let NoMapping =
    { MapInferredStatements = ""
      HandlerClassName = ""
      HandlerMethods = ""
      CommandNames = []
      CommandDefs = [ ] }

let OneMapping =
    let commandDef = CommandDef("A", [""], ReturnType.ReturnTypeVoid, Arbitrary "MyCommand")
    let members = [ MemberDef("one",commandDef, (SimpleNamedItem "string"), ArbitraryMember, true) ]
    commandDef.Members <- members

    { MapInferredStatements = "Builder.MapInferred(\"\", Handlers.A);"
      HandlerClassName = "Handlers"
      HandlerMethods = "public static void A(string one) {}"
      CommandNames = [ "" ]
      CommandDefs = [ commandDef ] }
        

let ThreeMappings =
    let package = CommandDef("package", [ "dotnet"; "add"; "package" ], ReturnType.ReturnTypeVoid, Arbitrary "MyCommand")
    package.Members <-
        [ MemberDef("packageName", package, (SimpleNamedItem "string"), ArbitraryMember, true)
          MemberDef("version", package, (SimpleNamedItem "string"), ArbitraryMember, true)
          MemberDef("framework",  package, (SimpleNamedItem "string"), ArbitraryMember, true)
          MemberDef("noRestore",  package, (SimpleNamedItem "bool"), ArbitraryMember, true)
          MemberDef("source",  package, (SimpleNamedItem "string"), ArbitraryMember, true)
          MemberDef("packageDirectory",  package, (SimpleNamedItem "string"), ArbitraryMember, true)
          MemberDef("interactive",  package, (SimpleNamedItem "bool"), ArbitraryMember, true)
          MemberDef("prerelease",  package, (SimpleNamedItem "bool"), ArbitraryMember, true) ] 
   
    let add = CommandDef("add", [ "dotnet"; "add" ], ReturnType.ReturnTypeVoid, Arbitrary "MyCommand")
    add.SubCommands <- [package]

    let dotnet = CommandDef("dotnet", [ "dotnet" ], ReturnType.ReturnTypeVoid, Arbitrary "MyCommand")
    dotnet.Members <-
        [ MemberDef("project", dotnet, (SimpleNamedItem "string"), ArbitraryMember, true) ]
    dotnet.SubCommands <- [add]

    { MapInferredStatements =
        @"
        Builder.MapInferred(""dotnet <PROJECT>"", DotnetHandlers.Dotnet);
        Builder.MapInferred(""dotnet add"", null);
        Builder.MapInferred(""dotnet add package <PACKAGE_NAME> --source|-s"", DotnetHandlers.AddPackage);"
      HandlerClassName = "DotnetHandlers"
      HandlerMethods = 
        @"
         public static void Dotnet(string project) { }
         public static void AddPackage(string packageName, string version, string framework, bool noRestore, string source, 
                string packageDirectory, bool interactive, bool prerelease) { }"
      CommandNames = [ "dotnet"; "add"; "package" ]

      CommandDefs = [ dotnet ] }