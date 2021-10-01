module Generator.Tests.CommandDefMappingTests

open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open Generator.ArchetypeMapping
open Generator.RoslynUtils
open Generator.GeneralUtils
open Generator.Models
open Generator.Tests.UtilForTests
open Generator.CommandDefMapping
open Microsoft.CodeAnalysis
open Generator.Tests.TestData

// KAD: If the parens are ommitted here, FsUnit gives an error about only one constructor allowed.
//      Maybe it is an error that FS shoudl have captured.
type ``When evaluating handlers``() =

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

