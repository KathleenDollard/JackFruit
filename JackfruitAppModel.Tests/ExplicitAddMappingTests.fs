module Generator.Tests.ExplicitAddMappingTests

open Xunit
//open Jackfruit
//open Jackfruit.Models
open Generator.Tests.UtilsForTests
open Generator
open Microsoft.CodeAnalysis.CSharp
open CliApp
open System.Collections.Generic
open FsUnit.Xunit
open FsUnit.CustomMatchers
//open Generator.Tests.MapExplicitAddData
open Generator.Tests
open Jackfruit.UtilsForTests
//open Jackfruit.Tests
open Generator.Tests.MapExplicitAddData
open Generator.ExplicitAdd.ExplicitAddMapping
open Microsoft.CodeAnalysis
open Common
open Generator.ExplicitAdd


//NOTE: We are testing against a facsimile of AppBase because using reference assemblies is a PITB
type ``For Command Name Patterns, you can ``() =

    let GetPatterns statements = 
        let evalLang = EvalCSharp()
        let statements = 
            $@"
        public void DefineCli ()
        {{ 
            var app = new MyCli();
            {statements};
        }}" 
        let source = CliWrapperCode statements
        let tree = CSharpSyntaxTree.ParseText(source)
        let semanticModelResult = GetSemanticModelFromFirstTree [tree]
        match semanticModelResult with
        | Ok semanticModel ->  GetNamePatterns AppBase.DefaultPatterns evalLang semanticModel
        | Error err -> invalidOp $"Error building test code: {err}"

    [<Fact>]
    // KAD: MAKE AN ISSUE: There is an issue here with ambiguity between a 'T and an IEnumerable<'T> overload without the explicit generic
    member _.``Add a pattern``() =
        let actual = GetPatterns @"AppBase.AddCommandNamePattern(""Cmd*"");"
        let expected = AppBase.DefaultPatterns @ ["Cmd*"]
        Assert.Equal<IEnumerable<string>>(expected, actual)
        //Assert.Equal(expected, actual)

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

type ``When parsing ExplicitAdds``() =
     [<Fact>]
     static member internal ``Ancestors found for empty ExplicitAdd``() =
         let actual = ParseExplicitAddInfo "\"\"" None "current"
         let expected = 
            { ExplicitAddInfo.Path = ["current"]
              Handler = None }

         Assert.Equal(expected, actual)
 
     [<Fact>]
     member _.``Ancestors found for simple ExplicitAdd``() =
         let actual = ParseExplicitAddInfo "\"app.first\"" None "current"
         let expected = 
            { Path = ["first"; "current"]
              Handler = None }
 
         Assert.Equal(expected, actual)

     [<Fact>]
     member _.``Ancestors found for multi-level ExplicitAdd``() =
         let actual = ParseExplicitAddInfo "\"app.first.second.third\"" None "current"
         let expected = 
            { Path = [ "first"; "second"; "third"; "current"]
              Handler = None }
 
         Assert.Equal(expected, actual)

type ``When creating ExplicitAddInfo from mapping``() =
    let eval = EvalCSharp()
 
    let CommandNamesFromSource source =
        let getCommandNames (archInfoList: ExplicitAddInfo list) =
            [ for archInfo in archInfoList do
                archInfo.Path |> List.last ]

        //let commandNamesFromModel semanticModel=
        //    GetPathAndHandler ["CreateWithRootCommand"] eval semanticModel
        //    |> Result.bind (mergeWith (GetPathAndHandler ["AddSubCommand"] eval semanticModel))
        //    |> Result.bind (ExplicitAddInfoListFrom eval)
        //    |> Result.map getCommandNames

        let commandNamesFromModel semanticModel=
            eval.InvocationsFromModel ["CreateWithRootCommand"] semanticModel
            |> Result.bind (MergeWith (eval.InvocationsFromModel ["AddSubCommand"] semanticModel))
            |> Result.bind (ExplicitAddInfoListFrom eval semanticModel)
            |> Result.map getCommandNames

        let result = 
            match ModelFrom [(CSharpCode source)] with
            | Ok semanticModel -> Ok (commandNamesFromModel semanticModel)
            | Error err -> Error err
            
 
        match result with 
        | Ok n ->  n
        | Error err -> invalidOp $"Test failed with {err}"
     
 
    [<Fact>]
    member _.``None are found when there are none``() =
        let source = MapExplicitAddData.NoMapping.CliCode
 
        let expected = MapExplicitAddData.NoMapping.CommandNames
        let actual = 
            match CommandNamesFromSource source with
            | Ok a -> a
            | Error err -> invalidOp (err.ToString())
        Assert.Equal<IEnumerable<string>>(MapExplicitAddData.NoMapping.CommandNames, actual)
 
    [<Fact>]
    member _.``One is found when there is one``() =
        let source = MapExplicitAddData.OneMapping.CliCode
        let actual = CommandNamesFromSource source
        let actual = 
            match CommandNamesFromSource source with
            | Ok a -> a
            | Error err -> invalidOp (err.ToString())
        Assert.Equal<IEnumerable<string>>(MapExplicitAddData.OneMapping.CommandNames, actual) 

    [<Fact>]
    member _.``Multiples are found when there are multiple``() =
        let source = MapExplicitAddData.ThreeMappings.CliCode
        let actual = CommandNamesFromSource source
        let actual = 
            match CommandNamesFromSource source with
            | Ok a -> a
            | Error err -> invalidOp (err.ToString())
        Assert.Equal<IEnumerable<string>>(MapExplicitAddData.ThreeMappings.CommandNames, actual)

