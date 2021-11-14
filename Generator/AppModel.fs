namespace Generator

open Generator.Models
open System
open Generator.AppModelHelpers
open AppModelHelpers
open Microsoft.CodeAnalysis

type Transformer() =

    abstract member CommandAliasesToAdd : CommandDef -> ItemReturn<string list>
    default _.CommandAliasesToAdd(_) = UsePreviousValue
    member this.AddToCommandAliases commandDef =
        match this.CommandAliasesToAdd commandDef with
            | UsePreviousValue -> commandDef
            | NewValue value -> 
                {commandDef with Aliases = (List.append commandDef.Aliases value)}

    abstract member NewCommandDescription : CommandDef -> ItemReturn<string option>
    default _.NewCommandDescription(_) = UsePreviousValue
    member this.UpdateCommandDescription commandDef =
        match this.NewCommandDescription commandDef with
            | UsePreviousValue -> commandDef
            | NewValue value -> 
                let newCommandDef = {commandDef with Description = value}
                newCommandDef

    abstract member CommandPocketItemsToAdd : CommandDef -> ItemReturn<(string * obj) list>
    default _.CommandPocketItemsToAdd(_) = UsePreviousValue
    member this.AddToCommandPocket commandDef =
        match this.CommandPocketItemsToAdd commandDef with
            | UsePreviousValue -> commandDef
            | NewValue value -> 
                {commandDef with Pocket = (List.append commandDef.Pocket value)}

    abstract member NewMemberKind : MemberDef -> ItemReturn<MemberKind option>
    default _.NewMemberKind(_) = UsePreviousValue
    member this.UpdateMemberKind memberDef =
        match this.NewMemberKind memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                {memberDef with MemberKind = value}

    abstract member MemberAliasesToAdd : MemberDef -> ItemReturn<string list>
    default _.MemberAliasesToAdd(_) = UsePreviousValue
    member this.AddMemberAliases memberDef =
        match this.MemberAliasesToAdd memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                {memberDef with Aliases = (List.append memberDef.Aliases value)}
 
    abstract member NewMemberArgDisplayName : MemberDef -> ItemReturn<string option>
    default _.NewMemberArgDisplayName(_) = UsePreviousValue
    member this.UpdateMemberArgDisplayName memberDef =
        match this.NewMemberArgDisplayName memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                {memberDef with ArgDisplayName = value}

    abstract member NewMemberDescription : MemberDef -> ItemReturn<string option>
    default _.NewMemberDescription(_) = UsePreviousValue
    member this.UpdateMemberDescription memberDef =
        match this.NewMemberDescription memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                {memberDef with Description = value}

    abstract member NewMemberRequiredOverride : MemberDef -> ItemReturn<bool option>
    default _.NewMemberRequiredOverride(_) = UsePreviousValue
    member this.UpdateMemberRequiredOverride memberDef =
        match this.NewMemberRequiredOverride memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                {memberDef with RequiredOverride = value}

    abstract member MemberPocketItemsToAdd : MemberDef -> ItemReturn<(string * obj) list>
    default _.MemberPocketItemsToAdd(_) = UsePreviousValue
    member this.AddToMemberPocket memberDef =
        match this.MemberPocketItemsToAdd memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                {memberDef with Pocket = (List.append memberDef.Pocket value)}

    member this.Apply commandDef =
        // KAD-Don: When uncommented each line of the following said that commandDef was not used
        //let commandDef = Apply model.NewAliases commandDef
        //let commandDef = Apply model.NewDescription commandDef
        //let commandDef = Apply model.NewPocket commandDef

        let rec ApplyToCommandDef commandDef : CommandDef =
            let newCommandDef = 
                this.AddToCommandAliases commandDef
                |> this.UpdateCommandDescription
                |> this.AddToCommandPocket
            let members = 
                [ for mbr in newCommandDef.Members do
                    this.UpdateMemberKind mbr
                    |> this.AddMemberAliases 
                    |> this.UpdateMemberArgDisplayName
                    |> this.UpdateMemberDescription
                    |> this.UpdateMemberRequiredOverride
                    |> this.AddToMemberPocket ]
            let subCommands = 
                [ for subCommandDef in newCommandDef.SubCommands do
                    ApplyToCommandDef subCommandDef ]
            { newCommandDef with SubCommands = subCommands; Members = members }

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
/// evaluation (Info and Childre) and transforms defined 
/// as a set of ICommandDefTransformers and IMemberDefTransformers.
[<AbstractClass>]
type AppModel<'T>() =
    abstract member Children: 'T -> 'T list
    abstract member Info: SemanticModel -> 'T -> AppModelCommandInfo
    abstract member Transformers: Transformer list
    default _.Transformers = 
        [ DescriptionsFromXmlCommentsTransforer() 
          DescriptionsFromAttributesTransformer()
              // longish list expected here
        ]


        
