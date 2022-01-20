module Generator.LanguageExpressions

open Generator
open Common
open Generator.LanguageModel
open System


type InvocationModel =
    { Instance: NamedItem // Named item for invoking static methods on generic types
      MethodName: NamedItem // For generic methods
      ShouldAwait: bool
      Arguments: IExpression list}
    interface IExpression
    interface IStatement
    static member Create instance methodName arguments =
        { Instance = instance // Named item for invoking static methods on generic types
          MethodName = methodName // For generic methods
          ShouldAwait = false
          Arguments = arguments }

type InstantiationModel =
    { TypeName: NamedItem
      Arguments: IExpression list}
    interface IExpression
    interface IStatement
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
    interface ICompareExpression

// KAD-Chet: This results in weird syntax around usage. Is there a better way (do FindRef to see)
type Symbol=
    | Symbol of s: string

type LiteralsModel =
    | StringLiteral of s: String
    | IntegerLiteral of i: int
    | DoubleLiteral of d: Double
    | SymbolLiteral of s: Symbol
    | NullLiteral
    | ThisLiteral
    | UnknownLiteral of x: obj
    interface IExpression

type CompareLiteralsModel =
    | TrueLiteral
    | FalseLiteral
    interface ICompareExpression


module ExpressionHelpers =
    // KAD: Do you see another way to differentiate this from the statement Invoke? Reconsider once E@E is working
    //           I wonder which is more common in C#. If we had Invoke and Invocation, which 
    //           would be the statement and which the expression?
    let InvokeExpression
        (instance: NamedItem)
        (methodToCall: NamedItem)
        (args: IExpression list) =

        InvocationModel.Create instance methodToCall args

    let New
        (typeName: NamedItem)
        (args: IExpression list) =

        InstantiationModel.Create typeName  args

    let Compare
        (left: IExpression)
        (operator: Operator)
        (right: IExpression) =
        
        ComparisonModel.Create left right operator
