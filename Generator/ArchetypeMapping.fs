module Generator.ArchetypeMapping

open Generator.GeneralUtils
open Generator.Models
open RoslynUtils
open Microsoft.CodeAnalysis


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


let private ArchetypeInfoListFromSyntaxTree (syntaxTree: SyntaxTree) =
    let archetypesFromInvocations syntaxTree =
        let invocations = InvocationsFrom syntaxTree "MapInferred"
        [ for invoke in invocations do
              match invoke with
              | (_, [ a; d ]) ->
                  ParseArchetypeInfo 
                    (StringFromExpression a.Expression) 
                    (if (d.Expression = null) then None else Some d.Expression)
              | _ -> () ]
    archetypesFromInvocations syntaxTree


let ArchetypeInfoListFrom (source: Source) =
    match SyntaxTreeResult source with
    | Ok syntaxTree -> Ok (ArchetypeInfoListFromSyntaxTree syntaxTree)
    | Error errors -> Error errors


let private mapBranch parents item childList=
    let data = 
        match item with 
        | Some i -> i
        | None -> 
            { AncestorsAndThis = parents 
              Raw = []
              HandlerExpression = None }
    { Data = data; Children = childList }

let private getKey item = item.AncestorsAndThis

let private ArchetypeInfoTreeFromSyntaxTree (syntaxTree: SyntaxTree) = 
    let list = ArchetypeInfoListFromSyntaxTree syntaxTree
    TreeFromList getKey mapBranch list

let ArchetypeInfoTreeFrom (source: Source) = 
    let archetypeInfoListResult = ArchetypeInfoListFrom source
    match archetypeInfoListResult with 
    | Ok list -> TreeFromList getKey mapBranch list
    | Error err -> invalidOp "Test failed because archetypeInfo mapping failed"

let ArchetypeAndParameters archetypeInfo model =
    match archetypeInfo.HandlerExpression with 
    | Some handler -> 
        match MethodFromHandler model handler with 
        | Some method -> 
            [ for parameter in method.Parameters do
                parameter.Name, 
                parameter.Type.ToDisplayString() ]
        | None -> invalidOp "Test failed because specified method not found"
    | None -> invalidOp "Test failed because no handler found"


let CommandDefFromSource (syntaxTree: SyntaxTree) model
    let archTree = ArchetypeInfoTreeFromSyntaxTree syntaxTree
    