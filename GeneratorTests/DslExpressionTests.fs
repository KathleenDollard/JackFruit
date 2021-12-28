module DslExpressionTests


open Xunit
open Generator.Language
open Common
open Generator.LanguageExpressions
open Generator.LanguageExpressions.ExpressionHelpers


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
        let left = OtherLiteralModel.Create "42"
        let right = OtherLiteralModel.Create "1"
        let expectedModel = 
            { Left = left
              Right = right
              Operator = NotEquals }

        let actualModel = Compare left NotEquals right

        Assert.Equal(expectedModel, actualModel)

    [<Fact>]
    member _.``Can create a String Literal``() =
        let value = "George"
        let expectedModel: IExpression = 
            { StringLiteralModel.Value = value }

        let actualModel = Literal value

        Assert.Equal(expectedModel, actualModel)

    [<Fact>]
    member _.``Can create a non-String Literal``() =
        let value = 42
        let stringValue = "42"
        let expectedModel: IExpression = 
            { OtherLiteralModel.Value = stringValue }

        let actualModel = Literal value

        Assert.Equal(expectedModel, actualModel)


    [<Fact>]
    member _.``Can create a Symbol``() =
        let name = "George"
        let expectedModel = 
            { SymbolModel.Name = name }

        let actualModel = Symbol name

        Assert.Equal(expectedModel, actualModel)


    [<Fact>]
    member _.``Can create a Null``() =
        let value = "George"
        let expectedModel = NullModel()

        let actualModel = Null

        Assert.Equal(expectedModel, actualModel)


    [<Fact>]
    member _.``New test``() =
        ()