type ``For command definitons, you can``() =
    let evalLang = EvalCSharp()

    let TestPath source methodName expected =
        let semanticModel = GetSemanticModel source
        let invocationsResult = eval.InvocationsFromModel methodName semanticModel
        let invocations =
            match invocationsResult with
            | Ok inv -> inv
            | Error err -> invalidOp "Error finding invocations"
        let infoList = GetCommandInfoList evalLang semanticModel invocations
        let actual = 
            [ for info in infoList do 
                info.Path]
        // KAD-Chet: I did not find an easkir  way to do the following
        let zip = List.zip actual expected
        for pair in zip do
            match pair with 
            | act, exp -> Assert.Equal<IEnumerable<string>>(exp, act)
    
    let TestHandler source methodName expected =
        let semanticModel = GetSemanticModel source
        let invocationsResult = eval.InvocationsFromModel methodName semanticModel
        let invocations =
            match invocationsResult with
            | Ok inv -> inv
            | Error err -> invalidOp "Error finding invocations"
        let infoList = GetCommandInfoList evalLang semanticModel invocations
        let actual = 
            [ for info in infoList do 
                match info.Handler with
                | Some handler -> MethodSymbolFromMethodCall semanticModel handler
                | None -> Assert.Fail("Handler missing")]
        let zip = List.zip actual expected
        for pair in zip do
            match pair with
            | Some act, exp -> Assert.Equal(exp, act.Name)
            | None, _ -> Assert.Fail("Handler missing")

    [<Fact>]
    member _.``Extract the path members for a command``() =
        TestPath OneMapping.CliCode ["CreateWithRootCommand"] [ ["NextGeneration"] ]

    [<Fact>]
    member _.``Extract the path members for a subcommand``() =
        TestPath ThreeMappings.CliCode ["AddSubCommand"] 
            [ ["OriginalSeries"; "NextGeneration"] 
              ["OriginalSeries"; "NextGeneration"; "Voyager"]]

    [<Fact>]
    member _.``Extract the handler for a command``() =
        TestHandler OneMapping.CliCode ["CreateWithRootCommand"] [ "NextGeneration"]


    [<Fact>]
    member _.``Extract the handler for a subcommand``() =
        TestHandler ThreeMappings.CliCode ["AddSubCommand"] [ "NextGeneration"; "Voyager" ]
    
type ToCompare = 
    { Path: string list
      HandlerName: string }

type ``When building CommandDefs from explicit AddCommands, you can``() =
    let evalLang = EvalCSharp()

    let InputTree semanticModel methodName =
        let invocationsResult = eval.InvocationsFromModel methodName semanticModel
        let invocations =
            match invocationsResult with
            | Ok inv -> inv
            | Error err -> invalidOp "Error finding invocations"
        let infoList = GetCommandInfoList evalLang semanticModel invocations
        let result = ExplicitAddInfoTreeFrom infoList
        match result with
        | Ok tree -> tree
        | Error err -> invalidOp $"Failure creating tree: {err}"

    let GetHandlerName semanticModel (info: SyntaxNode option) =    
        match info with
        | Some handler -> 
            match MethodSymbolFromMethodCall semanticModel handler with 
            | Some method -> method.Name
            | None -> invalidOp "Handler method missing"
        | None -> invalidOp "Handler missing"
         

    let CompareWithTree semanticModel (expected: TreeNodeType<ToCompare>) (actual: TreeNodeType<ExplicitAddInfo>) =
        let rec recurse depth exp (act: TreeNodeType<ExplicitAddInfo>) =
            if depth > 20 then invalidOp "Possible runaway recursion"
            Assert.Equal<IEnumerable<string>>(exp.Data.Path, act.Data.Path)
            Assert.Equal<string>(exp.Data.HandlerName, (GetHandlerName semanticModel act.Data.Handler))
            let zip = List.zip exp.Children act.Children
            for (e, a) in zip do
                recurse (depth + 1) e a

        recurse 0 expected actual

    let TestTree methodName source expected =
        let semanticModel = GetSemanticModel source
        let trees = InputTree semanticModel methodName
        for tree in trees do
            CompareWithTree semanticModel expected tree
        trees

    [<Fact>]
    member _.``Build the input tree for one command``() =
        let expected =
            { Data = { Path = [ "NextGeneration"]; HandlerName = "NextGeneration" }
              Children = [] }
        let trees = TestTree ["CreateWithRootCommand"] OneMapping.CliCode expected
        Assert.NotNull(trees[0].Data.Handler)

    [<Fact>]
    member _.``Build the input tree for two sub-commands``() =
        let expected =
            { Data = { Path = [ "OriginalSeries"]; HandlerName = "OriginalSeries" }
              Children = 
                [ { Data = { Path = [ "OriginalSeries"; "NextGeneration"]; HandlerName = "NextGeneration" }
                    Children = 
                        [ { Data = { Path = [ "OriginalSeries"; "NextGeneration"; "Voyager"]; HandlerName = "Voyager" }
                            Children = [] }] } ] }
        TestTree ["CreateWithRootCommand"; "AddSubCommand"] ThreeMappings.CliCode expected