namespace Generator

open Generator.Models
open Generator.AppModelHelpers
open Microsoft.CodeAnalysis

type Transformer() =

    abstract member CommandAliasesToAdd : CommandDef -> ItemReturn<string list>
    default _.CommandAliasesToAdd(_) = UsePreviousValue
    member this.AddToCommandAliases commandDef =
        match this.CommandAliasesToAdd commandDef with
            | UsePreviousValue -> commandDef
            | NewValue value -> 
                commandDef.Aliases <- (List.append commandDef.Aliases value)
                commandDef
              //  {commandDef with Aliases = (List.append commandDef.Aliases value)}

    abstract member NewCommandDescription : CommandDef -> ItemReturn<string option>
    default _.NewCommandDescription(_) = UsePreviousValue
    member this.UpdateCommandDescription commandDef =
        match this.NewCommandDescription commandDef with
            | UsePreviousValue -> commandDef
            | NewValue value -> 
                commandDef.Description <- value
                commandDef

    abstract member CommandPocketItemsToAdd : CommandDef -> ItemReturn<(string * obj) list>
    default _.CommandPocketItemsToAdd(_) = UsePreviousValue
    member this.AddToCommandPocket commandDef =
        match this.CommandPocketItemsToAdd commandDef with
            | UsePreviousValue -> commandDef
            | NewValue pairs -> 
                List.iter (fun (key, value) -> commandDef.AddToPocket key value) pairs
                commandDef

    abstract member NewMemberKind : MemberDef -> ItemReturn<MemberKind option>
    default _.NewMemberKind(_) = UsePreviousValue
    member this.UpdateMemberKind memberDef =
        match this.NewMemberKind memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value ->
                memberDef.MemberKind <- value
                memberDef

    abstract member MemberAliasesToAdd : MemberDef -> ItemReturn<string list>
    default _.MemberAliasesToAdd(_) = UsePreviousValue
    member this.AddMemberAliases memberDef =
        match this.MemberAliasesToAdd memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                memberDef.Aliases <- (List.append memberDef.Aliases value)
                memberDef
 
    abstract member NewMemberArgDisplayName : MemberDef -> ItemReturn<string option>
    default _.NewMemberArgDisplayName(_) = UsePreviousValue
    member this.UpdateMemberArgDisplayName memberDef =
        match this.NewMemberArgDisplayName memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                memberDef.ArgDisplayName <- value
                memberDef

    abstract member NewMemberDescription : MemberDef -> ItemReturn<string option>
    default _.NewMemberDescription(_) = UsePreviousValue
    member this.UpdateMemberDescription memberDef =
        match this.NewMemberDescription memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                memberDef.Description <- value
                memberDef

    abstract member NewMemberRequiredOverride : MemberDef -> ItemReturn<bool option>
    default _.NewMemberRequiredOverride(_) = UsePreviousValue
    member this.UpdateMemberRequiredOverride memberDef =
        match this.NewMemberRequiredOverride memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                memberDef.RequiredOverride <- value
                memberDef

    abstract member MemberPocketItemsToAdd : MemberDef -> ItemReturn<(string * obj) list>
    default _.MemberPocketItemsToAdd(_) = UsePreviousValue
    member this.AddToMemberPocket (memberDef: MemberDef) =
        match this.MemberPocketItemsToAdd memberDef with
            | UsePreviousValue -> memberDef
            | NewValue pairs -> 
                List.iter (fun (key, value) -> memberDef.AddToPocket key value) pairs
                memberDef

    member this.Apply commandDef =

        let rec ApplyToCommandDef commandDef : CommandDef =
            let newCommandDef = 
                this.AddToCommandAliases commandDef
                |> this.UpdateCommandDescription
                |> this.AddToCommandPocket
            for mbr in newCommandDef.Members do
                this.UpdateMemberKind mbr
                |> this.AddMemberAliases 
                |> this.UpdateMemberArgDisplayName
                |> this.UpdateMemberDescription
                |> this.UpdateMemberRequiredOverride
                |> this.AddToMemberPocket |> ignore
            for subCommandDef in newCommandDef.SubCommands do
                ApplyToCommandDef subCommandDef |> ignore
            commandDef

        let commandDef2 = ApplyToCommandDef commandDef
        commandDef2

type DescriptionsFromAttributesTransformer() =
    inherit Transformer()
        override this.NewCommandDescription commandDef = CommandDescFromAttribute commandDef
        override this.NewMemberDescription memberDef = MemberDescFromAttribute memberDef

type DescriptionsFromXmlCommentsTransforer() =
    inherit Transformer()
        override this.NewCommandDescription commandDef = CommandDescFromXmlComment commandDef
        override this.NewMemberDescription memberDef = MemberDescFromXmlComment memberDef


type AppModelCommandInfo =
    { InfoCommandId: string option
      Path: string list
      Method: IMethodSymbol option
      ForPocket: (string * obj) list }

/// AppModels are distinguished by how they do structural
/// evaluation (Info and Children) and transforms defined 
/// as a set of ICommandDefTransformers and IMemberDefTransformers.
[<AbstractClass>]
type AppModel<'T>() =
    //abstract member Initialize: SemanticModel -> Result<'T list, AppErrors>
    abstract member Children: 'T -> 'T list
    abstract member Info: SemanticModel -> 'T -> AppModelCommandInfo
    abstract member Transformers: Transformer list
    default _.Transformers = 
        [ DescriptionsFromXmlCommentsTransforer() 
          DescriptionsFromAttributesTransformer()
              // longish list expected here
        ]


        
