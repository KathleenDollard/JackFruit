module DslStatementTests

open Xunit
open Generator.Language
open Common
open DslKeywords
open DslCodeBuilder
open Generator.LanguageExpressions
open Generator.LanguageStatements


type ``When creating Return statements``() =
    [<Fact>]
    member _.``Can create void return``() =
        let methodName = "A"
        let codeModel = 
            Method(SimpleNamedItem methodName, Void) {
                Public
                Return
                }

        Assert.Equal(1, codeModel.Statements.Length)
        match codeModel.Statements[0] with 
        | :? ReturnModel as exp -> 
            match exp.Expression with 
            | None -> ()
            | Some x -> Assert.Fail("Return is not void return")
        | _ -> Assert.Fail("Statement not of expected ReturnModel type")

    [<Fact>]
    member _.``Can create string return``() =
        let methodName = "A"
        let expected: IExpression = (StringLiteralModel.Create "Fred")
        let codeModel = 
            Method(SimpleNamedItem methodName, Void) {
                Return "Fred"
                }

        Assert.Equal(1, codeModel.Statements.Length)
        match codeModel.Statements[0] with 
        | :? ReturnModel as exp -> 
            match exp.Expression with 
            | None -> Assert.Fail("Return expression was expected and not found")
            | Some x -> Assert.Equal(expected, x)
        | _ -> Assert.Fail("Statement not of expected ReturnModel type")

    [<Fact>]
    member _.``Can create other literal return``() =
        let methodName = "A"
        let expected: IExpression = (NonStringLiteralModel.Create "42")
        let codeModel = 
            Method(SimpleNamedItem methodName, Void) {
                Return 42
                }

        Assert.Equal(1, codeModel.Statements.Length)
        match codeModel.Statements[0] with 
        | :? ReturnModel as exp -> 
            match exp.Expression with 
            | None -> Assert.Fail("Return expression was expected and not found")
            | Some x -> Assert.Equal(expected, x)
        | _ -> Assert.Fail("Statement not of expected ReturnModel type")
    
    [<Fact>]
    member _.``Can create expression return``() =
        let methodName = "A"
        let expected: IExpression = NullModel.Create
        let codeModel = 
            Method(SimpleNamedItem methodName, Void) {
                Return NullModel.Create
                }

        Assert.Equal(1, codeModel.Statements.Length)
        match codeModel.Statements[0] with 
        | :? ReturnModel as exp -> 
            match exp.Expression with 
            | None -> Assert.Fail("Return expression was expected and not found")
            | Some x -> Assert.Equal(expected, x)
        | _ -> Assert.Fail("Statement not of expected ReturnModel type")

        
type ``When creating SimpleCall statements``() =
    [<Fact>]
    member _.``Can create an invocation``() =
        let methodName = "A"
        let instance = "B"
        let method = "C"
        let expected: IExpression = (InvocationModel.Create instance method [])
        let codeModel = 
            Method(methodName, Void) {
                Public
                SimpleCall (InvocationModel.Create instance method [])
                }

        Assert.Equal(1, codeModel.Statements.Length)
        match codeModel.Statements[0] with 
        | :? SimpleCallModel as actualCall -> Assert.Equal(expected, actualCall.Expression)
        | _ -> Assert.Fail("Statement not of expected SimpleCallModel type")