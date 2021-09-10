module CSharpTestCodeTests

    open Xunit
    open FsUnit.Xunit

    [<Fact>]
    let ``Can open file`` () =
        let text = System.IO.File.ReadAllText "..\..\..\TestHandlers.cs"
        
        text.Length |> should be (greaterThan 100)