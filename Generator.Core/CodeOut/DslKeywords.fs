module DslKeywords

open System.Collections.Generic
open System.Linq
open Common

type AliasWord =
| Alias

type AsWord =
    | As

type ToWord =
    | To

//// As soon as we stript the old builder, remove "Word" from the _members_ here
//type StaticWord =
//    | StaticWord

//type AbstractWord =
//    | AbstractWord

//type SealedWord =
//    | SealedWord

//type AsyncWord =
//    | AsyncWord

//type PartialWord =
//    | PartialWord


type IModifierWord = interface end
type IClassModifierWord = interface inherit IModifierWord end
type IFieldModifierWord = interface inherit IModifierWord end
type IMethodModifierWord = interface inherit IModifierWord end

