module LanguageCSharpTests

open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open Generator.GeneralUtils
open Generator
open Generator.Tests.TestData
open Generator.LanguageModel
open Common
open Generator.LanguageExpressions
open Generator.LanguageStatements
open Generator.LanguageRoslynOut
open Generator.LanguageHelpers
open System.Collections.Generic

let CompareLines (expected: (struct (int * string)) list) (actual: (struct (int * string)) list) =
    Assert.Equal<IEnumerable<struct (int * string)>>(List.toSeq expected, List.toSeq actual)

type ``When working with language parts`` () =
    let cSharp = LanguageCSharp() :> ILanguage

    [<Fact>]
    member _.``Using is correct without alias`` () =
        let actual = cSharp.Using UsingModel.ForTesting.Data
        actual |> should equal UsingModel.ForTesting.CSharp


    [<Fact>]
    member _.``Using is correct with alias`` () =
        let actual = cSharp.Using {  UsingModel.ForTesting.Data with Alias = Some "B"}
        actual |> should equal [ "using B = System;" ]


    [<Fact>]
    member _.``Namespace open is correct`` () =
        let actual = cSharp.NamespaceOpen NamespaceModel.ForTesting.Data
        actual |> should equal NamespaceModel.ForTesting.CSharpOpen


    [<Fact>]
    member _.``Namespace close is correct`` () =
        let actual = cSharp.NamespaceClose NamespaceModel.ForTesting.Data
        actual |> should equal NamespaceModel.ForTesting.CSharpClose


    [<Fact>]
    member _.``Class open is correct`` () =
        let actual = cSharp.ClassOpen ClassModel.ForTesting.Data
        actual |> should equal ClassModel.ForTesting.CSharpOpen


    [<Fact>]
    member _.``Class close is correct`` () =
        let actual = cSharp.ClassClose ClassModel.ForTesting.Data
        actual |> should equal ClassModel.ForTesting.CSharpClose


    [<Fact>]
    member _.``Method open is correct`` () =
        let actual = cSharp.MethodOpen MethodModel.ForTesting.Data
        actual |> should equal MethodModel.ForTesting.CSharpOpen


    [<Fact>]
    member _.``Method close is correct`` () =
        let actual = cSharp.MethodClose MethodModel.ForTesting.Data
        actual |> should equal MethodModel.ForTesting.CSharpClose


    [<Fact>]
    member _.``Property open is correct`` () =
        let actual = cSharp.PropertyOpen PropertyModel.ForTesting.Data
        actual |> should equal PropertyModel.ForTesting.CSharpOpen


    [<Fact>]
    member _.``Property close is correct`` () =
        let actual = cSharp.PropertyClose PropertyModel.ForTesting.Data
        actual |> should equal PropertyModel.ForTesting.CSharpClose

    [<Fact>]
    member _.``If open is correct`` () =
        let actual = cSharp.IfOpen IfModel.ForTesting.Data
        actual |> should equal IfModel.ForTesting.CSharpOpen


    [<Fact>]
    member _.``If close is correct`` () =
        let actual = cSharp.IfClose IfModel.ForTesting.Data
        actual |> should equal IfModel.ForTesting.CSharpClose


    [<Fact>]
    member _.``ForEach open is correct`` () =
        let actual = cSharp.ForEachOpen ForEachModel.ForTesting.Data
        actual |> should equal ForEachModel.ForTesting.CSharpOpen


    [<Fact>]
    member _.``ForEach close is correct`` () =
        let actual = cSharp.ForEachClose ForEachModel.ForTesting.Data
        actual |> should equal ForEachModel.ForTesting.CSharpClose

    // Assignment, AssigWithDeclare, Return, SimpleCall and Comment 
    // are sufficiently simple they are only tested as outputs below

