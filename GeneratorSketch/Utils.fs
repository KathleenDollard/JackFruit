namespace GeneratorSketch


module Utils =

    let removeLeadingTrailing startChar endChar (input:string) =
        let temp = input.Trim()

        if temp.Length < 2 then
            temp
        elif temp.[0] = startChar && temp.[temp.Length - 1] = endChar then
            temp.[1..temp.Length-2]
        else
            temp
        

                
    let removeLeadingTrailingDoubleQuote (input:string) =
        removeLeadingTrailing '"' '"' input

    // a
    // a b c
    // a b
    // b d e

    //type Tree<'Data> =
    //    | Leaf of 'Data
    //    | Branch of 'Data * 'Data list

    // Given a data, create either a leaf or depending on 
    //  fGroup -> How to group a list into the children at that level
    //  fIsLeaf -> Do we need to look any further

    let treeFromList 
        fGroupBy
        (fIsLeaf: 'groupId option->'item->bool)
        (fMapLeaf: 'groupId option->'item->'r) // without the type annotation these try to retun unit
        (fMapBranch: 'groupId option->'item option->'r list->'r)
        list =

        // This uses closures at present
        // This intenitionally shadows list
        let rec recurse (groupId: 'groupId option) (list: 'item list) = 
            match list with 
            | []
            | _ -> 
                // Find groups that define children
                let (leaves, notLeaves) = list |> List.partition (fIsLeaf groupId)
                let groups = notLeaves |> List.groupBy (fGroupBy groupId)

                // For each child, determine if it is a leaf and otherwise recurse
                [ for leaf in leaves do 
                    fMapLeaf groupId leaf

                  for g in groups do
                        let (gId, childList) = g
                        let someGId = Some gId
                        let (branchRoots, childNodes) = childList |> List.partition (fIsLeaf (someGId))
                        let branchRoot = 
                            match branchRoots with
                            | [] -> None
                            | [root] -> Some root
                            | _ -> invalidOp "Duplicate entries"
                        let children = recurse (someGId) childNodes
                        fMapBranch (someGId) branchRoot children 
                  ]

                //// For each child, determine if it is a leaf and otherwise recurse
                //[ for g in groups do
                //        let (gId, childList) = g
                //        for item in childList do 
                //            if fIsLeaf (Some gId) item then
                //                fMapLeaf (Some gId) item
                //        let children = recurse (Some gId) childList
                //        for c in children do
                //            c]
        recurse None list



    //let rec makeTree fLeaf fNode (tree:Tree<'LeafData,'INodeData>) :'r = 
    //     let recurse = makeTree fLeaf fNode  
    //     match tree with
    //     | LeafNode leafInfo -> 
    //         fLeaf leafInfo 
    //     | InternalNode (nodeInfo, subtrees) -> 
    //         fNode nodeInfo (subtrees |> List.map recurse)

    //let rec flattenTree fLeaf fNode acc (tree:Tree<'LeafData,'INodeData>) :'r = 
    //    let recurse = flattenTree fLeaf fNode  
    //    match tree with
    //    | LeafNode leafInfo -> 
    //        fLeaf acc leafInfo 
    //    | InternalNode (nodeInfo,subtrees) -> 
    //        // determine the local accumulator at this level
    //        let localAccum = fNode acc nodeInfo
    //        // thread the local accumulator through all the subitems using Seq.fold
    //        let finalAccum = subtrees |> Seq.fold recurse localAccum 
    //        // ... and return it
    //        finalAccum
