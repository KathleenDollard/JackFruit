module Generator.ArchetypeMapping

open Generator.GeneralUtils
open Generator.Models
open RoslynUtils


let private  (|Command|Arg|Option|) (part: string) =
    let part = part.Trim()

    if part.Length = 0 then
        Command part
    else
        match part.[0] with
        | '<' -> Arg part
        | '-' -> Option part
        | _ -> Command part


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


let ArchetypeInfoListFrom (source: Source) =
    let archetypesFromInvocations syntaxTree =
        let invocations = InvocationsFrom syntaxTree "MapInferred"

        // KAD: Why the type mismatch exception here. Expression inherits from SyntaxNode
        [ for invoke in invocations do
              match invoke with
              | (_, [ a; d ]) ->
                  ParseArchetypeInfo 
                    (StringFromExpression a.Expression) 
                    (if (d.Expression = null) then None else Some d.Expression)
              | _ -> () ]

    match SyntaxTreeResult source with
    | Ok tree -> Ok(archetypesFromInvocations tree)
    | Error errors -> Error errors

let mapBranch parents item childList=
    let data = 
        match item with 
        | Some i -> i
        | None -> 
            { AncestorsAndThis = parents 
              Raw = []
              HandlerExpression = None }
    { Data = data; Children = childList }

let getKey item = item.AncestorsAndThis

let ArchetypeInfoTreeFrom (source: Source) = 
    let archetypeInfoListResult = ArchetypeInfoListFrom source


    match archetypeInfoListResult with 
    | Ok list -> Generator.GeneralUtils.TreeFromList getKey mapBranch list
    | Error err -> invalidOp "Test failed because archetypeInfo mapping failed"

 
