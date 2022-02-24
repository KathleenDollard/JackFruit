namespace Generator.ExplicitAdd

open Generator
open Common
open Generator.CodeEval
open ExplicitAddMapping


type AppModel(evalLanguage: EvalBase) =
    inherit AppModel<TreeNodeType<ExplicitAddInfo>>() with 

    // TODO: Add SubCommands
    let mapMethodNames = ["CreateWithRootCommand"; "AddSubCommand"]

    let mutable nspace = ""

    override _.Initialize semanticModel =
        let invocationResult = evalLanguage.InvocationsFromModel mapMethodNames semanticModel
        nspace <- 
            match invocationResult with
            | Ok invocations -> 
                // KAD-Chet: is there an easier way to do this?
                let flat = List.collect (fun (_, invs) -> invs) invocations
                if not flat.IsEmpty then
                    let node = flat.Head
                    evalLanguage.NamespaceFromdDescendant node semanticModel
                else
                    ""
            | Error e -> ""

        invocationResult|> Result.bind (ExplicitAddInfoListFrom evalLanguage semanticModel)
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

    override _.Namespace = nspace

    //member _.CommandDefTransformers = []            

    //member _.MemberDefTransformers = []

   