module Generator.ArchetypeMapping

//let BuildCommandDef model archInfoTree =
//    let recurse model tree =
        

//// invocations list is built in the SyntaxReciever and then changed to a tuple in the
//// language specific code of the generator
//let generate invocations model= 
//    ArchetypeInfoListFrom invocations   // invocations                      -> archetypeInfo list Result
//    |> bind BuildTree               // archetypeInfo list Result        -> treeNode<archetypeInfo> Result
//    |> bind BuildCommandDef model   // treeNode<archetypeInfo> Result   -> CommandDef list Result
//    |> bind BuildCode CSharp        // CommandDef list Result           -> string

open Generator.GeneralUtils
open Generator.Models
open RoslynUtils
open Microsoft.CodeAnalysis



let ParseArchetypeInfo archetype handler =
    let stringSplitOptions =
        System.StringSplitOptions.RemoveEmptyEntries
        ||| System.StringSplitOptions.TrimEntries

    let parts =
        (RemoveLeadingTrailingDoubleQuote archetype)
            .Split(' ', stringSplitOptions)
        |> Array.toList

    let commandNames =
        [ for part in parts do
              match part with
              | Command commandName -> commandName
              | _ -> () ]

    { AncestorsAndThis = 
        // Root command name is generally empty
        if commandNames.IsEmpty then
            [""]
        else
            commandNames
      Raw = parts
      HandlerExpression = handler }


let ExpresionOption expression =
    if IsNullLiteral expression then
        None
    else
        Some expression


let ArchetypeInfoListFrom invocations =
        let archetypeInfoWithResults =
            let mutable pos = 0
            [ for invoke in invocations do
                let pos = pos + 1
                match invoke with
                | (_, [ a; d ]) ->
                    let archetype = StringFrom a
                    let expression = ExpressionFrom d
                    match (archetype, expression) with
                    | Ok arch, Ok expr -> Ok (ParseArchetypeInfo arch (ExpresionOption expr))
                    | Error _, _ ->  Error (UnexpectedExpressionForArchetype pos)
                    | Ok arch, Error _ ->  Error (UnexpectedExpressionForHandler arch)
                | _ -> Error UnexpectednumberOfArguments ]
        let errors = [
            for result in archetypeInfoWithResults do
                match result with 
                | Error err -> err
                | _ -> ()]
        match errors with 
        | [] -> Ok [
            for result in archetypeInfoWithResults do
                match result with
                | Ok arch -> arch
                | _ -> () ]
        | _ -> Error (Aggregate errors)


let ArchetypeInfoTreeFrom archetypeInfoList = 
    let mapBranch parents item childList=
        let data = 
            match item with
            | Some x -> x
            | None -> 
                { AncestorsAndThis = parents 
                  Raw = []
                  HandlerExpression = None }
        { Data = data; Children = childList }

    let getKey item = item.AncestorsAndThis

    TreeFromList getKey mapBranch archetypeInfoList
