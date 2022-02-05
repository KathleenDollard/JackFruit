namespace Generator.ExplicitAdd

open Generator
open Common
open Generator.CodeEval
open ExplicitAddMapping


type AppModel(evalLanguage: EvalBase) =
    inherit AppModel<TreeNodeType<ExplicitAddInfo>>() with 

    // TODO: Add SubCommands
    let mapMethodName = "AddCommand"

    override _.Initialize semanticModel =
        evalLanguage.InvocationsFromModel mapMethodName semanticModel
        |> Result.bind (ExplicitAddInfoListFrom evalLanguage)
        |> Result.bind ExplicitAddInfoTreeFrom
        
    override _.Children archTree =
        archTree.Children
        
    override _.Info semanticModel node =
        let nodeInfo = node.Data
        let method = 
            match nodeInfo.Handler with
            | None -> None
            | Some handler -> evalLanguage.MethodSymbolFromMethodCall semanticModel handler
        { InfoCommandId = Some (List.last nodeInfo.Path)
          Path = nodeInfo.Path 
          Method = method 
          ForPocket = [] }
        
    //member _.CommandDefTransformers = []            

    //member _.MemberDefTransformers = []

   