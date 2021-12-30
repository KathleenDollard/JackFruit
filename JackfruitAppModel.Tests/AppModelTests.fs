module AppModelTests


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
open Generator.NewMapping
//open Generator
open Generator.Models
open Jackfruit.Tests
open Common


type ``Can retrieve method for archetype``() =

    let ArchetypeList source =
        let result = ModelFrom [(CSharpCode source); (CSharpCode HandlerSource)]
        let model =
            match result with 
            | Ok m -> m
            | Error err -> invalidOp $"Test failed during model creation with {err}"
        
        let archListResult = 
            InvocationsFromModel "MapInferred" model
            |> Result.bind ArchetypeInfoListFrom

        match archListResult with 
        | Ok list -> list, model
        | Error err -> invalidOp $"Test failed durin Archetype list creation with {err}"

    let TestArchetypeHandlerRetrieval (code: string list) (expectedNames: string list) =
        let code = AddMethodsToClassWithBuilder code
        let (archetypeList, model) = ArchetypeList code
        
        let actual =
            [ for archetype in archetypeList do
                match archetype.Handler with 
                | Some handler -> MethodSymbolFromMethodCall model handler 
                | None -> () ]

        let actualNames = 
            [ for methodOption in actual do 
                match methodOption with 
                | Some method -> method.Name
                | None -> () ]

        if actualNames <> expectedNames then raise (MatchException ( expectedNames.ToString(), actualNames.ToString(), ""))

    // Not sure other than the first is interesting
    [<Fact>]
    member _.``Handler found for one mappings``() =
        TestArchetypeHandlerRetrieval MapData.OneMapping.MapInferredStatements  ["A"]

    [<Fact>]
    member _.``Error not thrown when no mapping``() =
        TestArchetypeHandlerRetrieval MapData.NoMapping.MapInferredStatements  []

    [<Fact>]
    member _.``Handlers found for three mappings``() =
        TestArchetypeHandlerRetrieval MapData.ThreeMappings.MapInferredStatements  ["Dotnet"; "AddPackage"]



type ``Can build CommandDef from archetype``() =


    let ArchetypeTree source =
        let result = ModelFrom [(CSharpCode source); (CSharpCode HandlerSource)]
        let model =
            match result with 
            | Ok m -> m
            | Error err -> invalidOp $"Test failed during model creation with {err}"
        
        let archListResult = 
            InvocationsFromModel "MapInferred" model
            |> Result.bind ArchetypeInfoListFrom
            |> Result.bind ArchetypeInfoTreeFrom

        match archListResult with 
        | Ok list -> list, model
        | Error err -> invalidOp $"Test failed durin ArchetypeTree creation with {err}"

    let TestArchetypeHandlerRetrieval (code: string list) (expectedCommandDefs: CommandDef list) =
        let code = AddMethodsToClassWithBuilder code
        let (archetypeTreeList, model) = ArchetypeTree code
        let appModel = Jackfruit.AppModel() :> Generator.AppModel<TreeNodeType<ArchetypeInfo>>
        let commandDefs =
            [ match CommandDefsFrom model appModel archetypeTreeList with
                | Ok nodeDefs -> for def in nodeDefs do def 
                | Error e -> invalidOp "Error when building CommandDef"]
        let differences = (CommandDefDifferences expectedCommandDefs commandDefs)

        match differences with 
        | None -> () // All is great!
        | Some issues -> 
            raise (MatchException (expectedCommandDefs.ToString(), commandDefs.ToString(), (String.concat "" [ for issue in issues do "\r\n" + issue])))
       
  
    [<Fact>]
    member _.``Handler found for one mappings``() =
        TestArchetypeHandlerRetrieval MapData.OneMapping.MapInferredStatements  MapData.OneMapping.CommandDefs

    [<Fact>]
    member _.``Error not thrown when no mapping``() =
        TestArchetypeHandlerRetrieval MapData.NoMapping.MapInferredStatements MapData.NoMapping.CommandDefs

    [<Fact>]
    member _.``Handlers found for three mappings``() =
        TestArchetypeHandlerRetrieval MapData.ThreeMappings.MapInferredStatements MapData.ThreeMappings.CommandDefs


 