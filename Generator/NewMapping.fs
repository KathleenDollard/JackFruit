module Generator.NewMapping

open Microsoft.CodeAnalysis
open Generator.Models
open Generator.LanguageModel
open Common

let CommandDefFromMethod (info: AppModelCommandInfo) =

    let id = 
        match info.InfoCommandId with
        | Some i -> i
        | None -> 
            match info.Method with 
            | MethodSymbol m -> m.Name
            | NoSymbolFound -> "<unknown>"

    let usage = 
        match info.Method with 
        | MethodSymbol m -> UserMethod m
        | NoSymbolFound -> Arbitrary ""

    let returnType = 
        match info.Method with 
        // Don: Why do we need the parens here? The error is "successive args..." when there is only one arg [Consider issue]
        | MethodSymbol m -> ReturnType.Create (m.ReturnType.ToDisplayString())
        | NoSymbolFound -> ReturnTypeVoid

    let commandDef = CommandDef(id, info.Path, returnType, usage, info.Namespace)

    let members = 
        match info.Method with
            | MethodSymbol method -> 
                [ for parameter in method.Parameters do
                    let memberType = NamedItem.Create (parameter.Type.ToDisplayString(), [])
                    let usage = UserParameter parameter
                    MemberDef(parameter.Name, commandDef, memberType, usage, true)]
            | NoSymbolFound -> [] 

    commandDef.Members <- members
    commandDef.AddToPocket "Method" info.Method 
    commandDef

let CommandDefsFrom<'T> (appModel: AppModel<'T>) (items: 'T list)  =

    let rec depthFirstCreate item  =
        let subCommands = 
            [ for child in (appModel.Children item) do
                yield depthFirstCreate child ]
        let info = appModel.Info item
        let commandDef = CommandDefFromMethod info
        commandDef.SubCommands <- subCommands
        //RunTransformers commandDef appModel
        commandDef
    try
        Ok [ for item in items do depthFirstCreate item ]
    with
    | ex -> Error (Other ex.Message)