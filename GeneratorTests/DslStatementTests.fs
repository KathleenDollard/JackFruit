module DslStatementTests

open Xunit
open Common
open Generator.LanguageModel
open DslKeywords
open DslForCode
open Generator.LanguageModel
open Generator.LanguageExpressions
open Generator.LanguageExpressions.ExpressionHelpers
open Generator.LanguageHelpers
open Generator.LanguageHelpers.Statements
open type Generator.LanguageHelpers.Structural
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
            Method(outerMethodName) {
                ReturnVoid()
                }

        checkResult expectedModel (codeModel.Model:?> MethodModel)
  
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
            Method(outerMethodName) {
                Return (InvokeExpression className methodName [])
                }
  
        checkResult (Some expectedModel) (codeModel.Model:?> MethodModel)

    [<Fact>]
    member _.``Can create new/instantiation return``() =
        let outerMethodName = "B"
        let className = "A"
        let expectedModel: IExpression = 
            { TypeName = NamedItem.Create (className, [])
              Arguments = [] }
        let codeModel = 
            Method(outerMethodName) {
                Return (New className [])
                }
  
        checkResult (Some expectedModel) (codeModel.Model:?> MethodModel)

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
            Method(outerMethodName) {
                Return (Compare (Literal left) GreaterThan (Literal right))
                }
  
        checkResult (Some expectedModel) (codeModel.Model:?> MethodModel)

    [<Fact>]
    member _.``Can create string literal return``() =
        let outerMethodName = "A"
        let literal = "\"Fred\""
        let expectedModel: IExpression = StringLiteral literal[1..literal.Length - 2]
        let codeModel = 
            Method(outerMethodName) {
                Return (Literal literal)
                }
  
        checkResult (Some expectedModel) (codeModel.Model:?> MethodModel)
  
    [<Fact>]
    member _.``Can create non string literal return``() =
        let outerMethodName = "A"
        let literal = 42
        let expectedModel: IExpression = IntegerLiteral 42
        let codeModel = 
            Method(outerMethodName) {
                Return (Literal literal)
                }
  
        checkResult (Some expectedModel)  (codeModel.Model:?> MethodModel)

    [<Fact>]
    member _.``Can create symbol return``() =
        let outerMethodName = "A"
        let name = "Fred"
        let expectedModel: IExpression = SymbolLiteral (Symbol name)
        let codeModel = 
            Method(outerMethodName) {
                Return (SymbolLiteral (Symbol name))
                }
  
        checkResult (Some expectedModel) (codeModel.Model:?> MethodModel)
     
    [<Fact>]
    member _.``Can create null return``() =
        let outerMethodName = "A"
        let expectedModel: IExpression = NullLiteral
        let codeModel = 
            Method(outerMethodName) {
                Return NullLiteral
                }
  
        checkResult (Some expectedModel) (codeModel.Model:?> MethodModel)

type ``When creating If statements``() =

    [<Fact>]
    member _.``Can  create If without Else``() =
        let returnIfTrue = "Fred"
        let returnStatement = { ReturnModel.Expression = Some (Literal returnIfTrue) }
        let expectedModel = { IfCondition = TrueLiteral; Statements = [ returnStatement ] }
        let actualModel = 
            If (TrueLiteral) {
                Return (Literal returnIfTrue) }

        Assert.Equal(expectedModel, (actualModel.Model :?> IfModel))

    
    [<Fact>]
    member _.``Can  create if with Else``() =
        let outerMethodName = "A"
        let returnIfTrue = "Fred"
        let returnStatementIfTrue = { ReturnModel.Expression = Some (Literal returnIfTrue) }
        let returnIfFalse = "George"
        let returnStatementIfFalse = { ReturnModel.Expression = Some (Literal returnIfFalse) }
        let expectedModelIfTrue= { IfCondition = TrueLiteral; Statements = [ returnStatementIfTrue ] }
        let expectedModelIfFalse= { ElseStatements = [ returnStatementIfFalse ] }
        let codeModel = 
            Method(outerMethodName) {
                If (TrueLiteral) {
                    Return (Literal returnIfTrue) }
                Else() {
                    Return (Literal returnIfFalse) }
                }           

        let actualModelIfTrue = (codeModel.Model :?> MethodModel).Statements[0] :?> IfModel
        let actualModelIfFalse =  (codeModel.Model :?> MethodModel).Statements[1] :?> ElseModel
        Assert.Equal(expectedModelIfTrue, actualModelIfTrue)
        Assert.Equal(expectedModelIfFalse, actualModelIfFalse)

    
    [<Fact>]
    member _.``Can  create if without ElseIf``() =
        let outerMethodName = "A"
        let returnIfTrue = "Fred"
        let returnStatementIfTrue = { ReturnModel.Expression = Some (Literal returnIfTrue) }
        let expectedModelIfTrue= { IfCondition = TrueLiteral; Statements = [ returnStatementIfTrue ] }
        let codeModel = 
            Method(outerMethodName) {
                If (TrueLiteral) {
                    Return (Literal returnIfTrue) }
                }           

        let actualModelIfTrue =  (codeModel.Model :?> MethodModel).Statements[0] :?> IfModel
        Assert.Equal(expectedModelIfTrue, actualModelIfTrue)
        
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
            Method(outerMethodName) {
                SimpleCall (InvocationModel.Create instance method [])
                }

        let statements =  (codeModel.Model :?> MethodModel).Statements
        Assert.Equal(1, statements.Length)
        match statements[0] with 
        | :? SimpleCallModel as actualCall -> Assert.Equal(expected, actualCall.Expression)
        | _ -> Assert.Fail("Statement not of expected SimpleCallModel type")
  
type ``When creating Invoke statements``() =
    [<Fact>]
    member _.``Can  create the simplest statement``() =
        let outerMethodName = "A"
        let instance = "B"
        let method = "C"
        let expected: IStatement = (InvocationModel.Create instance method [])
        let codeModel = 
            Method(outerMethodName) {
                Invoke instance method []
                }
        let statements =  (codeModel.Model :?> MethodModel).Statements
        Assert.Equal(1, statements.Length)
        Assert.Equal(expected, statements[0])

type ``When creating Comment statements``() =
    [<Fact>]
    member _.``Can  create the simplest statement``() =
        ()

type ``When creating CompilerDirective statements``() =
    [<Fact>]
    member _.``Can  create the simplest statement``() =
        ()


