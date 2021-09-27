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


let ArchetypeInfoTreeFrom (source: Source) = 
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

    let archetypeInfoListResult = ArchetypeInfoListFrom source

    match archetypeInfoListResult with 
    | Ok list -> Generator.GeneralUtils.TreeFromList getKey mapBranch list
    | Error err -> invalidOp "Test failed because archetypeInfo mapping failed"


// TODO: More work is needed because there is more we can get from the parameter
// But it was being difficult and I wanted to get the main logic done. This will 
// need to switch to a record or similar to carry the parameter information. 
let ParametersFromArchetype archetypeInfo model =
    match archetypeInfo.HandlerExpression with 
    | Some handler -> 
        match MethodFromHandler model handler with 
        | Some method -> 
            [ for parameter in method.Parameters do
                parameter.Name, 
                parameter.Type.ToDisplayString() ]
        | None -> invalidOp "Test failed because specified method not found"
    | None -> invalidOp "Test failed because no handler found"


let CommandDefFromSource (syntaxTree: SyntaxTree) model  =
    let argAndOptions (parameters: (string * string) list) parts = 
        let getKey (part: string) = 
            let aliases = part.Trim().Split("|")
            let first = aliases[0]
            first.Replace("-","")
                .Replace("<","")
                .Replace(">","")
                .Replace("[","")
                .Replace("]","")
                |> ToCamel

        let getArg part name typeName =
            ()

        let getOption part name typeName =
            ()

        let isArg symbolDef =
            match symbolDef with 
            | ArgDef a -> true
            | _ -> false

        let lookup = 
            [ for part in parts do
                (getKey part, part)]
            |> Map.ofList

        let combined =
            [ for (name, typeName) in parameters do 
                let partOption = lookup |> Map.tryFind name 
                match partOption with 
                | Some p -> 
                    match p with 
                    | Arg a -> getArg p name typeName           // TODO: Add providers to call 
                    | _ -> getOption partOption name typeName   // TODO: Add providers to call
                | None -> getOption partOption name typeName ]
        let arg = 
            [ for symbol in combined do
                match symbol with 
                | ArgDef a -> Some a
                | _ -> None ]
            |> List.head

        let options = 
            [ for symbol in combined do
                match symbol with 
                | OptionDef o -> o
                | _ -> () ]
        (arg, options)

    // KAD: Why do I have to specify the return here? 
    let rec depthFirstCreate archTree  =
        let subCommands = 
            [ for child in archTree.Children do
                yield depthFirstCreate child ]

        let archetypeInfo = archTree.Data
        let parameters = ParametersFromArchetype archetypeInfo model
        let (arg, options) = argAndOptions parameters archetypeInfo.Raw

        let id = archetypeInfo.AncestorsAndThis[archTree.Data.AncestorsAndThis.Length-1]
        let name = id // TODO: Pass this as default to providers, same for remainder except Options/Arg

        { CommandId = id
          Name = name       // TODO: Pass this as default to providers
          Description = None  // TODO: Pass this as default to providers
          Arg = arg 
          Options = options
          SubCommands = subCommands}

    let source = 
        match syntaxTree with 
        | :? CSharp.CSharpSyntaxTree as tree -> CSharpTree tree
        | :? VisualBasic.VisualBasicSyntaxTree as tree -> VBTree tree
        | _ -> invalidOp "Unexpected item passed as a syntax tree"

    let archTree = ArchetypeInfoTreeFrom source

    [ for topLevelArch in archTree do
        depthFirstCreate topLevelArch ]
    