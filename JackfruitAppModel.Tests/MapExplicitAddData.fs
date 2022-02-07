﻿module Generator.Tests.MapExplicitAddData

open Generator.Models
open Generator.Tests.UtilsForTests
open System
open Common
open Generator.LanguageModel
open Microsoft.CodeAnalysis.CSharp

let AppBaseCode =
    @"
    public class AppBase
    {

        public static List<string> DefaultPatterns = new() { ""*"", ""Run *"", ""* Handler"" };
        public static void AddCommandNamePattern(string pattern) { }
        public static void RemoveCommandNamePattern(string pattern) { }
        public void AddCommand(Delegate handler) { }
        public void AddSubCommand(Delegate handler) { }
    }"

let SyntaxTreeWithStatements statements =
    let code = 
        @$"

using System;
using System.Collections.Generic;

namespace CliApp
{{
    public class A
    {{
        public void B()
        {{
            var app = new CliApp.MyCli();
            {statements}
        }}
    }}

    public class ClassB
    {{
        
        public static void Hndlr(int p1, string pStr) {{ }}
    }}
}}"
    CSharpSyntaxTree.ParseText(code)

let OriginalSeriesPseudoGenerated = $"
    public class OriginalSeries : AppBase
    {{
        public NextGeneration NextGeneration {{ get; set; }}
        public AppBase Kirk {{ get; set; }}
        public AppBase Spock {{ get; set; }}
        public AppBase Uhura {{ get; set; }}
    }}
    "

let NextGenerationPseudoGenerated = $"
    public class NextGeneration : AppBase
    {{
        public Voyager Voyager {{ get; set; }}
        public DeepSpaceNine DeepSpaceNine {{ get; set; }}
        public AppBase Picard {{ get; set; }}
    }}
    "

// TODO: Update this to not use AppBase
let DeepSpaceNinePseudoGenerated = $"
    public class DeepSpaceNine : AppBase
    {{
        public AppBase Sisko {{ get; set; }}
        public AppBase Odo {{ get; set; }}
        public AppBase Dax {{ get; set; }}
        public AppBase Worf {{ get; set; }}
        public AppBase OBrien {{ get; set; }}
    }}
    "

let VoyagerPseudoGenerated = $"
    public class Voyager : AppBase
    {{
        public AppBase Janeway {{ get; set; }}
        public AppBase Chakotay {{ get; set; }}
        public AppBase Torres {{ get; set; }}
        public AppBase Tuvok {{ get; set; }}
        public AppBase SevenOfNine {{ get; set; }}
    }}
    "

let Handlers = $"
    public static class Handlers
    {{
        public static void OriginalSeries(string kirk, string spock, string uhura) {{ }}
        public static void NextGeneration(string picard) {{ }}
        public static void DeepSpaceNine(string sisko, string odo, string dax, string worf, string oBrien) {{ }}
        public static void Voyager(string janeway, string chakotay, string torres, bool tuvok, string sevenOfNine) {{ }}
    }}"

let CliWrapperCode statements = $@"
using System;
using System.Collections.Generic;
namespace CliApp
{{

   {AppBaseCode}

   {OriginalSeriesPseudoGenerated}
   {NextGenerationPseudoGenerated}
   {DeepSpaceNinePseudoGenerated}
   {VoyagerPseudoGenerated}

   {Handlers}

   public class MyCli : AppBase
   {{
       public OriginalSeries OriginalSeries {{ get; set; }}
   }}

   public class Program
   {{
       {statements} 
   }}
}}"


type Data =
    { ExplicitAddStatements: string
      CommandNames: string list
      CommandDefs: CommandDef list }
    // Common code is repeated to avoid reference assemblies. 
    // Pseudo-generated code is included so we can test
    member this.CliCode = CliWrapperCode this.ExplicitAddStatements

let NoMapping =
    { ExplicitAddStatements = ""
      CommandNames = []
      CommandDefs = [ ] }

let OneMapping =
    let mutable commandDef = CommandDef("NextGeneration", ["NextGeneration"], ReturnType.ReturnTypeVoid, Arbitrary "NextGeneration")
    let members = [ MemberDef("picard",commandDef, (SimpleNamedItem "string"), ArbitraryMember, true) ]
    commandDef.Members <- members

    { ExplicitAddStatements = @$"
    public void DefineCli ()
    {{ 
        var app = new MyCli();
        app.AddCommand(Handlers.NextGeneration);
    }}"
      CommandNames = [ "NextGeneration" ]
      CommandDefs = [ commandDef ] }

