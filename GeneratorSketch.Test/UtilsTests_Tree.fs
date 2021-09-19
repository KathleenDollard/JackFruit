module UtilsTests_Tree

open Xunit
open FsUnit.Xunit
open GeneratorSketch.Utils


type inputType = {
    parents: string list
    inputData: string }

type treeNodeType = {
    data: string
    children: treeNodeType list}

let example1 = [
    {parents = ["a"; "b"; "c"]; inputData="Fred"}
    {parents = ["a"; "b"; "d"]; inputData="George"}
    {parents = ["a"; "b"; "e"]; inputData="Bill"} ]

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
    { data = item.inputData; children = childList }

let treeNodeTypeFromInput = treeFromList groupByAncestors isLeaf mapLeaf mapBranch

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
        let input = example1
        let expected = []

        let actual = treeNodeTypeFromInput input

        Assert.Equal(true, false)
