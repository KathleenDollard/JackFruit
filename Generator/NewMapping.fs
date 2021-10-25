module Generator.NewMapping

open Microsoft.CodeAnalysis
open Generator.Models

let CommandDefFromMethod model (info: AppModelInfo) =

    let members = 
        match info.Method with
             | Some method -> 
                 [ for parameter in method.Parameters do
                     // KAD-Don: Why does this error without parens? What is "Function application"?
                     MemberDef.Create parameter.Name (parameter.Type.ToDisplayString()) ]
             | None -> [] 

    let id = 
        match info.InfoCommandId with
        | Some i -> i
        | None -> 
            match info.Method with 
            | Some m -> m.Name
            | None -> "<unknown>"

    let commandDef = CommandDef.Create id
    { commandDef with 
        Path = info.Path
        Aliases = [id]
        Members = members
        Pocket = 
        [ "Method", info.Method 
          "SemanticModel", model] }


let CommandDefsFrom<'T> semanticModel (appModel: IAppModel<'T>) (items: 'T list)  =

    let rec depthFirstCreate item  =
        let subCommands = 
            [ for child in (appModel.Children item) do
                yield depthFirstCreate child ]
        let info = appModel.Info semanticModel item
        let commandDef = CommandDefFromMethod semanticModel info
        let commandDef = { commandDef with SubCommands = subCommands }
        appModel.RunProviders commandDef

    [ for item in items do
        depthFirstCreate item ]