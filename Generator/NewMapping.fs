module Generator.NewMapping

open Microsoft.CodeAnalysis
open Generator.Models
open Generator.LanguageModel
open Common

let CommandDefFromMethod model (info: AppModelCommandInfo) =

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
        | None -> Arbitrary ""

    let returnType = 
        match info.Method with 
        // Don: Why do we need the parens here? The error is "successive args..." when there is only one arg [Consider issue]
        | Some m -> ReturnType.Create (m.ReturnType.ToDisplayString())
        | None -> ReturnTypeVoid

    let commandDef = CommandDef(id, info.Path, returnType, usage)

    let members = 
        match info.Method with
            | Some method -> 
                [ for parameter in method.Parameters do
                    let memberType = NamedItem.Create (parameter.Type.ToDisplayString(), [])
                    let usage = UserParameter parameter
                    MemberDef(parameter.Name, commandDef, memberType, usage, true)]
            | None -> [] 

    commandDef.Members <- members
    commandDef.AddToPocket "Method" info.Method 
    commandDef.AddToPocket "SemanticModel" model
    commandDef

let CommandDefsFrom<'T> semanticModel (appModel: AppModel<'T>) (items: 'T list)  =

    let rec depthFirstCreate item  =
        let subCommands = 
            [ for child in (appModel.Children item) do
                yield depthFirstCreate child ]
        let info = appModel.Info semanticModel item
        let commandDef = CommandDefFromMethod semanticModel info
        commandDef.SubCommands <- subCommands
        //RunTransformers commandDef appModel
        commandDef
    try
        Ok [ for item in items do depthFirstCreate item ]
    with
    | ex -> Error (Other ex.Message)