namespace Generator.ExplicitAdd

open Generator
open Common
open Generator.CodeEval
open ExplicitAddMapping
open Models

type AppModel(evalLanguage: EvalBase) =
    inherit AppModel<TreeNodeType<ExplicitAddInfo>>() with 

    // TODO: Add SubCommands
    let mapMethodNames = ["CreateWithRootCommand"; "AddSubCommand"]

    let mutable nspace = ""

    override _.Initialize compilation =
        let syntaxTrees = compilation.SyntaxTrees
        // KAD-Chet: is there a way to do a bind over a list?
        let invocations = 
            [ 
                for syntaxTree in syntaxTrees do
                    let invocationResult = evalLanguage.InvocationsFromSyntaxTree mapMethodNames syntaxTree
                    match invocationResult with
                    | Ok invocations -> for inv in invocations do inv
                    | Error _ -> () // InvocationFromSyntaxTree currently creates no errors
            ]

        // get a random namespace. They should all be the same. May need more work here in the future
        nspace <- 
            if not invocations.IsEmpty then 
                let first = invocations[0]
                let (_, args) = first
                let node = args[0]
                let semanticModel = compilation.GetSemanticModel(node.SyntaxTree)
                evalLanguage.NamespaceFromdDescendant node semanticModel
            else
                ""

        let invocationResult = Ok invocations
        invocationResult|> Result.bind (ExplicitAddInfoListFrom evalLanguage compilation)
        |> Result.bind ExplicitAddInfoTreeFrom
        
    override _.Children archTree =
        archTree.Children
        
    override _.Info node =
        let nodeInfo = node.Data

        let commandId =
            match nodeInfo.Handler  with 
            | MethodSymbol m -> Some m.Name
            | NoSymbolFound -> Option.None
        { InfoCommandId = commandId
          Path = nodeInfo.Path 
          Method = nodeInfo.Handler 
          ForPocket = [] 
          Namespace = nspace}


    //member _.CommandDefTransformers = []            

    //member _.MemberDefTransformers = []

   