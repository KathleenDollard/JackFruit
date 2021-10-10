module LanguageCSharpTests

open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open Generator.GeneralUtils
open Generator
open Generator.Language
open Generator.Tests.TestData

type ``When working with language parts`` () =
    let cSharp = LanguageCSharp() :> ILanguage

    [<Fact>]
    member _.``Using is correct without alias`` () =
        let actual = cSharp.Using Using.ForTesting.Data
        actual |> should equal Using.ForTesting.CSharp

    [<Fact>]
    member _.``Using is correct with alias`` () =
        let actual = cSharp.Using {  Using.ForTesting.Data with Alias = Some "B"}
        actual |> should equal "using B = System;"

    [<Fact>]
    member _.``Namespace open is correct`` () =
        let actual = cSharp.NamespaceOpen Namespace.ForTesting.Data
        actual |> should equal Namespace.ForTesting.CSharpOpen

    [<Fact>]
    member _.``Namespace close is correct`` () =
        let actual = cSharp.NamespaceClose Namespace.ForTesting.Data
        actual |> should equal Namespace.ForTesting.CSharpClose

    [<Fact>]
    member _.``Class open is correct`` () =
        let actual = cSharp.ClassOpen Class.ForTesting.Data
        actual |> should equal Class.ForTesting.CSharpOpen

    [<Fact>]
    member _.``Class close is correct`` () =
        let actual = cSharp.ClassClose Class.ForTesting.Data
        actual |> should equal Class.ForTesting.CSharpClose

    [<Fact>]
    member _.``Method open is correct`` () =
        let actual = cSharp.MethodOpen Method.ForTesting.Data
        actual |> should equal Method.ForTesting.CSharpOpen

    [<Fact>]
    member _.``Method close is correct`` () =
        let actual = cSharp.MethodClose Method.ForTesting.Data
        actual |> should equal Method.ForTesting.CSharpClose

    [<Fact>]
    member _.``Property open is correct`` () =
        let actual = cSharp.PropertyOpen Property.ForTesting.Data
        actual |> should equal Property.ForTesting.CSharpOpen

    [<Fact>]
    member _.``Property close is correct`` () =
        let actual = cSharp.PropertyClose Property.ForTesting.Data
        actual |> should equal Property.ForTesting.CSharpClose

    [<Fact>]
    member _.``If open is correct`` () =
        let actual = cSharp.IfOpen If.ForTesting.Data
        actual |> should equal If.ForTesting.CSharpOpen

    [<Fact>]
    member _.``If close is correct`` () =
        let actual = cSharp.IfClose If.ForTesting.Data
        actual |> should equal If.ForTesting.CSharpClose

    [<Fact>]
    member _.``ForEach open is correct`` () =
        let actual = cSharp.ForEachOpen ForEach.ForTesting.Data
        actual |> should equal ForEach.ForTesting.CSharpOpen

    [<Fact>]
    member _.``ForEach close is correct`` () =
        let actual = cSharp.ForEachClose ForEach.ForTesting.Data
        actual |> should equal ForEach.ForTesting.CSharpClose

type ``When working with members`` () =
    let writer = RoslynOut(LanguageCSharp(), ArrayWriter(3))

    [<Fact>]
    member _.``If outputs correctly``() =
        let actual = writer.OutputIf If.ForTesting.Data
        let expected = 
            If.ForTesting.CSharpOpen
            |> addLines If.ForTesting.CSharpBlock
            |> addLines If.ForTesting.CSharpClose
        actual |> should equal expected

    [<Fact>]
    member _.``ForEach outputs correctly``() =
        let actual = writer.OutputForEach ForEach.ForTesting.Data
        actual |> should equal ["public MyReturnType MyProperty {get; set;}"]

    [<Fact>]
    member _.``Assignment outputs correctly``() =
        let actual = writer.OutputAssignment Assignment.ForTesting.Data
        actual |> should equal ["public MyReturnType MyProperty {get; set;}"]

    [<Fact>]
    member _.``AssignWithDeclare outputs correctly``() =
        let actual = writer.OutputAssignWithDeclare AssignWithDeclare.ForTesting.Data
        actual |> should equal ["public MyReturnType MyProperty {get; set;}"]

    [<Fact>]
    member _.``Return outputs correctly``() =
        let actual = writer.OutputReturn (NonStringLiteral "42")
        actual |> should equal ["public MyReturnType MyProperty {get; set;}"]

    [<Fact>]
    member _.``SimpleCall outputs correctly``() =
        let actual = writer.OutputSimpleCall (StringLiteral "Harry")
        actual |> should equal ["public MyReturnType MyProperty {get; set;}"]


    [<Fact>]
    member _.``Auto-properties output correctly``() =
        let actual = writer.OutputProperty Property.ForTesting.Data
        actual |> should equal ["public MyReturnType MyProperty {get; set;}"]

    [<Fact>]
    member _.``Expanded properties output correctly``() =
        let input = 
            { Property.ForTesting.Data with 
                GetStatements = [Return (Symbol "x")]
                SetStatements = [Assignment { Item = "value"; Value = (Symbol "x")}] }
        let actual = writer.OutputProperty input
        actual |> should equal [
            "public MyReturnType MyProperty"
            "{"
            "get"
            "{";
             "return x;"
            "}"
            "set"
            "{";
             "value = x;"
            "}"
            "}"]

    [<Fact>]
    member _.``Void method outputs correctly`` () =
        let input = 
            { Method.ForTesting.Data with 
                Statements = 
                    [ AssignWithDeclare { Item = "x"; TypeName = None; Value = (NonStringLiteral "42")}
                      SimpleCall (Invocation { Instance = (GenericNamedItem.Create "Console"); MethodName = "WriteLine"; Arguments = [] }) ] }
        let actual = writer.OutputMethod input
        actual |> should equal [
            "public string MyMethod()"
            "{"
            "var x = 42;"
            "Console.WriteLine();"
            "}"]
