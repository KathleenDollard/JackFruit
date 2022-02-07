module Generator.Tests.GeneralUtils

open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open Generator.GeneralUtils
open Common

type ``When working with strings`` () =

    [<Fact>]
    member _.``Removing angle brackets succeeds when present`` () =
        let input = "<Some stuff>"
        let expected = "Some stuff"

        let actual = Generator.GeneralUtils.RemoveLeadingTrailing '<' '>' input

        actual |> should equal expected

    [<Fact>]
    member _.``Removing angle brackets does nothing when absent`` () =
        let input = "Some stuff"
        let expected = "Some stuff"

        let actual = Generator.GeneralUtils.RemoveLeadingTrailing '<' '>' input

        actual |> should equal expected

    [<Fact>]
    member _.``Removing angle brackets does nothing with empty string`` () =
        let input = ""
        let expected = ""

        let actual = Generator.GeneralUtils.RemoveLeadingTrailing '<' '>' input

        actual |> should equal expected

    [<Fact>]
    member _.``Removing angle brackets returns null for null`` () =
        let input = null
        let expected = null

        let actual = Generator.GeneralUtils.RemoveLeadingTrailing '<' '>' input

        actual |> should equal expected

    [<Fact>]
    member _.``Removing double quotes succeeds when present`` () =
        let input = @"""Some stuff"""
        let expected = "Some stuff"

        let actual = Generator.GeneralUtils.RemoveSurroundingDoubleQuote input

        actual |> should equal expected

    [<Fact>]
    member _.``ToCamel works on a variety of input`` () =
        ToCamel("") |> should equal ""
        ToCamel("lower") |> should equal "lower"
        ToCamel("UPPER") |> should equal "upper"
        ToCamel("start-end") |> should equal "startEnd"
        ToCamel("start_end") |> should equal "startEnd"
        ToCamel("START_END") |> should equal "startEnd"
        ToCamel("start-middle_end") |> should equal "startMiddleEnd"
        ToCamel("START-MIDDLE_END") |> should equal "startMiddleEnd"
        ToCamel("START_MIDDLE_END") |> should equal "startMiddleEnd"

    [<Fact>]
    member _.``ToSnake works on a variety of input`` () =
        ToSnake("") |> should equal ""
        ToSnake("startEnd") |> should equal "start_end"
        ToSnake("startMiddleEnd") |> should equal "start_middle_end"
        ToSnake("lower") |> should equal "lower"
        ToSnake("UPPER") |> should equal "upper"
        ToSnake("HTTPResult") |> should equal "http_result"

    [<Fact>]
    member _.``ToKebab works on a variety of input`` () =
        ToKebab("") |> should equal ""
        ToKebab("startEnd") |> should equal "start-end"
        ToKebab("startMiddleEnd") |> should equal "start-middle-end"
        ToKebab("lower") |> should equal "lower"
        ToKebab("UPPER") |> should equal "upper"
        ToKebab("HTTPResult") |> should equal "http-result"

    [<Fact>]
    member _.``ToPascal works on a variety of input`` () =
        ToPascal("") |> should equal ""
        ToPascal("lower") |> should equal "Lower"
        ToPascal("UPPER") |> should equal "Upper"
        ToPascal("start-end") |> should equal "StartEnd"
        ToPascal("start_end") |> should equal "StartEnd"
        ToPascal("START_END") |> should equal "StartEnd"
        ToPascal("start-middle_end") |> should equal "StartMiddleEnd"
        ToPascal("START-MIDDLE_END") |> should equal "StartMiddleEnd"
        ToPascal("START_MIDDLE_END") |> should equal "StartMiddleEnd"
// KAD: I want to put the following mess inside the type that it applies to, but got errors: Why?
// TODO: If the above is a real problem not a mistake on my part, move this to a separate file

/// Test type for input to testing tree building. The type in the actual code will be 
/// slightly more complex
type InputType<'T> = {
    Parents: string list
    InputData: 'T }


let private example1 = [
    {Parents = ["dotnet"]; InputData="dotnet"}
    {Parents = ["dotnet"; "add"]; InputData="add"}
    {Parents = ["dotnet"; "add"; "package"]; InputData="package"}
    {Parents = ["dotnet"; "add"; "project"]; InputData="project"}
    {Parents = ["dotnet"; "build"]; InputData="build"}
    {Parents = ["dotnet"; "clean"]; InputData="clean"}
    {Parents = ["dotnet"; "format "; "whitespace";]; InputData="whitespace"}
    {Parents = ["dotnet"; "format "; "style";]; InputData="style"}
    {Parents = ["dotnet"; "format "; "analyzers";]; InputData="analyzers"} ]

