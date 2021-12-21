module Common

//open Generator.GeneralUtils 
open System


type NamedItem =
    | GenericNamedItem of Name: string * GenericTypes: NamedItem list
    | SimpleNamedItem of Name: string
    static member GenericsFromStrings (name: string) genericsAsStrings =
        genericsAsStrings |> List.map (fun x -> SimpleNamedItem x)
    member this.SimpleName() =
        match this with 
        | SimpleNamedItem name -> name
        | GenericNamedItem (name, t) -> name
    static member Create (name: string) generics =
        match generics with 
        | [] -> SimpleNamedItem name
        | _ -> GenericNamedItem (name, generics)


type Return =
    | Void
    | Type of t: NamedItem
    static member Create typeName =
        match typeName with 
         | "void" -> Void
         | _ -> Type (NamedItem.Create (typeName) [])


type TreeNodeType<'T> = {
    Data: 'T
    Children: TreeNodeType<'T> list }

/// <summary>
/// Build a tree from a list of items. The items can be in any order, and are placed
/// into the tree via keys. 
/// </summary>
/// <param name="fKey">Defines the tree structure based on a grouping key that maps to ancestors.</param>
/// <param name="fMapBranch">Defines the data and children for a branch.</param>
/// <param name="list">The list of items to load into the tree</param>
let TreeFromKeyedList 
    (fKey: 'item -> string list)
    (fMapBranch: string list-> 'item option -> 'r list -> 'r)
    (list: 'item list) =

    // This uses closures at present
    // This intenitionally shadows list
    let rec recurse (groupId: string list) list = 

        let ancestors (gId: string list) item =
            let key = fKey item
            key.[0..gId.Length]
        
        let isLeaf gId item =
            ancestors gId item = gId

        // Find groups that define children
        let groups = list |> List.groupBy (ancestors groupId)

        // For each child, determine if it is a leaf and otherwise recurse
        [ for group in groups do
            let (gId, itemList) = group 
            let (branchRoots, childList) = itemList |> List.partition (isLeaf gId)
            let branchRoot = 
                match branchRoots with
                | [] -> None
                | [root] -> Some root
                | _ -> invalidOp $"Duplicate entries for {gId}"
            let children = recurse gId childList
            fMapBranch gId branchRoot children
        ]

    recurse [] list

/// <summary>
/// Build a tree from a delimited string, such as "A (B, C(D))". Errors such as brace matching 
/// may result in an invalid tree or dropped data, but won't fail.
/// </summary>
/// <param name="openChar">Character that indicates opening a new child branch.</param>
/// <param name="closeChar">Character that indicates the end of a child branch.</param>
/// <param name="separator">Character thaat separates children (such as a comma).</param>
/// <param name="input">The string used to load tree</param>
let TreeFromDelimitedString (openChar: char) (closeChar: char) (separator: char) (input: string) =

    // Implementation approach.. Given:
    // className <string, List<int>, int, float, MyType<List<bool>>>
    // 
    // <string, List<int>, int, float, MyType<List<bool>>>
    // , List<int>, int, float, MyType<List<bool>>>
    // , int, float, MyType<List<bool>>>
    // , float, MyType<List<bool>>>
    // , MyType<List<bool>>>
    // >


    let rec splitOne (input: string) : TreeNodeType<string> * string =
        let (nextPunctChar, nextPunctPos) = 
            let pos = input.IndexOfAny([| openChar; closeChar; separator |])
            if pos < 0 then ("", input.Length)
            else (input[pos].ToString(), pos)

        let currentData = input[0..nextPunctPos - 1].Trim()
        let remaining = input[nextPunctPos..]

        if nextPunctChar = openChar.ToString() then
            let trees, rem = splitMany remaining
            ( { Data = currentData; Children = trees}, rem)
        else
            ( { Data = currentData; Children = [] }, remaining)

    and splitMany (inputWithPrefix: string) : TreeNodeType<string> list * string =
        if inputWithPrefix.StartsWith(closeChar) then
            [], inputWithPrefix[1..]
        else
            let input = inputWithPrefix[1..]

            let (tree, rem) = splitOne input
            let (childTrees, rem2) = splitMany rem
            (tree::childTrees, rem2)

    if input.Length = 0 then 
        {Data = ""; Children = []}
    else
        let (tree, s) = splitOne input
        tree
