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
        // KAD: for code like this that could infer, but it might be clearer if not, Info CodeFix that allows switching between
        (fGroupBy: 'groupId option -> 'item -> 'groupId)
        (fMapBranch: 'groupId option-> 'item option -> 'r list -> 'r)
        list =

        // This uses closures at present
        // This intenitionally shadows list
        let rec recurse groupId list = 

            let isLeaf gId item =
                Some (fGroupBy gId item) = gId

            let processGroup group =
                let (gId, itemList) = group 
                let (branchRoots, childList) = itemList |> List.partition (isLeaf (Some gId))
                let branchRoot = 
                    match branchRoots with
                    | [] -> None
                    | [root] -> Some root
                    | _ -> invalidOp $"Duplicate entries for {gId}"
                let children = recurse (Some gId) childList
                fMapBranch (Some gId) branchRoot children

            // Find groups that define children
            let groups = list |> List.groupBy (fGroupBy groupId)

            // For each child, determine if it is a leaf and otherwise recurse
            [ for group in groups do
                processGroup group
            ]

        recurse None list
