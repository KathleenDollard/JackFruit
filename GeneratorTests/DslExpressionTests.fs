module DslExpressionTests


open Xunit
open Generator.Language
open Common
open Generator.LanguageExpressions
open Generator.LanguageExpressions.ExpressionHelpers
open Generator.LanguageHelpers


type ``When creating expressions``() =
    [<Fact>]
    member _.``Can create an Invocation``() =
        let instance = "B"
        let method = "C"
        let expectedModel = 
            { Instance = instance
              MethodName = method
              ShouldAwait = false
              Arguments = [] }

        let actualModel = InvokeExpression instance method []

        Assert.Equal(expectedModel, actualModel)
    

    [<Fact>]
    member _.``Can create an New/Instantiation``() =
        let className = "B"
        let expectedModel = 
            { TypeName = className
              Arguments = []}

        let actualModel = New className []

        Assert.Equal(expectedModel, actualModel)

    [<Fact>]
    member _.``Can create a Comparison``() =
        let left = Literal"42"
        let right = Literal "1"
        let expectedModel = 
            { Left = left
              Right = right
              Operator = NotEquals }

        let actualModel = Compare left NotEquals right

        Assert.Equal(expectedModel, actualModel)

    [<Fact>]
    member _.``Can create a String Literal``() =
        let value = "\"George\""
        let expectedModel: IExpression = StringLiteral value[1..value.Length - 2]

        let actualModel = Literal value

        Assert.Equal(expectedModel, actualModel)

    [<Fact>]
    member _.``Can create a non-String Literal``() =
        let value = 42
        let stringValue = "42"
        let expectedModel: IExpression = Literal stringValue

        let actualModel = Literal value

        Assert.Equal(expectedModel, actualModel)


    [<Fact>]
    member _.``Can create a Symbol``() =
        // I can't figure out a reasonable way to test this
        let name = "George"
        let expectedModel = Symbol name

        let actualModel = Symbol name

        Assert.Equal(expectedModel, actualModel)


    [<Fact>]
    member _.``Can create a Null``() =
         // I can't figure out a reasonable way to test this
        let value = "George"
        let expectedModel = NullLiteral

        let actualModel = NullLiteral

        Assert.Equal(expectedModel, actualModel)


    [<Fact>]
    member _.``New test``() =
        ()

