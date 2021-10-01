module Generator.GeneralUtils 

/// Removes leading and trailing characters
let RemoveLeadingTrailing startChar endChar (input:string) =
    if input = null then
        null
    else
        let input = input.Trim()

        if input.Length < 2 then
            input
        elif input.[0] = startChar && input.[input.Length - 1] = endChar then
            input.[1..input.Length-2]
        else
            input


/// Removes leading and trailing double 
let RemoveLeadingTrailingDoubleQuote (input:string) =
    RemoveLeadingTrailing '"' '"' input

let private RemoveCharsAndUpper (remove: char list) (input:string) =
    match input with 
    | null -> null
    | "" -> ""
    | _ ->
        let mutable lastIsSpecial = false
        new string 
            [| for c in input do
                if List.contains c remove then
                    lastIsSpecial <- true
                    ()
                elif lastIsSpecial then
                    lastIsSpecial <- false
                    System.Char.ToUpper c
                else
                    c |]

let ToCamel input =
    RemoveCharsAndUpper ['-'; '_'] input
     


type TreeNodeType<'T> = {
    Data: 'T
    Children: TreeNodeType<'T> list}

let TreeFromList 
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

