module DslExpressionTests


open Xunit
open Generator.Language
open Common
open DslKeywords
open DslCodeBuilder
open Generator.LanguageExpressions
open Generator.LanguageStatements


type ``When creating Invocation expressions``() =
    [<Fact>]
    member _.``Can create a SimpleCall``() =
        let methodName = "A"
        let instance = "B"
        let method = "C"
        let expected: IExpression = (InvocationModel.Create instance method [])
        let codeModel = 
            Method(methodName, Void) {
                Public
                SimpleCall (InvocationModel.Create instance method [])
                // We may want to make Invoke both a statement and a expression. I'm not sure what
                // other expressions can also be statements, thus SimpleCall and Invoke may collapse
                // together
                //SimpleCall Invoke
                }

        Assert.Equal(1, codeModel.Statements.Length)
        match codeModel.Statements[0] with 
        | :? SimpleCallModel as actualCall -> Assert.Equal(expected, actualCall.Expression)
        | _ -> Assert.Fail("Statement not of expected SimpleCallModel type")

        //type InvocationModel =
        //    { Instance: NamedItem // Named item for invoking static methods on generic types
        //      MethodName: NamedItem // For generic methods
        //      ShouldAwait: bool
        //      Arguments: IExpression list}
        //    interface IExpression
        //    static member Create instance methodName arguments =
        //        { Instance = instance // Named item for invoking static methods on generic types
        //          MethodName = methodName // For generic methods
        //          ShouldAwait = false
        //          Arguments = arguments }
        
        //type InstantiationModel =
        //    { TypeName: NamedItem
        //      Arguments: IExpression list}
        //    interface IExpression
        //    static member Create typeName arguments =
        //        { TypeName = typeName
        //          Arguments = arguments}
