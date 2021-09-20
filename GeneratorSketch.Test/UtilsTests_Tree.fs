module UtilsTests_Tree

open Xunit
open GeneratorSketch.Utils


// In the eventual code, the "TreeNodeType" will be a CommandDef model that gathers 
// Data from the archetype, the handler, evaluated dictionaries/JSON (descriptions)
// and some solution for validation. 

/// Test type for input to testing tree building. The type in the actual code will be the  
/// slightly more complex ArchetypeInfo
type InputType<'T> = {
    Parents: string list
    InputData: 'T }

/// Test type for testing tree building output. The type in the actual code will be the 
/// significantly more complex CommandDef
type TreeNodeType<'T> = {
    Data: 'T
    Children: TreeNodeType<'T> list}


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
        ]}
    { Data = "build"; Children = []}
    { Data = "clean"; Children = []}
    { Data = "format"; Children = [
        { Data = "whitespace"; Children = [] } 
        { Data = "style"; Children = [] } 
        { Data = "analyzers"; Children = [] }] }]

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

let private groupByAncestors (current: string list option) item = 
    match current with 
    | Some s -> item.Parents.[0..s.Length] // if the current is [a b], we want to group by [a b c]
    | None -> item.Parents.[0..0]          // at the start, we need everything

let private isLeaf (current: string list option) item =
    match current with 
    | Some s -> item.Parents.Length = s.Length
    | None -> item.Parents.Length = 0

let private mapLeaf _ item =
    { Data = item.InputData; Children = [] }
    
let private mapBranch Parents item childList=
    let Data = 
        match item with 
        | Some i -> i.InputData
        | None ->  Parents |> String.concat ","
    { Data = Data; Children = childList }

let private treeNodeTypeFromInput (input: InputType<string> list) = 
    TreeFromList groupByAncestors isLeaf mapLeaf mapBranch input

let private matches (expected: TreeNodeType<string> list) (actual: TreeNodeType<string> list) =
    // KAD: Check with Don on equality to see if this is needed
    let rec recurse (exp: TreeNodeType<string> list) (act: TreeNodeType<string> list) = 
         // KAD: Figure out the right thing to throw on lack of match
        if act.Length > exp.Length then invalidOp "An extra row was present"
        for item in exp do
            let matching = act |> List.tryFind (fun x -> x.Data = item.Data)
            // KAD: Is there a more reasonable way to do a "not"
            //if matching.IsNone && item.Data.Contains(",") = false then invalidOp $"Missing {item.Data}"
            // KAD: In the next line, VS sees the & separately, and refers to this as a binary and. I had to look it up.
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
    member _.``Tree is created with a list of one InputType``() =

        let actual = treeNodeTypeFromInput example3

        actual |> matches example3Expected


    [<Fact>]
    member _.``With a nested list of InputType``() =

        let actual = treeNodeTypeFromInput example1

        actual |> matches example1Expected

        
