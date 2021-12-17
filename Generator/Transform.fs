module Generator.Transform

open Generator.Models
open Generator


let AddToCommandAliases (transform: Transformer) commandDef =
    match transform.CommandAliasesToAdd commandDef with
        | UsePreviousValue -> commandDef
        | NewValue value -> 
            commandDef.Aliases <- (List.append commandDef.Aliases value)
            commandDef
            //  {commandDef with Aliases = (List.append commandDef.Aliases value)}

let UpdateCommandDescription (transform: Transformer) commandDef =
        match transform.NewCommandDescription commandDef with
            | UsePreviousValue -> commandDef
            | NewValue value -> 
                commandDef.Description <- value
                commandDef

let AddToCommandPocket (transform: Transformer) commandDef =
        match transform.CommandPocketItemsToAdd commandDef with
            | UsePreviousValue -> commandDef
            | NewValue pairs -> 
                List.iter (fun (key, value) -> commandDef.AddToPocket key value) pairs
                commandDef

let UpdateMemberKind (transform: Transformer) memberDef =
        match transform.NewMemberKind memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value ->
                memberDef.MemberKind <- value
                memberDef

let AddMemberAliases (transform: Transformer) memberDef =
        match transform.MemberAliasesToAdd memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                memberDef.Aliases <- (List.append memberDef.Aliases value)
                memberDef
 
let UpdateMemberArgDisplayName (transform: Transformer) memberDef =
        match transform.NewMemberArgDisplayName memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                memberDef.ArgDisplayName <- value
                memberDef

let UpdateMemberDescription (transform: Transformer) memberDef =
        match transform.NewMemberDescription memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                memberDef.Description <- value
                memberDef

let UpdateMemberRequiredOverride (transform: Transformer) memberDef =
        match transform.NewMemberRequiredOverride memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                memberDef.RequiredOverride <- value
                memberDef

let AddToMemberPocket (transform: Transformer) (memberDef: MemberDef) =
        match transform.MemberPocketItemsToAdd memberDef with
            | UsePreviousValue -> memberDef
            | NewValue pairs -> 
                List.iter (fun (key, value) -> memberDef.AddToPocket key value) pairs
                memberDef

let ApplyTransform (transform: Transformer) commandDef =

        let rec ApplyToCommandDef commandDef : CommandDef =
            let newCommandDef = 
                AddToCommandAliases transform commandDef
                |> UpdateCommandDescription transform
                |> AddToCommandPocket transform
            for mbr in newCommandDef.Members do
                UpdateMemberKind transform mbr
                |> AddMemberAliases transform 
                |> UpdateMemberArgDisplayName transform
                |> UpdateMemberDescription transform
                |> UpdateMemberRequiredOverride transform
                |> AddToMemberPocket transform |> ignore
            for subCommandDef in newCommandDef.SubCommands do
                ApplyToCommandDef subCommandDef |> ignore
            commandDef

        let commandDef2 = ApplyToCommandDef commandDef
        commandDef2
