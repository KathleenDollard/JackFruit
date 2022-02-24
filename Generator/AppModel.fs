namespace Generator

open Generator.Models
open Generator.AppModelHelpers
open Microsoft.CodeAnalysis

type Transformer() =

    abstract member CommandAliasesToAdd : CommandDef -> ItemReturn<string list>
    default _.CommandAliasesToAdd(_) = UsePreviousValue
 
    abstract member NewCommandDescription : CommandDef -> ItemReturn<string option>
    default _.NewCommandDescription(_) = UsePreviousValue
 
    abstract member CommandPocketItemsToAdd : CommandDef -> ItemReturn<(string * obj) list>
    default _.CommandPocketItemsToAdd(_) = UsePreviousValue
 
    abstract member NewMemberKind : MemberDef -> ItemReturn<MemberKind option>
    default _.NewMemberKind(_) = UsePreviousValue
    
    abstract member MemberAliasesToAdd : MemberDef -> ItemReturn<string list>
    default _.MemberAliasesToAdd(_) = UsePreviousValue
 
    abstract member NewMemberArgDisplayName : MemberDef -> ItemReturn<string option>
    default _.NewMemberArgDisplayName(_) = UsePreviousValue
  
    abstract member NewMemberDescription : MemberDef -> ItemReturn<string option>
    default _.NewMemberDescription(_) = UsePreviousValue
 
    abstract member NewMemberRequiredOverride : MemberDef -> ItemReturn<bool option>
    default _.NewMemberRequiredOverride(_) = UsePreviousValue
   
    abstract member MemberPocketItemsToAdd : MemberDef -> ItemReturn<(string * obj) list>
    default _.MemberPocketItemsToAdd(_) = UsePreviousValue
    
type DescriptionsFromAttributesTransformer() =
    inherit Transformer()
        override _.NewCommandDescription commandDef = CommandDescFromAttribute commandDef
        override _.NewMemberDescription memberDef = MemberDescFromAttribute memberDef

type DescriptionsFromXmlCommentsTransformer() =
    inherit Transformer()
        override _.NewCommandDescription commandDef = CommandDescFromXmlComment commandDef
        override _.NewMemberDescription memberDef = MemberDescFromXmlComment memberDef


type AppModelCommandInfo =
    { InfoCommandId: string option
      Path: string list
      Method: IMethodSymbol option
      ForPocket: (string * obj) list 
      Namespace: string}

/// AppModels are distinguished by how they do structural
/// evaluation (Info and Children) and transforms defined 
/// as a set of ICommandDefTransformers and IMemberDefTransformers.
[<AbstractClass>]
type AppModel<'T>() =
    abstract member Initialize: SemanticModel -> Result<'T list, AppErrors>
    abstract member Children: 'T -> 'T list
    abstract member Info: SemanticModel -> 'T -> AppModelCommandInfo
    abstract member Transformers: Transformer list


    default _.Transformers = 
        [ DescriptionsFromXmlCommentsTransformer() 
          DescriptionsFromAttributesTransformer()
              // longish list expected here
        ]


        
