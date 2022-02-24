namespace GeneratorTests

open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open Generator.RoslynUtils
open Microsoft.CodeAnalysis
open Generator.Tests.UtilsForTests
open System.Linq
open Microsoft.CodeAnalysis.CSharp.Syntax
open Generator

type ``When building a SyntaxTree``() =

    [<Fact>]
    member _.``Valid C# code creates tree``() =
        let input = @"
        public class A
        {
            public void B() {}
        }"

        let actual = SyntaxTreeResult (CSharpCode input)

        match actual with
        | Ok _ -> ()
        | Error e -> e |> should be Empty

        
    [<Fact>]
    member _.``Invalid C# code creates diagnostics``() =
        let input = @"
        public clss A
        {
            public void B() {}
        }"

        let actual = SyntaxTreeResult (VBCode input)

        match actual with
        | Ok _ -> ()
        | Error e -> e |> should not' (be Empty)

    [<Fact>]
    member _.``Valid VB code creates tree``() =
        let input = @"
        Public Class A
            Public Sub B() 
            End Sub
        End Class"

        let actual = SyntaxTreeResult (VBCode input)

        match actual with
        | Ok _ -> ()
        | Error e -> e |> should be Empty

    [<Fact>]
    member _.``Invalid VB code creates diagnostics``() =
        let input = @"
        Public Clss A
            Public Sub B()
            End Sub
        End Class"

        let actual = SyntaxTreeResult (CSharpCode input)

        match actual with
        | Ok _ -> ()
        | Error e -> e |> should not' (be Empty)

    [<Fact>]
    member _.``Namespace found for method invocation``() =
        let input = @"
        namespace My.Namespace
        {
            public class A
            {
                public void B() {}
            }
        }
        namespace My
        {
            namespace Other.Namespace
            {
                public class A2
                {
                    public void C() 
                    { 
                        var a = new My.Namespace.A();
                        a.B(); 
                    }
                }
            }
        }"

        let treeResult = SyntaxTreeResult (CSharpCode input)
        let nspaceName = 
            match treeResult with
            | Ok tree ->
                let semanticModelResult = GetSemanticModelFromFirstTree([tree])
                let semanticModel =
                    match semanticModelResult with 
                    | Ok model -> model
                    | _ -> invalidOp "Invalid input code model"
                let method = tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>().First()
                let evalLanguage = EvalCSharp()
                evalLanguage.NamespaceFromdDescendant method semanticModel
            | Error e -> invalidOp $"Invalid input code{e}"

        Assert.Equal("My.Other.Namespace", nspaceName)


