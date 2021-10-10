module LanguageCSharpTests

open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open Generator.GeneralUtils
open Generator
open Generator.Language
open Generator.Tests.TestDataCSharp

type ``When working with language parts`` () =
    let cSharp = LanguageCSharp() :> ILanguage

    [<Fact>]
    member _.``Using is correct without alias`` () =
        let actual = cSharp.Using Using.ForTesting
        actual |> should equal "using System;"

    [<Fact>]
    member _.``Using is correct with alias`` () =
        let actual = cSharp.Using {  Using.ForTesting with Alias = Some "B"}
        actual |> should equal "using B = System;"

    [<Fact>]
    member _.``Namespace open is correct`` () =
        let actual = cSharp.NamespaceOpen Namespace.ForTesting
        actual |> should equal ["namespace MyNamespace";"{"]

    [<Fact>]
    member _.``Namespace close is correct`` () =
        let actual = cSharp.NamespaceClose Namespace.ForTesting
        actual |> should equal ["}"]

    [<Fact>]
    member _.``Class open is correct`` () =
        let actual = cSharp.ClassOpen Class.ForTesting
        actual |> should equal ["public class RonWeasley";"{"]

    [<Fact>]
    member _.``Class close is correct`` () =
        let actual = cSharp.ClassClose Class.ForTesting
        actual |> should equal ["}"]

    [<Fact>]
    member _.``Method open is correct`` () =
        let actual = cSharp.MethodOpen Method.ForTesting
        actual |> should equal ["public string MyMethod()";"{"]

    [<Fact>]
    member _.``Method close is correct`` () =
        let actual = cSharp.MethodClose Method.ForTesting
        actual |> should equal ["}"]

    [<Fact>]
    member _.``Property open is correct`` () =
        let actual = cSharp.PropertyOpen Property.ForTesting
        actual |> should equal ["public MyReturnType MyProperty";"{"]

    [<Fact>]
    member _.``Property close is correct`` () =
        let actual = cSharp.PropertyClose Property.ForTesting
        actual |> should equal ["}"]

    [<Fact>]
    member _.``If open is correct`` () =
        let actual = cSharp.IfOpen If.ForTesting
        actual |> should equal ["if (A == 42)";"{"]

    [<Fact>]
    member _.``If close is correct`` () =
        let actual = cSharp.IfClose If.ForTesting
        actual |> should equal ["}"]

    [<Fact>]
    member _.``ForEach open is correct`` () =
        let actual = cSharp.ForEachOpen ForEach.ForTesting
        actual |> should equal ["for (var x in listOfThings)";"{"]

    [<Fact>]
    member _.``ForEach close is correct`` () =
        let actual = cSharp.ForEachClose ForEach.ForTesting
        actual |> should equal ["}"]

type ``When working with members`` () =
    let writer = RoslynWriter(LanguageCSharp(), 3)

    [<Fact>]
    member _.``If outputs correctly``() =
        let actual = writer.OutputIf If.ForTesting
        actual |> should equal ["public MyReturnType MyProperty {get; set;}"]

    [<Fact>]
    member _.``ForEach outputs correctly``() =
        let actual = writer.OutputForEach ForEach.ForTesting
        actual |> should equal ["public MyReturnType MyProperty {get; set;}"]

    [<Fact>]
    member _.``Assignment outputs correctly``() =
        let actual = writer.OutputAssignment Assignment.ForTesting
        actual |> should equal ["public MyReturnType MyProperty {get; set;}"]

    [<Fact>]
    member _.``AssignWithDeclare outputs correctly``() =
        let actual = writer.OutputAssignWithDeclare AssignWithDeclare.ForTesting
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
        let actual = writer.OutputProperty Property.ForTesting
        actual |> should equal ["public MyReturnType MyProperty {get; set;}"]

    [<Fact>]
    member _.``Expanded properties output correctly``() =
        let input = 
            { Property.ForTesting with 
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
            { Method.ForTesting with 
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
