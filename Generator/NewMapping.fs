module Generator.NewMapping

open Microsoft.CodeAnalysis
open Generator.Models

let CommandDefFromMethod model (info: AppModelInfo) =

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

/// Run the transformers defined in the AppModel. At present
/// there is no flexibility in order and the transformer runs
/// as part of the creation of each member or command. The 
/// last one wins. Last one winning was chosen because that is 
/// kind of how the lists need to work, unless we make thigs a 
/// lot more complicated. 
let RunTransformers commandDef appModel  =
    let mutable log = seq {}

    let optionValue<'v> transform memberDef fieldName value =
        let newValue =
            match transform memberDef with 
            | Some v -> if (v <> vaue) then Some v else None
            | None -> None
        match newValue with
        | None -> ()
        | Some _ -> log <- Seq.append log [ $"{fieldName} for {memberDef.MemberId} tranformed by {transform.TransformName}"]

    for transform in appModel.MemberDefTransformers do 
        for memberDef in commandDef.Members do 
            match

        let members = 
            [ for memberDef in commandDef.Members do
                let memberKind = optionValue transform.MemberKind memberDef "" memberDef.MemberKind
                let aliases = optionValue transform.Aliases memberDef "" memberDef.Aliases
                let description = optionValue transform.Description memberDef "" memberDef.Description
                let requiredOverride = optionValue transform.RequiredOverride "" memberDef memberDef.RequiredOverride
                let pocket = optionValue transform.Pocket memberDef "" memberDef.Pocket 
                ]
        let aliases = optionValue transform.Aliases memberDef "" memberDef.Aliases
        let description = optionValue transform.Description "" memberDef memberDef.Description
        let pocket = optionValue transform.Pocket memberDef "" memberDef.Pocket



let CommandDefsFrom<'T> semanticModel (appModel: IAppModel<'T>) (items: 'T list)  =

    let rec depthFirstCreate item  =
        let subCommands = 
            [ for child in (appModel.Children item) do
                yield depthFirstCreate child ]
        let info = appModel.Info semanticModel item
        let commandDef = CommandDefFromMethod semanticModel info
        let commandDef = { commandDef with SubCommands = subCommands }
        RunTransformers commandDef appModel

    [ for item in items do
        depthFirstCreate item ]