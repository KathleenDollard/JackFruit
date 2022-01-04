module Common

open System

type Scope =
    | Unknown
    | Public
    | Private
    | Internal
    | Protected

type Operator =
    | Equals
    | NotEquals
    | GreaterThan
    | LessThan
    | GreaterThanOrEqualTo
    | LessThanOrEqualTo

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

    let maxRecursion = 15 // Stack overflow is a PITB exception to resolve, so forcing to a real exception

    let rec splitOne (input: string) (depth: int): TreeNodeType<string> * string =
        if depth > maxRecursion then invalidOp "May be in infite recursion in splitOne, or you made want a algorithm that supports tail call optimiation"
        if String.IsNullOrWhiteSpace(input) then
            ({Data = ""; Children = []}, "")
        else

            let (nextPunctChar, nextPunctPos, currentData, remaining) = 
                let input = input.Trim()
                let pos = input.IndexOfAny([| openChar; closeChar; separator |])
                match pos with 
                | _ when pos < 0 -> ("", input.Length, input.Trim(), "")
                | 0 -> (input[pos].ToString(), pos, "", input[pos..]) 
                | _ -> (input[pos].ToString(), pos, input[0..pos - 1].Trim(), input[pos..]) 

            if nextPunctChar = openChar.ToString() then
                let trees, rem = splitMany remaining (depth + 1)
                ( { Data = currentData; Children = trees}, rem)
            else
                ( { Data = currentData; Children = [] }, remaining)

    and splitMany (inputWithPrefix: string) (depth: int): TreeNodeType<string> list * string =
        if depth > maxRecursion then invalidOp "May be in infite recursion in splitMany, or you made want a algorithm that supports tail call optimiation"
        if inputWithPrefix[0] = closeChar then
            [], inputWithPrefix[1..]
        else
            let input = inputWithPrefix[1..]

            let (tree, rem) = splitOne input (depth + 1)
            if String.IsNullOrWhiteSpace(rem) then
                ([tree], "")
            else
                let (childTrees, rem2) = splitMany rem (depth + 1)
                (tree::childTrees, rem2)

    if input.Length = 0 then 
        {Data = ""; Children = []}
    else
        let (tree, s) = splitOne input 0
        tree

let MapTree<'b> fMap tree : 'b =
    let rec innerMap (recurseCount: int) fMap tree : 'b =
        if recurseCount > 10 then 
            invalidOp "Possible runaway recursion!"
        let newChildren =
            [ for child in tree.Children do 
                innerMap (recurseCount + 1) fMap child ]
        fMap tree newChildren
    innerMap 0 fMap tree


type NamedItem =
    | GenericNamedItem of Name: string * GenericTypes: NamedItem list
    | SimpleNamedItem of Name: string
    static member GenericsFromStrings (name: string, genericsAsStrings) =
        genericsAsStrings |> List.map (fun x -> SimpleNamedItem x)
    member this.WithoutGenerics() =
        match this with 
        | SimpleNamedItem name -> name
        | GenericNamedItem (name, t) -> name
    static member Create (name: string, generics) =
        match generics with 
        | [] -> SimpleNamedItem name
        | _ -> GenericNamedItem (name, generics)
    static member Create (name: string) =
        let fMap (oldNode: TreeNodeType<string>) (newChildren: NamedItem list) =
            if newChildren.IsEmpty then 
                SimpleNamedItem oldNode.Data
            else
                GenericNamedItem (oldNode.Data, newChildren)
        TreeFromDelimitedString '<' '>' ',' name
        |> MapTree fMap 
    static member op_Implicit(name: string) : NamedItem = 
        NamedItem.Create name


        