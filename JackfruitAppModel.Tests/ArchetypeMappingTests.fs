module Generator.Tests.ArchetypeMappingTests


open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open Jackfruit.ArchetypeMapping
open Generator.RoslynUtils
open Generator.GeneralUtils
open Jackfruit.Models
open Generator.Tests.UtilsForTests
open Microsoft.CodeAnalysis
open Generator.Tests
open Jackfruit.UtilsForTests
open Jackfruit.Tests


type ``When parsing archetypes``() =
    [<Fact>]
    member _.``Ancestors found for empty archetype``() =
        let actual = ParseArchetypeInfo "\"\"" None

        actual.Path |> should equal [""]

    [<Fact>]
    member _.``Ancestors found for simple archetype``() =
        let actual = ParseArchetypeInfo "\"first\"" None

        actual.Path |> should equal ["first"]

    [<Fact>]
    member _.``Ancestors found for multi-level archetype``() =
        let expectedCommands = [ "first"; "second"; "third"]
        let actual = ParseArchetypeInfo "\"first second third\"" None

        actual.Path |> should equal expectedCommands

    [<Fact>]
    member _.``ArgArchetype parsed from single element part`` () =
        let input = "<one>"
        let expected = 
            ArgArchetype 
                { Id = "one"
                  Name = "one"
                  Aliases = [] 
                  HiddenAliases = []}

        let actual = ArgArchetypeFrom input

        actual |> should equal expected

    [<Fact>]
    member _.``OptionArchetype parsed from single element part`` () =
        let input = "--one"
        let expected = 
           OptionArchetype
               { Id = "one"
                 Name = "one"
                 Aliases = []
                 HiddenAliases = [] }

        let actual = OptionArchetypeFrom input

        actual |> should equal expected

    [<Fact>]
    member _.``CommandArchetype parsed from single element part`` () =
        let input = "one"
        let expected = 
            CommandArchetype
                { Id = "one"
                  Name = "one"
                  Aliases = []
                  HiddenAliases = [] }

        let actual = CommandArchetypeFrom input
        
        actual |> should equal expected

    [<Fact>]
    member _.``ArgArchetype parsed from multi-element part`` () =
        let input = "<one|two>"
        let expected = 
            ArgArchetype 
                { Id = "one"
                  Name = "one"
                  Aliases = ["two"]
                  HiddenAliases = [] }

        let actual = ArgArchetypeFrom input
        
        actual |> should equal expected

    [<Fact>]
    member _.``OptionArchetype parsed from multi-element part`` () =
        let input = "--one|two"
        let expected = 
            OptionArchetype
                { Id = "one"
                  Name = "one"
                  Aliases = ["two"]
                  HiddenAliases = [] }

        let actual = OptionArchetypeFrom input
        
        actual |> should equal expected

    [<Fact>]
    member _.``CommandArchetype parsed from multi-element part`` () =
        let input = "one|two"
        let expected = 
            CommandArchetype
                { Id = "one"
                  Name = "one"
                  Aliases = ["two"]
                  HiddenAliases = [] }

        let actual = CommandArchetypeFrom input
        
        actual |> should equal expected

    [<Fact>]
    member _.``ArgArchetype parsed with hidden id`` () =
        let input = "<[one]|two>"
        let expected = 
            ArgArchetype 
                { Id = "one"
                  Name = "two"
                  Aliases = []
                  HiddenAliases = ["one"] }

        let actual = ArgArchetypeFrom input
        
        actual |> should equal expected

    [<Fact>]
    member _.``OptionArchetype parsed with hidden id`` () =
        let input = "--[one]|two"
        let expected = 
            OptionArchetype
                { Id = "one"
                  Name = "two"
                  Aliases = []
                  HiddenAliases = ["one"] }

        let actual = OptionArchetypeFrom input
        
        actual |> should equal expected

    [<Fact>]
    member _.``CommandArchetype parsed with hidden id`` () =
        let input = "[one]|two"
        let expected = 
            CommandArchetype
                { Id = "one"
                  Name = "two"
                  Aliases = []
                  HiddenAliases = ["one"] }

        let actual = CommandArchetypeFrom input
        
        actual |> should equal expected

    [<Fact>]
    member _.``ArgArchetype parsed with hidden alias`` () =
        let input = "<one|[two]>"
        let expected = 
            ArgArchetype 
                { Id = "one"
                  Name = "one"
                  Aliases = []
                  HiddenAliases = ["two"] }

        let actual = ArgArchetypeFrom input
        
        actual |> should equal expected

    [<Fact>]
    member _.``OptionArchetype parsed with hidden aliss`` () =
        let input = "--one|[two]"
        let expected = 
            OptionArchetype
                { Id = "one"
                  Name = "one"
                  Aliases = []
                  HiddenAliases = ["two"] }

        let actual = OptionArchetypeFrom input
        
        actual |> should equal expected

    [<Fact>]
    member _.``CommandArchetype parsed with hidden alias`` () =
        let input = "one|[two]"
        let expected = 
            CommandArchetype
                { Id = "one"
                  Name = "one"
                  Aliases = []
                  HiddenAliases = ["two"] }

        let actual = CommandArchetypeFrom input
        
        actual |> should equal expected


type ``When creating archetypeInfo from mapping``() =
    let CommandNamesFromSource source =
        let commandNames archInfoList =
            [ for archInfo in archInfoList do
                archInfo.Path |> List.last ]

        let result = 
            ModelFrom [(CSharpCode source); (CSharpCode HandlerSource)]
            |> Result.bind (InvocationsFromModel "MapInferred")
            |> Result.bind ArchetypeInfoListFrom 
            |> Result.map commandNames

        match result with 
        | Ok n -> n
        | Error err -> invalidOp $"Test failed with {err}"


    [<Fact>]
    member _.``None are found when there are none``() =
        let source = MapData.NoMapping.CliCode

        let actual = CommandNamesFromSource source

        actual |> should matchList MapData.NoMapping.CommandNames

    [<Fact>]
    member _.``One is found when there is one``() =
        let source = MapData.OneMapping.CliCode

        let actual = CommandNamesFromSource source

        actual |> should matchList MapData.OneMapping.CommandNames

    [<Fact>]
    member _.``Multiples are found when there are multiple``() =
        let source = MapData.ThreeMappings.CliCode

        let actual = CommandNamesFromSource source

        actual |> should matchList MapData.ThreeMappings.CommandNames

