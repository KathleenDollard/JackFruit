namespace Generator

open Microsoft.CodeAnalysis
open Generator.Models


type AppModelInfo =
    { InfoCommandId: string option
      Path: string list
      Method: IMethodSymbol option
      ForPocket: (string * obj) list }

// If this feels like overkill for two values, more are coming :)
type ICommandDefTransformer =
    { 
      // KAD-Don:** I have having trouble with this signature. What I want is
      // X: ((CommandDef (string list)) -> string list) option
      // I can do it with a tuple, but why require a tuple of users>
      Aliases: (string list -> string list) option
      Description: (string option -> string option) option
      Pocket: ((string * obj) list ->  (string * obj) list) option }

type IMemberDefTransformer =
    { MemberKind: (MemberKind option -> MemberKind option) option
      Aliases: (string list -> string list) option
      ArgDisplayName: (string list -> string list) option
      Description: (string option -> string option) option
      RequiredOverride: (bool option -> bool option) option
      Pocket: ((string * obj) list ->  (string * obj) list) option }

type IAppModel<'T> =
    abstract member Children: 'T -> 'T list
    // Id, method, stuff for pocket
    abstract member Info: SemanticModel -> 'T -> AppModelInfo
    abstract member CommanDDefTransformers: ICommandDefTransformer list
    abstract member MemberDefTransformers: IMemberDefTransformer list




