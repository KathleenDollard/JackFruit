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
open Generator.Tests.TestData


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
