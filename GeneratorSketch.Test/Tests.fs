 module Tests

    open Xunit
    open FsUnit.Xunit
    open GeneratorSketch.Generator
    open Microsoft.CodeAnalysis.CSharp
    open System.Linq
    open TestUtils
    open CSharpTestCode
        
    let handlerSource = readCodeFromFile "..\..\..\TestHandlers.cs"
    let addMapStatementToTestCode (statement:string) =
        let methods = [ createMethod "MethodA" statement; handlerMethod ]
        // KAD: can this pipe? I did not make that work
        let x = (String.concat "" (List.toSeq methods))
        createNamespace [] testNamespace (createClass "ClassA" x)
    let oneMapping = addMapStatementToTestCode """MapInferred("", Handlers.A);"""
    let oneMappingCommandNames = [""]
    let noMapping = addMapStatementToTestCode ""
    let noMappingCommandNames = []
    let threeMappings = addMapStatementToTestCode 
                            """MapInferred("", Handlers.A);
                            MapInferred("add <PROJECT>", null);
                            MapInferred("add package <PACKAGE_NAME>", Handlers.B);"""
    let threeMappingsCommandNames = [""; "add"; "package"]

    type ``When parsing archetypes``() =

        [<Fact>]
        member _.``Info is created from a simple archetype`` () =
            let actual = parseArchetype "\"first\""

            actual.commandName |> should equal "first"
           // Assert.Equal ("first", actual.commandName)

        [<Fact>]
        member _.``Parents are parsed from the archetype`` () =
            let expectedParentCommands = ["first"; "second"]
            let expectedCommandName = "third"

            let actual = parseArchetype "\"first second third\""

            actual.commandName |> should equal expectedCommandName
            actual.parentCommandNames |> should matchList expectedParentCommands

            //Assert.Equal (expected.Length - 1, actual.parentCommandNames.Length)
            //Assert.Equal (expected.[0], actual.commandName)
            //Assert.Equal (expected.[1], actual.parentCommandNames.[0])
            //Assert.Equal (expected.[2], actual.parentCommandNames.[1])

    type ``When creating commandDefs``() =
        [<Fact>]
        member _.``None are found when there are no mappings`` () =
            let source = noMapping
            let tree = CSharpSyntaxTree.ParseText(source)

            let actual = commandInfo tree

            actual |> should matchList noMappingCommandNames
            //Assert.Equal (0, actual.Length)

        [<Fact>]
        member _.``One is found when there is one mapping`` () =
            let tree = CSharpSyntaxTree.ParseText(oneMapping)
            if tree.GetDiagnostics().Count() > 0
            then invalidOp "Compilation failed during Arrange"

            let actual = commandInfo tree
            let actualNames = [ for c in actual do
                                    c.archetype.commandName ]

            actualNames |> should matchList oneMappingCommandNames
            // Assert.Equal (1, actual.Length)

        [<Fact>]
        member _.``Multiples are found when there are multiple mappings`` () =
            let tree = CSharpSyntaxTree.ParseText threeMappings
            if tree.GetDiagnostics().Count() > 0
            then invalidOp "Compilation failed during Arrange"

            let actual = commandInfo tree
            let actualNames = [ for c in actual do
                                    c.archetype.commandName ]

            actualNames |> should matchList threeMappingsCommandNames
            //Assert.Equal (3, actual.Length)


        [<Fact>]
        member _.``Handler name is found as method in separate class`` () =

            let handler = buildHandler oneMapping handlerSource

            handler |> should not' (be Null)
            handler.ToString() |> should haveSubstring "Handlers.A"
            handler.Parameters.Count() |> should equal 2 