let private example1Expected = [
    { Data="dotnet"; Children = [
        { Data = "add"; Children = [
            { Data = "package"; Children = [] }
            { Data = "project"; Children = [] } ]} 
        { Data = "build"; Children = []}
        { Data = "clean"; Children = []}
        { Data = "dotnet,format"; Children = [
            { Data = "whitespace"; Children = [] } 
            { Data = "style"; Children = [] } 
            { Data = "analyzers"; Children = [] }] }] }]

let private example2 = [
    {Parents = ["dotnet"]; InputData="dotnet"}
    {Parents = ["dotnet"; "add"]; InputData="add"}
    {Parents = ["dotnet"; "add"; "package"]; InputData="package"}
    {Parents = ["dotnet"; "add"; "project"]; InputData="project"} ]

let private example2Typo = [
    { Data="dotnet"; Children = [
        { Data = "add"; Children = [
            { Data = "packge"; Children = [] }
            { Data = "project"; Children = [] } ]} 
        ]} ]

let private example2MissingEntry = [
    { Data="dotnet"; Children = [
        { Data = "add"; Children = [
            { Data = "project"; Children = [] } ]} 
        ]} ]

let private example2ExtraEntry = [
    { Data="dotnet"; Children = [
        { Data = "add"; Children = [
            { Data = "packge"; Children = [] }
            { Data = "extra"; Children = [] }
            { Data = "project"; Children = [] } ]} 
        ]} ]

let private example3 = [
    {Parents = ["a"]; InputData="Cedrella"} ]

let private example3Expected = [
    { Data="Cedrella"; Children = [] } ]

let private mapBranch parents item childList=
    let data = 
        match item with 
        | Some i -> i.InputData
        | None -> parents |> String.concat ","
    { Data = data; Children = childList }

let private getKey item = item.Parents

let private treeNodeTypeFromInput (input: InputType<string> list) = 
    TreeFromKeyedList getKey mapBranch input

let private matches (expected: TreeNodeType<string> list) (actual: TreeNodeType<string> list) =
    let rec recurse (exp: TreeNodeType<string> list) (act: TreeNodeType<string> list) = 
         // KAD: Figure out the right thing to throw on lack of match
        if act.Length > exp.Length then invalidOp "An extra row was present"
        for item in exp do
            let matching = act |> List.tryFind (fun x -> x.Data = item.Data)
            if matching.IsSome then
                recurse item.Children matching.Value.Children
            elif item.Data.Contains(",") then 
                () // this just means the Data was inferred
            else
                invalidOp $"Missing {item.Data}"
            
    recurse expected actual

let shouldNotMatch (expected: TreeNodeType<string> list) (actual: TreeNodeType<string> list) =
    try
        matches expected actual
        raise (System.ApplicationException("Failure was expected and did not occur"))
    with 
        | :? System.InvalidOperationException -> () // all is well, failure expected
        | _ -> reraise()


type ``When building a tree from a list of tuple(string list, string)``() =

    [<Fact>]
    member _.``Match test fails with typo``() =
        let actual = treeNodeTypeFromInput example2

        actual |> shouldNotMatch example2Typo

    [<Fact>]
    member _.``Match test fails with missing row``() =
        let actual = treeNodeTypeFromInput example2

        actual |> shouldNotMatch example2ExtraEntry

    [<Fact>]
    member _.``Match test fails with extra row``() =
        let actual = treeNodeTypeFromInput example2

        actual |> shouldNotMatch example2MissingEntry


    [<Fact>]
    member _.``Tree is created with an empty list``() =
        let input = []
        let expected = []

        let actual = treeNodeTypeFromInput input

        actual |> matches expected


    [<Fact>]
    member _.``Tree is created with a list of one InputType``() =

        let actual = treeNodeTypeFromInput example3

        actual |> matches example3Expected


    [<Fact>]
    member _.``With a nested list of InputType``() =

        let actual = treeNodeTypeFromInput example1

        actual |> matches example1Expected

        

