module TestUtilsTests

open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open TestUtils
open Microsoft.CodeAnalysis.CSharp
open CSharpTestCode
open GeneratorSketch.Generator


type ``When using utility functions``() =


    [<Fact>]
    member __.``SemanticModel can be retrieved``() =
        let source =
            @"
        using System;

        public static class Program
        {
           public static void Main()
           {
           }
        }"

        let actual =
            getSemanticModelFromSource (Code source) []

        actual |> should be (ofCase <@ Ok @>)

    [<Fact>]
    member __.``SemanticModel not Ok when compilation errors present``() =
        let source =
            @"
        using System;

        public static class Program
        {
           public static void A()
           {
               var x = new B(); // B does not exist in the compilation
           }
        }"

        let actual =
            getSemanticModelFromSource (Code source) []

        let concatErrors errors =
            String.concat
                "\n\r"
                [ for error in errors do
                      error.ToString() ]
        actual |> should be (ofCase <@ Ok @>)

    [<Fact>]
    member __.``SemanticModel not Ok when syntax errors present``() =
        let source =
            @"
        using System;

        publi static class Program
        {
           // No void Main
           public static void A()
           {
           }
        }"

        let actual =
            getSemanticModelFromSource (Code source) []

        let concatErrors errors =
            String.concat
                "\n\r"
                [ for error in errors do
                      error.ToString() ]
        actual |> should be (ofCase <@ Ok @>)

    [<Fact>]
    member __.``Semantic model for single tree is not null``() =
        let tree =
            CSharpSyntaxTree.ParseText(handlerSource)

        let modelResult = getSemanticModelFromFirstTree [ tree ]

        Assert.True(TestUtils.isResultOk modelResult)

    [<Fact>]
    member __.``Semantic model for single source is not null``() =
        let modelResult =
            getSemanticModelFromSource (Code handlerSource) []

        Assert.True(TestUtils.isResultOk modelResult)

    // This test may become obsolete in the cotnext of this project in the future
    [<Fact>]
    member __.``Semantic model for local source is found``() =
        let modelResult =
            getSemanticModelFromSource (Code oneMapping) [ (Code handlerSource) ]

        Assert.True(TestUtils.isModelOk modelResult)
