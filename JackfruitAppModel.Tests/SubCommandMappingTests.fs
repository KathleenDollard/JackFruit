module Generator.Tests.SubCommandMappingTests


open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open Generator.SubCommandMapping
open Generator.RoslynUtils
open Generator.GeneralUtils
open Jackfruit.Models
open Generator.Tests.UtilsForTests
open Microsoft.CodeAnalysis
open Generator.Tests
open Jackfruit.UtilsForTests
open Generator
open Microsoft.CodeAnalysis.CSharp
open CliApp
open System.Collections.Generic

//NOTE: We are testing against a facsimile of AppBase because using reference assemblies is a PITB
type ``For Command Name Patterns, you can ``() =

    let appBaseCode =
        @"
namespace CliApp
{
    using System.Collections.Generic;

    class AppBase
    {

        public static List<string> DefaultPatterns = new() { ""*"", ""Run *"", ""* Handler"" };
        public static void AddCommandNamePattern(string pattern) { }
        public static void RemoveCommandNamePattern(string pattern) { }
    }
}"

    let SyntaxTreeWithStatements statements =
        let code = 
            @$"
namespace CliApp
{{
    class A
    {{
       void B()
       {{
          {statements}
       }}
    }}
}}"
        CSharpSyntaxTree.ParseText(code)

    let GetPatterns statements = 
        let evalLang = EvalCSharp()
        let tree = SyntaxTreeWithStatements statements
        let appBaseTree = CSharpSyntaxTree.ParseText(appBaseCode)
        let semanticModelResult = GetSemanticModelFromFirstTree [tree; appBaseTree]
        match semanticModelResult with
        | Ok semanticModel ->  GetNamePatterns AppBase.DefaultPatterns evalLang semanticModel
        | Error _ -> invalidOp "Error building test code"

    [<Fact>]
    // KAD-Don: There is an issue here with ambiguity between a 'T and an IEnumerable<'T> overload without the explicit generic
    member _.``Add a pattern``() =
        let actual = GetPatterns @"AppBase.AddCommandNamePattern(""Cmd*"");"
        let expected = AppBase.DefaultPatterns @ ["Cmd*"]
        Assert.Equal<IEnumerable<string>>(expected, actual)

    [<Fact>]
    member _.``Remove a pattern``() =
        let actual = GetPatterns @"AppBase.RemoveCommandNamePattern(""*Handler"");"
        let expected = List.removeAt 2 AppBase.DefaultPatterns
        Assert.Equal<IEnumerable<string>>(expected, actual)

    [<Fact>]
    member _.``Duplicate patterns are removed``() =
        let actual = GetPatterns @"AppBase.AddCommandNamePattern(""*Handler"");"
        let expected = AppBase.DefaultPatterns
        Assert.Equal<IEnumerable<string>>(expected, actual)

    [<Fact>]
    member _.``Add three patterns``() =
        let actual = GetPatterns @"AppBase.AddCommandNamePattern(""Cmd*"");
            AppBase.AddCommandNamePattern(""*Delegate"");
            AppBase.AddCommandNamePattern(""*CmdHandler"");"
        let expected = AppBase.DefaultPatterns @ ["Cmd*"; "*Delegate"; "*CmdHandler"]
        Assert.Equal<IEnumerable<string>>(expected, actual)

    [<Fact>]
    member _.``Remove all patterns``() =
        let actual = GetPatterns @"AppBase.RemoveCommandNamePattern(""*"");
            AppBase.RemoveCommandNamePattern(""Run*"");
            AppBase.RemoveCommandNamePattern(""*Handler"");"
        let expected = []
        Assert.Equal<IEnumerable<string>>(expected, actual)