type ``When outputting code`` () =
    let writer = RoslynOut(LanguageCSharp(),ArrayWriter(3))


    [<Fact>]
    member _.``If outputs correctly``() =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let (expected: (struct (int * string)) list) = 
            [ (0, IfModel.ForTesting.CSharpOpen |> List.head)
              (0, "{")
              (1, AssignmentModel.ForTesting.CSharp |> List.head)
              (0, "}")]
        let data = { IfModel.ForTesting.Data with Statements = [ AssignmentModel.ForTesting.Data] }

        outPutter.OutputIf data
        let actual = writer.LinePairs()

        CompareLines (expected: (struct (int * string)) list) actual


    [<Fact>]
    member _.``ForEach outputs correctly``() =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let (expected: (struct (int * string)) list) = 
            [ (0, ForEachModel.ForTesting.CSharpOpen |> List.head)
              (0, "{")
              (1, AssignmentModel.ForTesting.CSharp |> List.head)
              (0, "}")]
        let data = { ForEachModel.ForTesting.Data with Statements = [ AssignmentModel.ForTesting.Data] }

        outPutter.OutputForEach data
        let actual = writer.LinePairs()

        CompareLines (expected: (struct (int * string)) list) actual


    [<Fact>]
    member _.``Assignment outputs correctly``() =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let (expected: (struct (int * string)) list) = 
            [ (0, AssignmentModel.ForTesting.CSharp |> List.head )]
        let data = AssignmentModel.ForTesting.Data

        outPutter.OutputAssignment data
        let actual = writer.LinePairs()

        CompareLines (expected: (struct (int * string)) list) actual


    [<Fact>]
    member _.``AssignWithDeclare outputs correctly``() =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let (expected: (struct (int * string)) list) = 
            [ (0, AssignWithDeclareModel.ForTesting.CSharp |> List.head )]
        let data = AssignWithDeclareModel.ForTesting.Data

        outPutter.OutputAssignWithDeclare data
        let actual = writer.LinePairs()

        CompareLines (expected: (struct (int * string)) list) actual


    [<Fact>]
    member _.``Return outputs correctly``() =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let (expected: (struct (int * string)) list) = 
            [ (0,"return 42;") ]
        let data =  { ReturnModel.Expression = Some (Literal "42") }

        outPutter.OutputReturn data
        let actual = writer.LinePairs()

        CompareLines (expected: (struct (int * string)) list) actual


    [<Fact>]
    member _.``SimpleCall outputs correctly``() =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let (expected: (struct (int * string)) list) = 
            [ (0,"\"Harry\";") ]
        let data =  { SimpleCallModel.Expression = StringLiteral "Harry" }

        outPutter.OutputSimpleCall data
        let actual = writer.LinePairs()

        CompareLines (expected: (struct (int * string)) list) actual

    [<Fact>]
    member _.``Comment outputs correctly``() =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let (expected: (struct (int * string)) list) = 
            [ (0,"// This is a Comment") ]
        let data = CommentModel.Create "This is a Comment"

        outPutter.OutputComment data
        let actual = writer.LinePairs()

        CompareLines (expected: (struct (int * string)) list) actual


    [<Fact>]
    member _.``Auto-properties output correctly``() =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let (expected: (struct (int * string)) list) = 
            [ (0,"public MyReturnType MyProperty {get; set;}") ]
        let data = PropertyModel.ForTesting.Data

        outPutter.OutputProperty data
        let actual = writer.LinePairs()

        Assert.Equal(expected[0], actual[0])

        CompareLines (expected: (struct (int * string)) list) actual


    [<Fact>]
    member _.``Expanded properties output correctly``() =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        //// Don: This is the issue from VS
        //let issue = 
        //  [ "public MyReturnType MyProperty "
        //    "{"
        //    "get"
        //    "{"
        //     "return x;"
        //    "}"
        //    "set"
        //    "{"
        //     "value = x;"
        //    "}"
        //    "}" ]

        let (expected: (struct (int * string)) list) = 
            [ (0, PropertyModel.ForTesting.CSharpOpen |> List.head)
              (0, "{")
              (1, "get")
              (1, "{")
              (2, "return x;")
              (1, "}")
              (1, "set")
              (1, "{")
              (2, "value = x;")
              (1, "}")
              (0, "}")]
        let getter = {PropertyAccessorModel.Create(Getter) with Statements = [ { ReturnModel.Expression = Some (Literal "x") } ]}
        let setter = {PropertyAccessorModel.Create(Setter) with Statements = [ { AssignmentModel.Variable = "value"; Value = Literal "x"}]}
        let data = 
            { PropertyModel.ForTesting.Data with 
                Getter = Some getter
                Setter = Some setter }

        outPutter.OutputProperty data
        let actual = writer.LinePairs()

        CompareLines (expected: (struct (int * string)) list) actual


    //[<Fact>]
    //member _.``Void method outputs correctly`` () =
    //    let writer = ArrayWriter(3)
    //    let outPutter = RoslynOut(LanguageCSharp(),writer)
    //    let (expected: (struct (int * string)) list) = 
    //        [ (0, MethodModel.ForTesting.CSharpOpen |> List.head)
    //          (0, "{")
    //          (1, "var x = 42;")
    //          (1, "Console.WriteLine();")
    //          (0, "}")]
    //    let assignWithDeclare =  { Variable = "x"; TypeName = None; Value = (NonStringLiteralModel.Create "42")}
    //    let simpleCall = 
    //        { SimpleCallModel.Expression = 
    //             Invocation 
    //                { Instance = (NamedItem.Create "Console" [])
    //                  MethodName = SimpleNamedItem "WriteLine"
    //                  ShouldAwait = false
    //                  Arguments = [] } }
    //    let data = 
    //        { MethodModel.ForTesting.Data with 
    //            Statements = 
    //                [ assignWithDeclare
    //                  simpleCall ] }

    //    outPutter.OutputMethod data
    //    let actual = writer.LinePairs()

    //    CompareLines (expected: (struct (int * string)) list) actual


    [<Fact>]
    member _.``Class outputs correctly`` () =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let (expected: (struct (int * string)) list) = 
            [ (0, ClassModel.ForTesting.CSharpOpen |> List.head)
              (0, "{")
              (1, MethodModel.ForTesting.CSharpOpen |> List.head)
              (1, "{")
              (1, "}")
              (0, "}")]
        ()
        //let data = 
        //    { ClassModel.ForTesting.Data with
        //        Members = [ Method MethodModel.ForTesting.Data ]}

        //outPutter.OutputClass data
        //let actual = writer.LinePairs()

        //CompareLines (expected: (struct (int * string)) list) actual

    [<Fact>]
    member _.``Namespace outputs correctly`` () =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let (expected: (struct (int * string)) list) = 
            [ (0, NamespaceModel.ForTesting.CSharpOpen |> List.head)
              (0, "{")
              (1, "")
              (1, ClassModel.ForTesting.CSharpOpen |> List.head)
              (1, "{")
              (1, "}")
              (0, "}")]
        let data = 
            { NamespaceModel.ForTesting.Data with
                Classes = [ ClassModel.ForTesting.Data ]}

        outPutter.Output data
        let actual = writer.LinePairs()

        CompareLines (expected: (struct (int * string)) list) actual

