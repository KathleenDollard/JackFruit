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


type IModifierWord = interface end
type IClassModifierWord = interface inherit IModifierWord end
type IFieldModifierWord = interface inherit IModifierWord end
type IMethodModifierWord = interface inherit IModifierWord end

