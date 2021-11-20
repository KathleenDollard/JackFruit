module LanguageCSharpTests

open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open Generator.GeneralUtils
open Generator
open Generator.Language
open Generator.Tests.TestData
open Common

type ``When working with language parts`` () =
    let cSharp = LanguageCSharp() :> ILanguage

    [<Fact>]
    member _.``Using is correct without alias`` () =
        let actual = cSharp.Using Using.ForTesting.Data
        actual |> should equal Using.ForTesting.CSharp


    [<Fact>]
    member _.``Using is correct with alias`` () =
        let actual = cSharp.Using {  Using.ForTesting.Data with Alias = Some "B"}
        actual |> should equal [ "using B = System;" ]


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

    // Assignment, AssigWithDeclare, Return, SimpleCall and Comment 
    // are sufficiently simple they are only tested as outputs below

type ``When outputting code`` () =
    let writer = RoslynOut(LanguageCSharp(),ArrayWriter(3))


    [<Fact>]
    member _.``If outputs correctly``() =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let expected = 
            [ (0, If.ForTesting.CSharpOpen |> List.head)
              (0, "{")
              (1, Assignment.ForTesting.CSharp |> List.head)
              (0, "}")]
        let data = { If.ForTesting.Data with Statements = [ Statement.Assign Assignment.ForTesting.Data] }

        // KAD-Don: Ick, where did my design go wrong that I have things like Assignment Assignment
        // KAD-Don: Why isn't there a warning when I assign unit? I know it is legal, but is it ever sensible?
        outPutter.OutputIf data
        let actual = writer.LinePairs()

        actual |> should equal expected


    [<Fact>]
    member _.``ForEach outputs correctly``() =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let expected = 
            [ (0, ForEach.ForTesting.CSharpOpen |> List.head)
              (0, "{")
              (1, Assignment.ForTesting.CSharp |> List.head)
              (0, "}")]
        let data = { ForEach.ForTesting.Data with Statements = [ Statement.Assign Assignment.ForTesting.Data] }

        outPutter.OutputForEach data
        let actual = writer.LinePairs()

        actual |> should equal expected


    [<Fact>]
    member _.``Assignment outputs correctly``() =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let expected = 
            [ (0, Assignment.ForTesting.CSharp |> List.head )]
        let data = Assignment.ForTesting.Data

        outPutter.OutputAssignment data
        let actual = writer.LinePairs()

        actual |> should equal expected


    [<Fact>]
    member _.``AssignWithDeclare outputs correctly``() =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let expected = 
            [ (0, AssignWithDeclare.ForTesting.CSharp |> List.head )]
        let data = AssignWithDeclare.ForTesting.Data

        outPutter.OutputAssignWithDeclare data
        let actual = writer.LinePairs()

        actual |> should equal expected


    [<Fact>]
    member _.``Return outputs correctly``() =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let expected = 
            [ (0,"return 42;") ]
        let data = NonStringLiteral "42"

        outPutter.OutputReturn data
        let actual = writer.LinePairs()

        actual |> should equal expected


    [<Fact>]
    member _.``SimpleCall outputs correctly``() =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let expected = 
            [ (0,"\"Harry\";") ]
        let data = StringLiteral "Harry"

        outPutter.OutputSimpleCall data
        let actual = writer.LinePairs()

        actual |> should equal expected

    [<Fact>]
    member _.``Comment outputs correctly``() =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let expected = 
            [ (0,"// This is a Comment") ]
        let data = Comment "This is a Comment"

        outPutter.OutputComment data
        let actual = writer.LinePairs()

        actual |> should equal expected


    [<Fact>]
    member _.``Auto-properties output correctly``() =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let expected = 
            [ (0,"public MyReturnType MyProperty {get; set;}") ]
        let data = Property.ForTesting.Data

        outPutter.OutputProperty data
        let actual = writer.LinePairs()

        actual |> should equal expected


    [<Fact>]
    member _.``Expanded properties output correctly``() =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        //// **Don**: This is the issue from VS
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

        let expected = 
            [ (0, Property.ForTesting.CSharpOpen |> List.head)
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
        let data = 
            { Property.ForTesting.Data with 
                GetStatements = [Return (Symbol "x")]
                SetStatements = [Statement.Assign { Item = "value"; Value = (Symbol "x")}] }

        outPutter.OutputProperty data
        let actual = writer.LinePairs()

        actual |> should equal expected


    [<Fact>]
    member _.``Void method outputs correctly`` () =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let expected = 
            [ (0, Method.ForTesting.CSharpOpen |> List.head)
              (0, "{")
              (1, "var x = 42;")
              (1, "Console.WriteLine();")
              (0, "}")]
        let data = 
            { Method.ForTesting.Data with 
                Statements = 
                    [ AssignWithDeclare { Variable = "x"; TypeName = None; Value = (NonStringLiteral "42")}
                      SimpleCall (Invocation { Instance = (NamedItem.Create "Console" []); MethodName = "WriteLine"; Arguments = [] }) ] }

        outPutter.OutputMethod data
        let actual = writer.LinePairs()

        actual |> should equal expected


    [<Fact>]
    member _.``Class outputs correctly`` () =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let expected = 
            [ (0, Class.ForTesting.CSharpOpen |> List.head)
              (0, "{")
              (1, Method.ForTesting.CSharpOpen |> List.head)
              (1, "{")
              (1, "}")
              (0, "}")]
        let data = 
            { Class.ForTesting.Data with
                Members = [ Method Method.ForTesting.Data ]}

        outPutter.OutputClass data
        let actual = writer.LinePairs()

        actual |> should equal expected

    [<Fact>]
    member _.``Namespace outputs correctly`` () =
        let writer = ArrayWriter(3)
        let outPutter = RoslynOut(LanguageCSharp(),writer)
        let expected = 
            [ (0, Namespace.ForTesting.CSharpOpen |> List.head)
              (0, "{")
              (1, Class.ForTesting.CSharpOpen |> List.head)
              (1, "{")
              (1, "}")
              (0, "}")]
        let data = 
            { Namespace.ForTesting.Data with
                Classes = [ Class.ForTesting.Data ]}

        outPutter.Output data
        let actual = writer.LinePairs()

        actual |> should equal expected

