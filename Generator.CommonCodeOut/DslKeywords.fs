module DslKeywords

open System.Collections.Generic
open System.Linq
open Generator.Language

type AliasWord =
| Alias

type OfWord =
    | Of


type IModifierWord = interface end
type IClassModifierWord = interface inherit IModifierWord end
type IFieldModifierWord = interface inherit IModifierWord end
type IMethodModifierWord = interface inherit IModifierWord end

type StaticWord =
    | Static
    interface IModifierWord
    interface IClassModifierWord
    interface IFieldModifierWord
    interface IMethodModifierWord

type AsyncWord = 
    | Async
    interface IModifierWord
    interface IClassModifierWord
    interface IMethodModifierWord

type PartialWord =
    | Partial
    interface IModifierWord
    interface IClassModifierWord
    interface IMethodModifierWord

type AbstractWord = 
    | Abstract
    interface IModifierWord
    interface IClassModifierWord
    interface IMethodModifierWord

type SealedWord = 
    | Sealed
    interface IClassModifierWord
    interface IMethodModifierWord

type ReadonlyWord = 
    | ReadOnly
    interface IFieldModifierWord

// This is expected to grow, thus the pattern to evaluate words in one place. It might change to all keywords
type internal Modifiers =
    { StaticOrInstance: StaticOrInstance
      IsAbstract: bool
      IsAsync: bool
      IsPartial: bool
      IsSealed: bool}

    // KAD-Chet: I did not get this signature to work
    //static member Evaluate<'T when 'T :> IModifierWord>(
    //        modifiers: 'T[],
    //        ?staticOrInstance: StaticOrInstance,
    //        ?isAbstract,
    //        ?isAsync: bool,
    //        ?isPartial: bool,
    //        ?isSealed: bool ) =
    /// This method evaluates a list of modifier keywords and returns a structure that represents 
    /// appropriate values.In the common case, just pass modifiersr. If you can override 
    /// defaults, only then pass named parameters. 
    static member Evaluate(
            modifiers: IModifierWord[],
            ?staticOrInstance: StaticOrInstance,
            ?isAbstract,
            ?isAsync: bool,
            ?isPartial: bool,
            ?isSealed: bool ) =

        let getValue (word:IModifierWord) ifWordValue param defaultValue =
            if modifiers.Contains word then ifWordValue
            else
                match param with 
                | Some x -> x
                | None -> defaultValue

        {
            Modifiers.StaticOrInstance = getValue Static StaticOrInstance.Static staticOrInstance Instance
            IsAbstract = getValue Abstract true  isAbstract false
            IsAsync = getValue Async true isAsync false
            IsPartial = getValue Partial true isPartial false
            IsSealed = getValue Sealed true isSealed false
        }

    static member Evaluate(
        modifiers: IClassModifierWord[],
        ?staticOrInstance: StaticOrInstance,
        ?isAbstract: bool,
        ?isAsync: bool,
        ?isPartial: bool,
        ?isSealed: bool ) =
            
        Modifiers.Evaluate (modifiers.OfType<IModifierWord>().ToArray(), 
            ?staticOrInstance = staticOrInstance, 
            ?isAbstract = isAbstract, 
            ?isAsync = isAsync, 
            ?isPartial = isPartial, 
            ?isSealed = isSealed)

    static member Evaluate(
        modifiers: IFieldModifierWord[],
        ?staticOrInstance: StaticOrInstance,
        ?isAbstract: bool,
        ?isAsync: bool,
        ?isPartial: bool,
        ?isSealed: bool ) =
            
        Modifiers.Evaluate (modifiers.OfType<IModifierWord>().ToArray(), 
            ?staticOrInstance = staticOrInstance, 
            ?isAbstract = isAbstract, 
            ?isAsync = isAsync, 
            ?isPartial = isPartial, 
            ?isSealed = isSealed)
    
    static member Evaluate(
        modifiers: IMethodModifierWord[],
        ?staticOrInstance: StaticOrInstance,
        ?isAbstract: bool,
        ?isAsync: bool,
        ?isPartial: bool,
        ?isSealed: bool ) =
            
        Modifiers.Evaluate (modifiers.OfType<IModifierWord>().ToArray(), 
            ?staticOrInstance = staticOrInstance, 
            ?isAbstract = isAbstract, 
            ?isAsync = isAsync, 
            ?isPartial = isPartial, 
            ?isSealed = isSealed)