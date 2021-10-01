module Generator.Tests.ArchetypeMappingTests


open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open Generator.ArchetypeMapping
open Generator.RoslynUtils
open Generator.GeneralUtils
open Generator.Models
open Generator.Tests.UtilForTests
open Microsoft.CodeAnalysis

let badMapping = """MapInferredX("", Handlers.X);"""

let AddMapStatements includeBad (statements: string list) =
    AddMapStatementToTestCode [ "var builder = new ConsoleSupport.BuilderInferredParser();";
                                if includeBad then badMapping
                                for s in statements do s ]

let oneMapping = ["""builder.MapInferred("", Handlers.A);"""]
let oneMappingCommandNames = [ "" ]

let noMapping = [ ]
let noMappingCommandNames = []

let threeMappings = ["""builder.MapInferred("dotnet", Handlers.A);"""
                     """builder.MapInferred("dotnet add <PROJECT>", null);"""
                     """builder.MapInferred("dotnet add package <PACKAGE_NAME>", Handlers.B);"""]
let threeMappingsCommandNames = [ "dotnet"; "add"; "package" ]


type ``When parsing archetypes``() =
    [<Fact>]
    member _.``Ancestors found for empty archetype``() =
        let actual = ParseArchetypeInfo "\"\"" None

        actual.AncestorsAndThis |> should equal [""]

    [<Fact>]
    member _.``Ancestors found for simple archetype``() =
        let actual = ParseArchetypeInfo "\"first\"" None

        actual.AncestorsAndThis |> should equal ["first"]

    [<Fact>]
    member _.``Ancestors found for multi-level archetype``() =
        let expectedCommands = [ "first"; "second"; "third"]
        let actual = ParseArchetypeInfo "\"first second third\"" None

        actual.AncestorsAndThis |> should equal expectedCommands


type ``When creating archetypeInfo from mapping``() =
    let CommandNamesFromSource source =
        let commandNames archInfoList =
            [ for archInfo in archInfoList do
                archInfo.AncestorsAndThis |> List.last ]

        let result = 
            ModelFrom (CSharpCode source) (CSharpCode HandlerSource)
            |> Result.map (InvocationsFromModel "MapInferred")
            |> Result.bind ArchetypeInfoListFrom 
            |> Result.map commandNames

        match result with 
        | Ok n -> n
        | Error err -> invalidOp $"Test failed with {err}"


    [<Fact>]
    member _.``None are found when there are none``() =
        let source = AddMapStatements false noMapping

        let actual = CommandNamesFromSource source

        actual |> should matchList noMappingCommandNames

    [<Fact>]
    member _.``One is found when there is one``() =
        let source = AddMapStatements false oneMapping

        let actual = CommandNamesFromSource source

        actual |> should matchList oneMappingCommandNames

    [<Fact>]
    member _.``Multiples are found when there are multiple``() =
        let source = AddMapStatements false threeMappings

        let actual = CommandNamesFromSource source

        actual |> should matchList threeMappingsCommandNames



 type ``When creating commandDefs from handlers``() =
    let archetypesAndModelFromSource source =
        let source = AddMapStatements false source
        let mutable model:SemanticModel option = None

        // KAD: Any better way to catch an interim value in a pipeline
        let updateModel newModel = 
            model <- Some newModel
            newModel

        let result = 
            ModelFrom (CSharpCode source) (CSharpCode HandlerSource)
            |> Result.map updateModel
            |> Result.map (InvocationsFromModel "MapInferred")
            |> Result.bind ArchetypeInfoListFrom

        match result with
        | Ok archetypeList -> (archetypeList, model.Value)
        | Error err -> invalidOp $"Test failed building archetypes from source {err}"


    [<Fact>]
    member _.``Handler name is found as method in separate class``() =
        let (archetypes, model) = archetypesAndModelFromSource oneMapping
        let archetypeInfo = archetypes |> List.exactlyOne

        let actual = 
            match archetypeInfo.HandlerExpression with 
            | Some handler -> MethodFromHandler model handler
            | None -> invalidOp "Test failed because no handler found"

        actual |> should not' (be Null)

        actual.ToString() |> should haveSubstring "Handlers.A"


    [<Fact>]
    member _.``Parameters retrieved from Handler``() =
        let (archetypes, model) = archetypesAndModelFromSource oneMapping
        let expected = [("one", "string")]

        let parameters = ParametersFromArchetype archetypes[0] model
        let actual = 
            [ for tuple in parameters do
                match tuple with 
                | (name, t) -> (name, t.ToString())]

        actual |> should matchList expected



    [<Fact>]
    member _.``Tree is built with ArchetypeInfoTreeFrom``() =
        let source = AddMapStatements false threeMappings
        let result = 
            SyntaxTreeResult (CSharpCode source)
            |> Result.map (InvocationsFrom "MapInferred")
            |> Result.bind ArchetypeInfoListFrom
            |> Result.map ArchetypeInfoTreeFrom

        let actual = 
            match result with 
            | Ok tree -> tree
            | Error err -> invalidOp $"Failed to build tree {err}" // TODO: Work on error reporting

        actual[0].Data.AncestorsAndThis |> should equal ["dotnet"]
        actual[0].Children[0].Data.AncestorsAndThis |> should equal ["dotnet"; "add"]
        actual[0].Children[0].Children[0].Data.AncestorsAndThis |> should equal ["dotnet";"add"; "package"]


    [<Fact>]
    member _.``CommandDef built from ArchetypeInfo with Handler``() =
        let (archetypes, model) = archetypesAndModelFromSource oneMapping
        let archetypeInfo = archetypes |> List.exactlyOne

        let actual = 
            match archetypeInfo.HandlerExpression with 
            | Some handler -> MethodFromHandler model handler
            | None -> invalidOp "Test failed because no handler found"

        actual |> should not' (be Null)

        actual.ToString()
        |> should haveSubstring "Handlers.A"

    //[<Fact>]
    //member _.``Option and Argument types are updated on command``() =
    //    let (archetypes, model) = archetypesAndModelFromSource oneMapping
    //    let archetypeInfo = archetypes |> List.exactlyOne 
    //    let methodSymbolResult = MethodFromHandler model archetypeInfo.HandlerExpression

    //    let actual = 
    //        match methodSymbolResult with 
    //        | Some methodSymbol -> copyUpdateArchetypeInfoFromSymbol archetypeInfo methodSymbol
    //        | None -> invalidOp "Method symbol not found during arrange"

    //    actual.Archetype.Arg |> shouldBeNone
    //    actual.Archetype.Options |> should haveLength 1
    //    let firstOption = actual.Archetype.Options.[0]
    //    firstOption |> should equal {OptionName="one"; TypeName=Some "string"}
