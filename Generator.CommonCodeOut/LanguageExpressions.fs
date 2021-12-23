module Generator.LanguageExpressions

open Generator
open Common
open Generator.Language

type InvocationModel =
    { Instance: NamedItem // Named item for invoking static methods on generic types
      MethodName: NamedItem // For generic methods
      ShouldAwait: bool
      Arguments: IExpression list}
    interface IExpression
    static member Create instance methodName arguments =
        { Instance = instance // Named item for invoking static methods on generic types
          MethodName = methodName // For generic methods
          ShouldAwait = false
          Arguments = arguments }

type InstantiationModel =
    { TypeName: NamedItem
      Arguments: IExpression list}
    interface IExpression
    static member Create typeName arguments =
        { TypeName = typeName
          Arguments = arguments}

type ComparisonModel =
    { Left: IExpression
      Right: IExpression
      Operator: Operator}
    interface IExpression
    static member Create left right operator =
        { Left = left
          Right = right
          Operator = operator }

type StringLiteralModel =
    { Expression: string}
    interface IExpression
    static member Create text =
        { Expression = text }

type NonStringLiteralModel =
    { Expression: string}
    interface IExpression
    static member Create text =
        { Expression = text }

type SymbolModel =
    { Expression: string}
    interface IExpression
    static member Create text =
        { Expression = text }

type CommentModel =
    { Expression: string}
    interface IExpression
    static member Create text =
        { Expression = text }

type PragmaModel =
    { Expression: string}
    interface IExpression
    static member Create text =
        { Expression = text }

// KAD-Chet: This seems pretty dumb, but I need something that implements the interface.
type NullModel =
    { Dummy: string} // not sure how to manage this
    interface IExpression
    static member Create = { Dummy = "" }


