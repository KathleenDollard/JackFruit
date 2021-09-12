module Tests

open Xunit
open FsUnit.Xunit
open GeneratorSketch.Generator
open Microsoft.CodeAnalysis.CSharp
open System.Linq
open TestUtils
open CSharpTestCode
open Microsoft.CodeAnalysis

let badMapping = """MapInferredX("", Handlers.X);"""

let addMapStatements includeBad (statements: string list) =
    addMapStatementToTestCode [ "var builder = new ConsoleSupport.BuilderInferredParser();";
                                if includeBad then badMapping
                                for s in statements do s ]

let oneMapping = ["""builder.MapInferred("", Handlers.A);"""]
let oneMappingCommandNames = [ "" ]

let noMapping = [ ]
let noMappingCommandNames = []

let threeMappings = ["""builder.MapInferred("", Handlers.A);"""
                     """builder.MapInferred("add <PROJECT>", null);"""
                     """builder.MapInferred("add package <PACKAGE_NAME>", Handlers.B);"""]
let threeMappingsCommandNames = [ ""; "add"; "package" ]

type ``When parsing archetypes``() =

    [<Fact>]
    member _.``CommandDef is created from a empty archetype``() =
        let actual = parseArchetype "\"\""

        actual.CommandName |> should equal ""

    [<Fact>]
    member _.``CommandDef is created from a simple archetype``() =
        let actual = parseArchetype "\"first\""

        actual.CommandName |> should equal "first"

    [<Fact>]
    member _.``Parents are parsed``() =
        let expectedParentCommands = [ "first"; "second" ]
        let expectedCommandName = "third"

        let actual = parseArchetype "\"first second third\""

        actual.CommandName
        |> should equal expectedCommandName

        actual.ParentCommandNames
        |> should matchList expectedParentCommands

    [<Fact>]
    member _.``Argument is parsed``() =
        let actual = parseArchetype "\"<arg1>\""

        let expected =
            Some { ArgName = "arg1"; TypeName = None }

        actual.Arg |> shouldEqual expected

    [<Fact>]
    member _.``Option is parsed``() =
        let actual = parseArchetype "\"--opt1\""

        let expected =
            [ { OptionName = "opt1"; TypeName = None } ]

        actual.Options |> should matchList expected

    [<Fact>]
    member _.``Complex archetype is parsed``() =
        let expectedParentCommands = [ "first"; "second" ]
        let expectedCommandName = "third"
        let expectedArg = { ArgName = "arg1"; TypeName = None }

        let expectedOptions =
            [ { OptionName = "opt1"; TypeName = None }
              { OptionName = "opt2"; TypeName = None } ]

        let parents = String.concat " " expectedParentCommands

        let optionNames =
            String.concat
                ""
                [ for o in expectedOptions do
                      " --" + o.OptionName ]

        let archetype =
            $"{parents} {expectedCommandName} <{expectedArg.ArgName}> {optionNames}"

        let actual = parseArchetype archetype

        actual.CommandName
        |> should equal expectedCommandName

        actual.ParentCommandNames
        |> should matchList expectedParentCommands

        actual.Options |> should matchList expectedOptions


type ``When creating archetypeInfo from mapping``() =
    [<Fact>]
    member _.``None are found when there are none``() =
        let source = addMapStatements true noMapping

        let actual = archetypeInfoFrom (Code source)
        let actualNames =CommandNamesFromArchetypeInfo actual

        actualNames |> should matchList noMappingCommandNames

    [<Fact>]
    member _.``One is found when there is one``() =
        let source = addMapStatements true oneMapping

        let actual = archetypeInfoFrom (Code source)
        let actualNames =CommandNamesFromArchetypeInfo actual

        actualNames |> should matchList oneMappingCommandNames

    [<Fact>]
    member _.``Multiples are found when there are multiple``() =
        let source = addMapStatements true threeMappings

        let actual = archetypeInfoFrom (Code source)
        let actualNames =CommandNamesFromArchetypeInfo actual

        actualNames
        |> should matchList threeMappingsCommandNames


 type ``When creating commandDefs from handlers``() =
    let archetypesFromSource source =
        let source = addMapStatements false source
        let model = modelFrom (Code source) (Code handlerSource)

        match archetypeInfoFrom (Source.SyntaxTree model.SyntaxTree) with
        | Ok archetype -> archetype
        | Error errors -> invalidOp (concatErrors errors)

    [<Fact>]
    member _.``Handler name is found as method in separate class``() =
        let actual = archetypesFromSource oneMapping
                    |> List.exactlyOne
        let source = addMapStatements false oneMapping
        let model = modelFrom (Code source) (Code handlerSource)

        let archetypeInfo = match archetypeInfoFrom (Source.SyntaxTree model.SyntaxTree) with
                            | Ok archetype -> archetype
                            | Error errors -> invalidOp (concatErrors errors)
                            |> List.exactlyOne

        let handler = evaluateHandler model archetypeInfo.HandlerExpression

        handler |> should not' (be Null)

        handler.ToString()
        |> should haveSubstring "Handlers.A"

    [<Fact>]
    member _.``Option and Argument types are updated on command``() =
