module DslStatementTests

open Xunit
open Generator.Language
open Common
open DslKeywords
open DslCodeBuilder
open Generator.LanguageExpressions
open Generator.LanguageExpressions.ExpressionHelpers
open Generator.LanguageStatements


type ``When creating Return statements to test all expressions``() =
    let checkResult (expectedModel: IExpression option) (codeModel: MethodModel)  = 
        Assert.Equal(1, codeModel.Statements.Length)
        let actualStatement = codeModel.Statements[0]
        let actualModel =
            match actualStatement with 
            | :? ReturnModel as x -> x.Expression
            | _ -> invalidOp "Unexpected statement in codeModel"
        Assert.Equal(expectedModel, actualModel)

    [<Fact>]
    member _.``Can  create Void/empty return``() =
        let outerMethodName = "A"
        let expectedModel = None
        let codeModel = 
            Method(SimpleNamedItem outerMethodName, Void) {
                Public
                Return
                }

        checkResult expectedModel codeModel
  
    [<Fact>]
    member _.``Can create invocation return``() =
        let outerMethodName = "C"
        let className = "A"
        let methodName = "B"
        let expectedModel: IExpression = 
            { InvocationModel.Instance = className
              MethodName = methodName
              ShouldAwait = false
              Arguments = [] }
        let codeModel = 
            Method(SimpleNamedItem outerMethodName, Void) {
                Return (InvokeExpression className methodName [])
                }
  
        checkResult (Some expectedModel) codeModel

    [<Fact>]
    member _.``Can create new/instantiation return``() =
        let outerMethodName = "B"
        let className = "A"
        let expectedModel: IExpression = 
            { TypeName = NamedItem.Create className []
              Arguments = [] }
        let codeModel = 
            Method(SimpleNamedItem outerMethodName, Void) {
                Return (New className [])
                }
  
        checkResult (Some expectedModel) codeModel

    [<Fact>]
    member _.``Can create comparison return``() =
        let outerMethodName = "A"
        let left = "Abc"
        let right = "Def"
        let operator = GreaterThan
        let expectedModel: IExpression = 
            { Left = Literal left
              Right = Literal right
              Operator = operator }
        let codeModel = 
            Method(SimpleNamedItem outerMethodName, Void) {
                Return (Compare (Literal left) GreaterThan (Literal right))
                }
  
        checkResult (Some expectedModel) codeModel

    [<Fact>]
    member _.``Can create string literal return``() =
        let outerMethodName = "A"
        let literal = "Fred"
        let expectedModel: IExpression = (StringLiteralModel.Create literal)
        let codeModel = 
            Method(SimpleNamedItem outerMethodName, Void) {
                Return (Literal literal)
                }
  
        checkResult (Some expectedModel) codeModel
  
    [<Fact>]
    member _.``Can create non string literal return``() =
        let outerMethodName = "A"
        let literal = 42
        let expectedModel: IExpression = (OtherLiteralModel.Create "42")
        let codeModel = 
            Method(SimpleNamedItem outerMethodName, Void) {
                Return (Literal literal)
                }
  
        checkResult (Some expectedModel) codeModel

    [<Fact>]
    member _.``Can create symbol return``() =
        let outerMethodName = "A"
        let name = "Fred"
        let expectedModel: IExpression = (SymbolModel.Create name)
        let codeModel = 
            Method(SimpleNamedItem outerMethodName, Void) {
                Return (Symbol name)
                }
  
        checkResult (Some expectedModel) codeModel
     
    [<Fact>]
    member _.``Can create null return``() =
        let outerMethodName = "A"
        let expectedModel: IExpression = NullModel.Create()
        let codeModel = 
            Method(SimpleNamedItem outerMethodName, Void) {
                Return Null
                }
  
        checkResult (Some expectedModel) codeModel

type ``When creating If statements``() =

    [<Fact>]
    member _.``Can  create if without else``() =
        let outerMethodName = "A"
        let returnIfTrue = "Fred"
        let codeModel = 
            Method(SimpleNamedItem outerMethodName, Void) {
                If (True) {
                    Return returnIfTrue }

                }

        Assert.Equal(1, codeModel.Statements.Length)
        match codeModel.Statements[0] with 
        | :? ReturnModel as exp -> 
            match exp.Expression with 
            | None -> ()
            | Some x -> Assert.Fail("Return is not void return")
        | _ -> Assert.Fail("Statement not of expected ReturnModel type")   
        
type ``When creating ForEach statements``() =
    [<Fact>]
    member _.``Can  create the simplest statement``() =
        ()
    
type ``When creating Assignment statements``() =
    [<Fact>]
    member _.``Can  create the simplest statement``() =
        ()   
    
type ``When creating AssignWithDeclare statements``() =
    [<Fact>]
    member _.``Can  create the simplest statement``() =
        ()
       
type ``When creating SimpleCall statements``() =
    [<Fact>]
    member _.``Can  create the simplest statement``() =
        let outerMethodName = "A"
        let instance = "B"
        let method = "C"
        let expected: IExpression = (InvocationModel.Create instance method [])
        let codeModel = 
            Method(outerMethodName, Void) {
                Public
                SimpleCall (InvocationModel.Create instance method [])
                }

        Assert.Equal(1, codeModel.Statements.Length)
        match codeModel.Statements[0] with 
        | :? SimpleCallModel as actualCall -> Assert.Equal(expected, actualCall.Expression)
        | _ -> Assert.Fail("Statement not of expected SimpleCallModel type")
  
type ``When creating Invoke statements``() =
    [<Fact>]
    member _.``Can  create the simplest statement``() =
        let outerMethodName = "A"
        let instance = "B"
        let method = "C"
        let expected: IExpression = (InvocationModel.Create instance method [])
        let codeModel = 
            Method(outerMethodName, Void) {
                Public
                Invoke instance method
                }
        Assert.Equal(1, codeModel.Statements.Length)
        match codeModel.Statements[0] with 
        | :? SimpleCallModel as actualCall -> Assert.Equal(expected, actualCall.Expression)
        | _ -> Assert.Fail("Statement not of expected SimpleCallModel type")

type ``When creating Comment statements``() =
    [<Fact>]
    member _.``Can  create the simplest statement``() =
        ()

type ``When creating CompilerDirective statements``() =
    [<Fact>]
    member _.``Can  create the simplest statement``() =
        ()


