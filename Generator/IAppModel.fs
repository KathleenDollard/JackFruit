namespace Generator

open Microsoft.CodeAnalysis
open Generator.Models


type AppModelCommandInfo =
    { InfoCommandId: string option
      Path: string list
      Method: IMethodSymbol option
      ForPocket: (string * obj) list }

// I considered making transformer members options so they might
// not be run, but there are three cases:
//      * The transformer never affects this value
//      * The transformer might affects the value, depending on conditions
//        (such as the presence of an XML comment or dictionary entry)
//      * The transformer affects the value to reset it to None
//
// The first and second are the common cases, and the optimization of
// skipping the transformer, rather than returning None, which is the 
// indicator of the second case. 
//
// Identity transforms are not used because we could not log what is
// causing the transformation, which I think is probably important. 
//
// If anyone actually needs the third option, we can add reset methods. 
// Ugly, but effective.
//
// Writing all this here, because I switched back and forth multiple times
// and I am still not certain this is correct. 
//type ICommandDefTransformer =
//    { /// The transformer name as used for logging.
//      TransformerName: string
//      Aliases: CommandDef -> string list
//      Description: CommandDef -> string option
//      Pocket: CommandDef ->  (string * obj) list }


//type IMemberDefTransformer =
//    { /// The transformer name as used for logging.
//      TransformerName: string
//      MemberKind: MemberDef -> MemberKind option
//      Aliases: MemberDef -> string list
//      ArgDisplayName: MemberDef -> string list
//      Description: MemberDef -> string option
//      RequiredOverride: MemberDef -> bool option
//      Pocket: MemberDef ->  (string * obj) list }

/// AppModels are distinguished by how they do structural
/// evaluation (Info and Childre) and transforms defined 
/// as a set of ICommandDefTransformers and IMemberDefTransformers.
type IAppModel<'T> =
    abstract member Children: 'T -> 'T list
    abstract member Info: SemanticModel -> 'T -> AppModelCommandInfo
    //abstract member CommandDefTransformers: ICommandDefTransformer list
    //abstract member MemberDefTransformers: IMemberDefTransformer list




