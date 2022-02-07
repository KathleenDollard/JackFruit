module ExplicitAdd.AppModelTests


open Xunit
open Generator.Tests.UtilsForTests
open Jackfruit.UtilsForTests
open Generator.NewMapping
//open Generator
open Generator.Models
//open Jackfruit.Tests
open Common
open ApprovalTests.Reporters
open Generator
open Generator.Tests.MapExplicitAddData
open Generator.ExplicitAdd.ExplicitAddMapping
open Generator.ExplicitAdd


type ``Can retrieve method for AddCommand-AddSubCommand``() =
    let eval = EvalCSharp()

    let NodeInfoList source =
        let result = ModelFrom [(CSharpCode source); (CSharpCode HandlerSource)]
        let semanticModel =
            match result with 
            | Ok m -> m
            | Error err -> invalidOp $"Test failed during model creation with {err}"
        
        let invocationResult = 
            let commands =
                eval.InvocationsFromModel ["AddRootCommand"] semanticModel
            let subCommands = 
                eval.InvocationsFromModel ["AddSubCommand"] semanticModel
            match commands, subCommands with
            | (Ok r1, Ok r2) -> Ok (r1 @ r2)
            | (Error err1, Error err2) -> invalidOp $"Failed finding AddCommand/AddSubCommand: {err1} {err2}"
            
        let infoListResult = 
            invocationResult
            |> Result.bind (ExplicitAddInfoListFrom eval semanticModel)

        match infoListResult with 
        | Ok list -> list, semanticModel
        | Error err -> invalidOp $"Test failed durin node info list creation with {err}"

    let TestRetrieval (code: string) (expectedNames: string list) =
        //let code = AddMethodsToClassWithBuilder code
        let (nodeInfoList, semanticModel) = NodeInfoList code
        
        let actual =
            [ for nodeInfo in nodeInfoList do
                match nodeInfo.Handler with 
                | Some handler -> MethodSymbolFromMethodCall semanticModel handler 
                | None -> () ]

        let actualNames = 
            [ for methodOption in actual do 
                match methodOption with 
                | Some method -> method.Name
                | None -> () ]

        if actualNames <> expectedNames then 
            invalidOp $"Names do not match: \r{expectedNames} {actualNames}"

    // Not sure other than the first is interesting
    [<Fact>]
    member _.``Handler found for one mappings``() =
        TestRetrieval OneMapping.CliCode  ["NextGeneration"]

    [<Fact>]
    member _.``Error not thrown when no mapping``() =
        TestRetrieval NoMapping.CliCode  []

    [<Fact>]
    member _.``Handlers found for three mappings``() =
        TestRetrieval ThreeMappings.CliCode  ["OriginalSeries"; "NextGeneration"; "Voyager"]



type ``Can build CommandDef from AddCommand``() =

    let eval = EvalCSharp()

    let InfoTree source =
        let result = ModelFrom [(CSharpCode source); (CSharpCode HandlerSource)]
        let semanticModel =
            match result with 
            | Ok m -> m
            | Error err -> invalidOp $"Test failed during model creation with {err}"
        
        let nodeInfoListResult = 
            eval.InvocationsFromModel ["AddRootCommand"; "AddSubCommand"] semanticModel
            |> Result.bind (ExplicitAddInfoListFrom eval semanticModel)
            |> Result.bind ExplicitAddInfoTreeFrom

        match nodeInfoListResult with 
        | Ok list -> list, semanticModel
        | Error err -> invalidOp $"Test failed durin tree creation with {err}"


    let TestHandlerRetrieval (code: string) (expectedCommandDefs: CommandDef list) =
        //let code = AddMethodsToClassWithBuilder code
        let (infoList, semanticModel) = InfoTree code
        let appModel = AppModel(eval) :> Generator.AppModel<TreeNodeType<ExplicitAddInfo>>
        let commandDefs =
            [ match CommandDefsFrom semanticModel appModel infoList with
                | Ok nodeDefs -> for def in nodeDefs do def 
                | Error e -> invalidOp "Error when building CommandDef"]
        let differences = (CommandDefDifferences expectedCommandDefs commandDefs)

        match differences with 
        | None -> () // All is great!
        | Some issues -> 
            let s = String.concat "" [ for issue in issues do "\r\n" + issue]
            invalidOp $"CommandDefs do not match\r{s}"
       
  
    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``Handler found for one mappings``() =
        TestHandlerRetrieval OneMapping.CliCode OneMapping.CommandDefs

    [<Fact>]
    member _.``Error not thrown when no mapping``() =
        TestHandlerRetrieval NoMapping.CliCode NoMapping.CommandDefs

    [<Fact>]
    member _.``Handlers found for three mappings``() =
        TestHandlerRetrieval ThreeMappings.CliCode ThreeMappings.CommandDefs


 