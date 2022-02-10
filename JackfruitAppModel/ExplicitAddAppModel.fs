namespace Generator.ExplicitAdd

open Generator
open Common
open Generator.CodeEval
open ExplicitAddMapping


type AppModel(evalLanguage: EvalBase) =
    inherit AppModel<TreeNodeType<ExplicitAddInfo>>() with 

    // TODO: Add SubCommands
    let mapMethodNames = ["CreateWithRootCommand"; "AddSubCommand"]

    override _.Initialize semanticModel =
        evalLanguage.InvocationsFromModel mapMethodNames semanticModel
        |> Result.bind (ExplicitAddInfoListFrom evalLanguage semanticModel)
        |> Result.bind ExplicitAddInfoTreeFrom
        
    override _.Children archTree =
        archTree.Children
        
    override _.Info semanticModel node =
        let nodeInfo = node.Data
        let method = 
            match nodeInfo.Handler with
            | None -> None
            | Some handler -> evalLanguage.MethodSymbolFromMethodCall semanticModel handler
        let commandId =
            match method with 
            | Some m -> Some m.Name
            | None -> None
        { InfoCommandId = commandId
          Path = nodeInfo.Path 
          Method = method 
          ForPocket = [] }
        
    //member _.CommandDefTransformers = []            

    //member _.MemberDefTransformers = []

   