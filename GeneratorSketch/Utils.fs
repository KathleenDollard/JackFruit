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
        (fIsLeaf: 'groupId option->'item->bool)
        (fMapLeaf: 'groupId option ->'item->'r) // without the type annotation these try to return unit
        (fMapBranch: 'groupId ->'item option->'r list->'r)
        list =

        // This uses closures at present
        // This intenitionally shadows list
        let rec recurse groupId list = 
            // Find groups that define children
            let (leaves, notLeaves) = list |> List.partition (fIsLeaf groupId)
            let groups = notLeaves |> List.groupBy (fGroupBy groupId)

            // For each child, determine if it is a leaf and otherwise recurse
            [ for leaf in leaves do 
                fMapLeaf groupId leaf

                for g in groups do
                    let (gId, childList) = g
                    let someGId = Some gId
                    let (branchRoots, childNodes) = childList |> List.partition (fIsLeaf someGId)
                    let branchRoot = 
                        match branchRoots with
                        | [] -> None
                        | [root] -> Some root
                        | _ -> invalidOp $"Duplicate entries for {gId}"
                    let children = recurse (someGId) childNodes
                    fMapBranch gId branchRoot children 
                ]

        recurse None list
