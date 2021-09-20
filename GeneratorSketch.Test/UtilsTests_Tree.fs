module UtilsTests_Tree

open Xunit
open FsUnit.Xunit
open GeneratorSketch.Utils
open FsUnit.CustomMatchers


type inputType = {
    parents: string list
    inputData: string }

type treeNodeType = {
    data: string
    children: treeNodeType list}

let example1 = [
    {parents = ["a"]; inputData="Cedrella"}
    {parents = ["a"; "b"]; inputData="Arthur"}
    {parents = ["a"; "b"; "d"]; inputData="Fred"}
    {parents = ["a"; "b"; "e"]; inputData="George"}
    {parents = ["a"; "b"; "c";]; inputData="Bill"}
    {parents = ["a"; "b"; "f";]; inputData="Percy"}
    {parents = ["f"; "g"]; inputData="Harry"}
    {parents = ["h"; "i"]; inputData="Hermione"} ]

let example1Expected = [
    { data="Cedrella"; children = [
        { data = "Arthur"; children = [
            { data = "Fred"; children = [] }
            { data = "George"; children = [] }
            { data = "Bill"; children = [] }
            { data = "Percy"; children = [] } ]} 
        ]}
    { data = "<unknown>"; children = [
        { data = "Harry"; children = [] }]}
    { data = "<unknown>"; children = [
        { data = "Hermione"; children = [] }] }]

let groupByAncestors (current: string list option) item = 
    match current with 
    | Some s -> item.parents.[0..s.Length] // if the current is [a b], we want to group by [a b c]
    | None -> item.parents.[0..0]          // at the start, we need everything

let isLeaf (current: string list option) item =
    match current with 
    | Some s -> item.parents.Length = s.Length
    | None -> item.parents.Length = 0

let mapLeaf _ item =
    { data = item.inputData; children = [] }
    
let mapBranch _ item childList=
    let data = 
        match item with 
        | Some i -> i.inputData
        | None -> "<unknown>"
    { data = data; children = childList }

let treeNodeTypeFromInput (input: inputType list) = 
    treeFromList groupByAncestors isLeaf mapLeaf mapBranch input

let matches expected actual =
    let rec recurse = 
        for item in expected do
            let hasMatch = actual |> List.exists (fun x -> x = item)
            if not hasMatch then invalidOp $"Mismatch on {item.data}" // Figure out the right thing to throw on lack of match
    ()



type ``When buliding a tree from a list of tuple(string list, string)``() =
    [<Fact>]
    member _.``Tree is created with an empty list``() =
        let input = []
        let expected = []

        let actual = treeNodeTypeFromInput input

        Assert.Equal(true, false)

    [<Fact>]
    member _.``Tree is created with a list of one string``() =
        Assert.Equal(true, false)

    [<Fact>]
    member _.``Tree is created with a flat list of many strings``() =
        Assert.Equal(true, false)

    [<Fact>]
    member _.``With a nested list of strings``() =

        let actual = treeNodeTypeFromInput example1

        actual |> matches example1Expected
