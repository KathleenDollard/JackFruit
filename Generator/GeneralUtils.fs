module Generator.GeneralUtils 

open System
open System.Text

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
// TODO: Solve naming challenge: RemoveLeadingTrailing was too long. UnSurround isn't a word
let RemoveSurroundingDoubleQuote (input:string) =
    RemoveLeadingTrailing '"' '"' input

let RemoveSurroundingAngleBrackets (input:string) =
    RemoveLeadingTrailing '<' '>' input

let RemoveSurroundingSquareBrackets(input:string) =
    RemoveLeadingTrailing '[' ']' input

let UseDefaultIfEmpty defaultValue (list: 'a list) =
    if list.IsEmpty then 
        [ defaultValue ]
    else 
        list

let private RemoveCharsAndUpper (remove: char list) (input:string) =
    let charIsSpecial c = remove |> List.contains c
    match input with 
    | null -> null
    | "" -> ""
    | _ ->
        let mutable lastIsSpecial = false
        new string 
            [| for c in input do
                if charIsSpecial c then
                    lastIsSpecial <- true
                    ()
                elif lastIsSpecial then
                    lastIsSpecial <- false
                    System.Char.ToUpper c
                else
                    c |]

/// Adds character between words. Sequential upper case is treated as a word. 
/// where 'HTTPResult' would be 'http_result'
let private AddCharsAndLower (add:string) (input: string) =
    let lastWasUpper i =
        if i > 0 then
            System.Char.IsUpper(input.[i-1])
        else
            false
    let nextIsLower i =
        if i < input.Length - 2 then
            System.Char.IsLower(input.[i+1])
        else
            false
    match input with 
    | null -> null
    | "" -> ""
    | _ ->
        let x = [
            for i in (0) .. (input.Length - 1) do
                let c = input[i]
                let thisIsUpper = System.Char.IsUpper(c)
                let partOfCappedWord = (lastWasUpper i) && not (nextIsLower i)
                if i <> 0 && thisIsUpper && (not partOfCappedWord) then
                    add + System.Char.ToLower(c).ToString()
                elif thisIsUpper then
                    System.Char.ToLower(c).ToString()
                else
                    c.ToString()]
        String.Join("", x)

let ToCamel input =
    RemoveCharsAndUpper ['-'; '_'] input

let ToPascal input =
    match input with 
    | null -> null
    | "" -> ""
    | _ ->
        let camel = RemoveCharsAndUpper ['-'; '_'] input
        let first = System.Char.ToUpper(camel[0]).ToString()
        first + camel[1..]

let ToSnake input =
    AddCharsAndLower "_" input

let ToKebab input =
    AddCharsAndLower "-" input

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

type SpaceStringBuilder(spacesForIndent: int) =
    let sb = new StringBuilder()
    let mutable indentSize = spacesForIndent
    let mutable indentLevel = 0
      
    member _.AppendLine line = 
        let spaceCount = indentSize * indentLevel
        let spaces = String.replicate spaceCount " "
        sb.AppendLine(spaces + line)

    member _.AppendLines lines list = 
        let spaceCount = indentSize * indentLevel
        let spaces = String.replicate spaceCount " "
        for line in lines do
            sb.AppendLine(spaces + line) |> ignore
        ()
        
    member _.IncreaseIndent =
        indentLevel <- indentLevel + 1

    member _.DecreaseIndent =
        if indentLevel > 1 then
            indentLevel <- indentLevel - 1

    override _.ToString() =
        sb.ToString()