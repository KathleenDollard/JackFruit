module Generator.NewMapping

open Microsoft.CodeAnalysis
open Generator.Models

let CommandDefFromMethod model (info: AppModelCommandInfo) =

    let members = 
        match info.Method with
             | Some method -> 
                 [ for parameter in method.Parameters do
                     let usage = UserParameter parameter
                     // KAD-Don: Why does this error without parens? 
                     MemberDef.Create usage parameter.Name (parameter.Type.ToDisplayString()) ]
             | None -> [] 

    let id = 
        match info.InfoCommandId with
        | Some i -> i
        | None -> 
            match info.Method with 
            | Some m -> m.Name
            | None -> "<unknown>"

    let usage = 
        match info.Method with 
        | Some m -> UserMethod (m, model)
        | None -> Arbitrary

    let commandDef = CommandDef.Create usage id
    { commandDef with 
        Path = info.Path
        Aliases = [id]
        Members = members
        Pocket = 
        [ "Method", info.Method 
          "SemanticModel", model] }

let CommandDefsFrom<'T> semanticModel (appModel: AppModel<'T>) (items: 'T list)  =

    let rec depthFirstCreate item  =
        let subCommands = 
            [ for child in (appModel.Children item) do
                yield depthFirstCreate child ]
        let info = appModel.Info semanticModel item
        let commandDef = CommandDefFromMethod semanticModel info
        let commandDef = { commandDef with SubCommands = subCommands }
        //RunTransformers commandDef appModel
        commandDef

    [ for item in items do
        depthFirstCreate item ]