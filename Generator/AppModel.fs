namespace Generator

open Generator.Models
open System
open Generator.AppModelCommandDefHelpers

type CommandDefTransformer() =

    abstract member AliasesToAdd : CommandDef -> ItemReturn<string list>
    default _.AliasesToAdd(_) = UsePreviousValue
    member this.AddToAliases commandDef =
        match this.AliasesToAdd commandDef with
            | UsePreviousValue -> commandDef
            | NewValue value -> 
                {commandDef with Aliases = (List.append commandDef.Aliases value)}

    abstract member NewDescription : CommandDef -> ItemReturn<string option>
    default _.NewDescription(_) = UsePreviousValue
    member this.UpdateDescription commandDef =
        match this.NewDescription commandDef with
            | UsePreviousValue -> commandDef
            | NewValue value -> 
                {commandDef with Description = value}

    abstract member PocketItemsToAdd : CommandDef -> ItemReturn<string * option>
    default _.PocketItemsToAdd(_) = UsePreviousValue
    member this.AddToPocket commandDef =
        match this.PocketItemsToAdd commandDef with
            | UsePreviousValue -> commandDef
            | NewValue value -> 
                {commandDef with Pocket = (List.append commandDef.Pocket value)}

type MemberDefTransformer() =
    abstract member NewMemberKind : MemberDef -> ItemReturn<MemberKind option>
    default _.NewMemberKind(_) = UsePreviousValue
    member this.UpdateMemberKind memberDef =
        match this.NewMemberKind memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                {memberDef with MemberKind = value}

    abstract member AliasesToAdd : MemberDef -> ItemReturn<string list>
    default _.AliasesToAdd(_) = UsePreviousValue
    member this.AddAliases memberDef =
        match this.AliasesToAdd memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                {memberDef with Aliases = (List.append memberDef.Aliases value)}
 
    abstract member NewArgDisplayName : MemberDef -> ItemReturn<string option>
    default _.NewArgDisplayName(_) = UsePreviousValue
    member this.UpdateArgDisplayName memberDef =
        match this.NewArgDisplayName memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                {memberDef with ArgDisplayName = value}

    abstract member NewDescription : MemberDef -> ItemReturn<string option>
    default _.NewDescription(_) = UsePreviousValue
    member this.UpdateDescription memberDef =
        match this.NewDescription memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                {memberDef with Description = value}

    abstract member NewRequiredOverride : MemberDef -> ItemReturn<bool option>
    default _.NewRequiredOverride(_) = UsePreviousValue
    member this.UpdateRequiredOverride memberDef =
        match this.NewRequiredOverride memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                {memberDef with RequiredOverride = value}

    abstract member PocketItemsToAdd : MemberDef -> ItemReturn<string option>
    default _.PocketItemsToAdd(_) = UsePreviousValue
    member this.AddToPocket memberDef =
        match this.PocketItemsToAdd memberDef with
            | UsePreviousValue -> memberDef
            | NewValue value -> 
                {memberDef with Pocket = (List.append memberDef.Pocket value)}

type AppModel =
    { CommandDefTransformer: CommandDefTransformer
      MemberDefTransformer: MemberDefTransformer }

    static member Apply model commandDef =
        // KAD-Don: When uncommented each line of the following said that commandDef was not used
        //let commandDef = Apply model.NewAliases commandDef
        //let commandDef = Apply model.NewDescription commandDef
        //let commandDef = Apply model.NewPocket commandDef

        let rec ApplyToCommandDef commandDef : CommandDef =
            let commandTransform = model.CommandDefTransformer // just supply shorter name
            let memberTransform = model.MemberDefTransformer
            let commandDef = 
                commandTransform.AddToAliases commandDef
                |> commandTransform.UpdateDescription
                |> commandTransform.AddToPocket
            let members = 
                [ for mbr in commandDef.Members do
                    memberTransform.UpdateMemberKind mbr
                    |> memberTransform.AddAliases 
                    |> memberTransform.UpdateArgDisplayName
                    |> memberTransform.UpdateDescription
                    |> memberTransform.UpdateRequiredOverride
                    |> memberTransform.AddToPocket ]
            let subCommands = 
                [ for subCommandDef in commandDef.SubCommands do
                    ApplyToCommandDef subCommandDef ]
            { commandDef with SubCommands = subCommands; Members = members }

        ApplyToCommandDef commandDef

type DescriptionsFromAttributeAppModel 
    inherits 


type CommonCommandDefAppModel(descriptionMap: Map<string, string> option, commonAliases: string * string option) =

    member this.UpdateCommandDef(commandDef) =
        // Last wins when multiple things contribute
        // This is deliberately granular
        commandDef 
        |> DescriptionsFromAttribute.ApplyAppModel model
        |> DescriptionsFromXmlComment.ApplyAppModel model
        |> (DescriptionsFromLookup descriptionMap).ApplyAppModel model
        |> (AliasesForCli commonAliases).ApplyAppModel model
        |> AliasesFromAttribute.ApplyAppModel model

      
   
        







type AppModel =
    { CommandDefAppModels: CommandDefAppModel list
      MemberDefAppModels: MemberDefAppModel list }




