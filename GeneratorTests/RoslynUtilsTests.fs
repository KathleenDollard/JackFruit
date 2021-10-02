namespace GeneratorTests

open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open Generator.RoslynUtils
open Microsoft.CodeAnalysis

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