let ThreeMappings =
    // KAD-Don-Chet: Why don't I need mutable here?
    let voyager = CommandDef("Voyager", [ "OriginalSeries"; "NextGeneration"; "Voyager" ], ReturnType.ReturnTypeVoid, Arbitrary "Voyager")
    voyager.Members <-
        [ MemberDef("janeway", voyager, (SimpleNamedItem "string"), ArbitraryMember, true)
          MemberDef("chakotay", voyager, (SimpleNamedItem "string"), ArbitraryMember, true)
          MemberDef("torres",  voyager, (SimpleNamedItem "string"), ArbitraryMember, true)
          MemberDef("tuvok",  voyager, (SimpleNamedItem "bool"), ArbitraryMember, true)
          MemberDef("sevenOfNine",  voyager, (SimpleNamedItem "string"), ArbitraryMember, true) ]
   
    //let deepSpaceNine = CommandDef("nextGeneration", [ "originalSeries"; "nextGeneration"; "deepSpaceNine" ], ReturnType.ReturnTypeVoid, Arbitrary "DeepSpaceNine")
    //deepSpaceNine.Members <-
    //    [ MemberDef("sisko", deepSpaceNine, (SimpleNamedItem "string"), ArbitraryMember, true)
    //      MemberDef("odo", deepSpaceNine, (SimpleNamedItem "string"), ArbitraryMember, true)
    //      MemberDef("dax",  deepSpaceNine, (SimpleNamedItem "string"), ArbitraryMember, true)
    //      MemberDef("worf",  deepSpaceNine, (SimpleNamedItem "bool"), ArbitraryMember, true)
    //      MemberDef("oBrien",  deepSpaceNine, (SimpleNamedItem "string"), ArbitraryMember, true) ]

    let nextGeneration = CommandDef("NextGeneration", [ "OriginalSeries"; "NextGeneration" ], ReturnType.ReturnTypeVoid, Arbitrary "DeepSpaceNine")
    nextGeneration.Members <-
        [ MemberDef("picard", nextGeneration, (SimpleNamedItem "string"), ArbitraryMember, true) ]
    nextGeneration.SubCommands <- [voyager]

    let deepSpaceNine = CommandDef("DeepSpaceNine", [ "OriginalSeries"; "NextGeneration"; "DeepSpaceNine"], ReturnType.ReturnTypeVoid, Arbitrary "DeepSpaceNine")
    deepSpaceNine.Members <-
        [ MemberDef("sisko", nextGeneration, (SimpleNamedItem "string"), ArbitraryMember, true)
          MemberDef("odo", nextGeneration, (SimpleNamedItem "string"), ArbitraryMember, true)
          MemberDef("dax",  nextGeneration, (SimpleNamedItem "string"), ArbitraryMember, true)
          MemberDef("worf",  nextGeneration, (SimpleNamedItem "bool"), ArbitraryMember, true)
          MemberDef("oBrien",  nextGeneration, (SimpleNamedItem "string"), ArbitraryMember, true) ]
    deepSpaceNine.SubCommands <- []

    let originalSeries = CommandDef("OriginalSeries", [ "OriginalSeries" ], ReturnType.ReturnTypeVoid, Arbitrary "OriginalSeries")
    originalSeries.Members <-
        [ MemberDef("kirk", originalSeries, (SimpleNamedItem "string"), ArbitraryMember, true)
          MemberDef("spock", originalSeries, (SimpleNamedItem "string"), ArbitraryMember, true)
          MemberDef("uhura",  originalSeries, (SimpleNamedItem "string"), ArbitraryMember, true) ]
    originalSeries.SubCommands <- [nextGeneration]

    { ExplicitAddStatements =
        @"
    public void DefineCli ()
    {{ 
        var app = new MyCli();
        app.AddCommand(Handlers.OriginalSeries);
        app.OriginalSeries.AddSubCommand(Handlers.NextGeneration);
        app.OriginalSeries.NextGeneration.AddSubCommand(Handlers.Voyager);
    }}"
      CommandNames = [ "OriginalSeries"; "NextGeneration"; "Voyager" ]

      CommandDefs = [ originalSeries ] }