module Generator.Tests.AddCommandMappingTests

open Xunit
open Jackfruit
open Jackfruit.Models
open Generator.Tests.UtilsForTests
open Generator
open Microsoft.CodeAnalysis.CSharp
open CliApp
open System.Collections.Generic
open AddCommandMappingTestUtils
open FsUnit.Xunit
open FsUnit.CustomMatchers
open Jackfruit.AddCommandMapping
open Generator.Tests
open Jackfruit.UtilsForTests
open Jackfruit.Tests


//NOTE: We are testing against a facsimile of AppBase because using reference assemblies is a PITB
type ``For Command Name Patterns, you can ``() =

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

//type ``For command definitons, you can``() =
//    let GetHandler methodName statements = 
//        let evalLang = EvalCSharp()
//        let tree = SyntaxTreeWithStatements statements
//        let appBaseTree = CSharpSyntaxTree.ParseText(appBaseCode)
//        let semanticModelResult = GetSemanticModelFromFirstTree [tree; appBaseTree]
//        match semanticModelResult with
//        | Ok semanticModel ->  GetPathAndHandler methodName evalLang semanticModel
//        | Error _ -> invalidOp "Error building test code"

//    [<Fact>]
//    member _.``Extract the handler for a command``() =
//        let actualWithHandlers = GetHandler "AddCommand" @"app.AddCommand(ClassB.Hndlr);"
//        let actual = 
//            [ for pair in actualWithHandlers do
//                match pair with 
//                | (path, _) -> path ]
//        let expected = [""]

//        Assert.Equal<IEnumerable<string>>(expected, actual)

//    [<Fact>]
//    member _.``Extract the handler for a subcommand``() =
//        let actualWithHandlers = GetHandler "AddSubCommand" @"app.Spock.AddSubCommand(ClassB.Hndlr);"
//        let actual = 
//            [ for pair in actualWithHandlers do
//                match pair with 
//                | (path, _) -> path ]
//        let expected = ["Spock"]

//        Assert.Equal<IEnumerable<string>>(expected, actual)
 

 type ``When parsing addCommands``() =
     [<Fact>]
     static member internal ``Ancestors found for empty addCommand``() =
         let actual = ParseAddCommandInfo "\"\"" None
         let expected = 
            { Path = [""]
              Handler = None }

         Assert.Equal(expected, actual)
 
     [<Fact>]
     member _.``Ancestors found for simple addCommand``() =
         let actual = ParseAddCommandInfo "\"app.first\"" None
         let expected = 
            { Path = ["first"]
              Handler = None }
 
         Assert.Equal(expected, actual)

     [<Fact>]
     member _.``Ancestors found for multi-level addCommand``() =
         let actual = ParseAddCommandInfo "\"app.first.second.third\"" None
         let expected = 
            { Path = [ "first"; "second"; "third"]
              Handler = None }
 
         Assert.Equal(expected, actual)

 type ``When creating addCommandInfo from mapping``() =
    let eval = EvalCSharp()
 
    let CommandNamesFromSource source =
        let getCommandNames (archInfoList: AddCommandInfo list) =
            [ for archInfo in archInfoList do
                archInfo.Path |> List.last ]
 
        let result = 
            ModelFrom [(CSharpCode source); (CSharpCode HandlerSource)]
            |> Result.bind (eval.InvocationsFromModel "MapInferred")
            |> Result.bind (AddCommandInfoListFrom eval)
            |> Result.map getCommandNames
 
        match result with 
        | Ok n ->  n
        | Error err -> invalidOp $"Test failed with {err}"
     
 
    [<Fact>]
    member _.``None are found when there are none``() =
        let source = MapData.NoMapping.CliCode
 
        let expected = MapData.NoMapping.CommandNames
        let actual = CommandNamesFromSource source 
        Assert.Fail("Not yet updated")
        actual |> should matchList MapData.NoMapping.CommandNames
 
    [<Fact>]
    member _.``One is found when there is one``() =
        let source = MapData.OneMapping.CliCode
 
        let actual = CommandNamesFromSource source
        Assert.Fail("Not yet updated")
        actual |> should matchList MapData.OneMapping.CommandNames
 
    [<Fact>]
    member _.``Multiples are found when there are multiple``() =
        let source = MapData.ThreeMappings.CliCode
 
        let actual = CommandNamesFromSource source
        Assert.Fail("Not yet updated")

        actual |> should matchList MapData.ThreeMappings.CommandNames