module DslKeywords

open System.Collections.Generic
open System.Linq
open Generator.Language

type AliasWord =
| Alias

type IModifierWord = interface end
type IClassModifierWord = interface inherit IModifierWord end
type IFieldModifierWord = interface inherit IModifierWord end
type IMethodModifierWord = interface inherit IModifierWord end

type StaticWord =
    | Static
    interface IClassModifierWord
    interface IFieldModifierWord
    interface IMethodModifierWord

type AsyncWord = 
    | Async
    interface IClassModifierWord
    interface IMethodModifierWord

type PartialWord =
    | Partial
    interface IClassModifierWord
    interface IMethodModifierWord

type AbstractWord = 
    | Abstract
    interface IClassModifierWord
    interface IMethodModifierWord

type SealedWord = 
    | Abstract
    interface IClassModifierWord
    interface IMethodModifierWord

type OfWord =
    | Of

// This is expected to grow, thus the pattern to evaluate words in one place. It might change to all keywords
type Modifiers =
    { StaticOrInstance: StaticOrInstance
      IsAsync: bool
      IsPartial: bool }
    /// This method evaluates a list of modifier keywords and returns a structure that represents 
    /// appropriate values.In the common case, just pass modifiersr. If you can override 
    /// defaults, only then pass named parameters. 
    static member Evaluate (
            modifiers: IEnumerable<IModifierWord>,
            ?staticOrInstance: StaticOrInstance,
            ?isAsync: bool,
            ?isPartial: bool) =

        let getValue word ifWordValue param defaultValue =
            if modifiers.Contains word then ifWordValue
            else
                match param with 
                | Some x -> x
                | None -> defaultValue
        {
            Modifiers.StaticOrInstance = getValue Static StaticOrInstance.Static staticOrInstance Instance
            IsAsync = getValue Async true isAsync false
            IsPartial = getValue Partial true isPartial false
        }
