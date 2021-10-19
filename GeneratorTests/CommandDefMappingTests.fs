module Generator.Tests.CommandDefMappingTests

open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open Generator.RoslynUtils
open Generator.GeneralUtils
open Generator.Models
open Generator.Tests.UtilForTests
open Microsoft.CodeAnalysis
open Generator.Tests.MapData

// KAD: If the parens are ommitted here, FsUnit gives an error about only one constructor allowed.
//     Maybe a better error
type ``When evaluating handlers``() =

    [<Fact>]
    member _.``Parameters retrieved from Handler``() =
        let (archetypes, model) = archetypesAndModelFromSource MapData.OneMapping.MapInferredStatements
        let expected = [("one", "string")]

        let parameters = ParametersFromArchetype archetypes[0] model
        let actual = 
            [ for tuple in parameters do
                match tuple with 
                | (name, t) -> (name, t.ToString())]

        actual |> should matchList expected


//type ``When building CommandDef parts``() =

    
//    [<Fact>]
//    member _.``Argument is found``() =
//        let parameters = [("two", "int")]
//        let raw = [ ArgArchetype { Id = "two"; Name = "two"; Aliases = []; HiddenAliases = [] } ]
//        let expected = Some {
//            ArgId = "two" 
//            Name = "two"
//            Description = None
//            Required = None
//            TypeName = "int" }

//        let (arg, options) = argAndOptions parameters raw

//        options |> should be Empty
//        arg |> should be (ofCase <@ Some @>)
//        arg |> should equal expected 

    
//    [<Fact>]
//    member _.``One option is found``() =
//        let parameters = [("one", "string")]
//        let raw = [ OptionArchetype { Id = "one"; Name = "one"; Aliases = []; HiddenAliases = [] } ]
//        let expected = [{
//            OptionId = "one" 
//            Name = "one"
//            Description = None
//            Aliases = []
//            Required = None
//            TypeName = "string" }]

//        let (arg, options) = argAndOptions parameters raw

//        arg |> should equal None
//        options |> should equal expected
//        if arg <> None then failwith "Wat!"

//    [<Fact>]
//    member _.``Two options and one argument are found``() =
//        let parameters = [("one", "string"); ("two", "int"); ("three", "int")]
//        let raw = 
//            [ OptionArchetype { Id = "one"; Name = "one"; Aliases = []; HiddenAliases = [] } 
//              ArgArchetype { Id = "two"; Name = "two"; Aliases = []; HiddenAliases = [] } 
//              OptionArchetype { Id = "three"; Name = "three"; Aliases = []; HiddenAliases = [] } ]

//        let optionDefOne = {
//            OptionId = "one" 
//            Name = "one"
//            Description = None
//            Aliases = []
//            Required = None
//            TypeName = "string" };
//        let optionDefThree = {
//            OptionId = "three" 
//            Name = "three"
//            Description = None
//            Aliases = []
//            Required = None
//            TypeName = "int" }
//        let expectedOptions = [ optionDefOne; optionDefThree ]
//        let expectedArg = Some {
//            ArgId = "two" 
//            Name = "two"
//            Description = None
//            Required = None
//            TypeName = "int" }

//        let (arg, options) = argAndOptions parameters raw

//        arg |> should equal expectedArg
//        options |> should equal expectedOptions


//    [<Fact>]
//    member _.``CommandDef is built``() =
//        let source = AddMapStatements false MapData.ThreeMappings.MapInferredStatements
//        let expected = MapData.ThreeMappings.CommandDef
//        let mutable model = null
//        let result = 
//            InvocationsAndModelFrom source
//            |> Result.map (
//                fun (invocations, m) -> 
//                    model <- m
//                    invocations)
//            |> Result.bind ArchetypeInfoListFrom
//            |> Result.map ArchetypeInfoTreeFrom
//            |> Result.map (CommandDefFrom model)

//        let actual = 
//            match result with 
//            | Ok tree -> tree
//            | Error err -> invalidOp $"Failed to build tree {err}" // TODO: Work on error reporting

//        actual.Length |> should equal 1
//        let actual = actual.Head
//        let errors = MatchCommandDef expected actual
//        errors |> should equal ""
//        actual |> should equal expected
