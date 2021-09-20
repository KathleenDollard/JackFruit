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
    { data = "f,g"; children = [
        { data = "Harry"; children = [] }]}
    { data = "h,i"; children = [
        { data = "Hermione"; children = [] }] }]

let example2 = [
    {parents = ["a"]; inputData="Cedrella"}
    {parents = ["a"; "b"]; inputData="Arthur"}
    {parents = ["a"; "b"; "d"]; inputData="Fred"}
    {parents = ["a"; "b"; "e"]; inputData="George"}
    {parents = ["f"; "g"]; inputData="Harry"} ]

let example2Typo = [
    { data="Cedrella"; children = [
        { data = "Arthur"; children = [
            { data = "Frd"; children = [] }
            { data = "George"; children = [] }
        ]} ]}
    { data = "f,g"; children = [
        { data = "Harry"; children = [] }]} ]

let example2MissingEntry = [
    { data="Cedrella"; children = [
        { data = "Arthur"; children = [
            { data = "George"; children = [] }
        ]} ]}
    { data = "f,g"; children = [
        { data = "Harry"; children = [] }]} ]

let example2ExtraEntry = [
    { data="Cedrella"; children = [
        { data = "Arthur"; children = [
            { data = "Fred"; children = [] }
            { data = "George"; children = [] }
            { data = "Bill"; children = [] }
        ]} ]}
    { data = "f,g"; children = [
        { data = "Harry"; children = [] }]} ]

let example3 = [
    {parents = ["a"]; inputData="Cedrella"} ]

let example3Expected = [
    { data="Cedrella"; children = [] } ]

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
    
let mapBranch parents item childList=
    let data = 
        match item with 
        | Some i -> i.inputData
        | None ->  parents |> String.concat ","
    { data = data; children = childList }

let treeNodeTypeFromInput (input: inputType list) = 
    treeFromList groupByAncestors isLeaf mapLeaf mapBranch input

let matches (expected: treeNodeType list) (actual: treeNodeType list) =
    // KAD: Check with Don on equality to see if this is needed
    let rec recurse (exp: treeNodeType list) (act: treeNodeType list) = 
         // KAD: Figure out the right thing to throw on lack of match
        if act.Length > exp.Length then invalidOp "An extra row was present"
        for item in exp do
            let matching = act |> List.tryFind (fun x -> x.data = item.data)
            // KAD: Is there a more reasonable way to do a "not"
            //if matching.IsNone && item.data.Contains(",") = false then invalidOp $"Missing {item.data}"
            // KAD: In the next line, VS sees the & separately, and refers to this as a binary and. I had to look it up.
            if matching.IsSome then
                recurse item.children matching.Value.children
            elif item.data.Contains(",") then 
                () // this just means the data was inferred
            else
                invalidOp $"Missing {item.data}"
                
    recurse expected actual

let shouldNotMatch (expected: treeNodeType list) (actual: treeNodeType list) =
    try
        matches expected actual
        raise (System.ApplicationException("Failure was expected and did not occur"))
    with 
        | :? System.InvalidOperationException -> () // all is well, failure expected
        // KAD: I tried reraise here without parens, and got a type inconsistent in the match issue. I understand that, but let's make it easier
        | _ -> reraise()


type ``When testing for matches``() =

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



type ``When buliding a tree from a list of tuple(string list, string)``() =

    [<Fact>]
    member _.``Tree is created with an empty list``() =
        let input = []
        let expected = []

        let actual = treeNodeTypeFromInput input

        actual |> matches expected


    [<Fact>]
    member _.``Tree is created with a list of one inputType``() =

        let actual = treeNodeTypeFromInput example3

        actual |> matches example3Expected


    [<Fact>]
    member _.``With a nested list of inputType``() =

        let actual = treeNodeTypeFromInput example1

        actual |> matches example1Expected

        
