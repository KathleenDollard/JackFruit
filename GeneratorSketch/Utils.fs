namespace GeneratorSketch


module Utils =

    /// Removes leading and trailing characters
    let RemoveLeadingTrailing startChar endChar (input:string) =
        let temp = input.Trim()

        if temp.Length < 2 then
            temp
        elif temp.[0] = startChar && temp.[temp.Length - 1] = endChar then
            temp.[1..temp.Length-2]
        else
            temp


    /// Removes leading and trailing double 
    let RemoveLeadingTrailingDoubleQuote (input:string) =
        RemoveLeadingTrailing '"' '"' input
    

    let TreeFromList 
        (fKey: 'item -> string list)
        (fMapBranch: string list-> 'item option -> 'r list -> 'r)
        list =

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

