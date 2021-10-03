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

// Support these syntaxes
//
// Arg is singular for now because management of an item is different than a list and I want that experience
//
// one two three <arg> --option1 <optionArg> --option2
// one|longOrShortOne --option1|o
//
// I want to support hidden args in the future. For that, square brackets. 
// This is for deprecation and handler mismatches, and may help a common case where option names don't match their arg names
// System.CommandLine does not yet support this. 
//
// one|[old] --option1|o|[old]

let (|Command|Arg|Option|) (part: string) =
    let part = part.Trim()

    if part.Length = 0 then
        Command part
    else
        match part.[0] with
        | '<' -> Arg part
        | '-' -> Option part
        | _ -> Command part

let private stringSplitOptions =
     System.StringSplitOptions.RemoveEmptyEntries
     ||| System.StringSplitOptions.TrimEntries

let private FirstNonHiddenWord words =
    let mutable name = ""
    for word:string in words do
        if name = "" then
            if word[0] <> '[' then
                name <- word
    if name = "" then
        None
    else
        Some name

let private ParseArechetypePart (part: string) =
    let words = part.Trim().Split('|', stringSplitOptions)
    let id = RemoveSurroundingSquareBrackets words[0]
    let firstNonHidden = FirstNonHiddenWord words
    let name = 
        match firstNonHidden with
        | Some name -> name
        | None -> id
    let aliases = [
        // KAD-Don: Is there a difference between do and ->
        for word in words do
            if word[0] <> '[' && word <> name then
                word ]
    let hiddenAliases = [
        for word in words do
            if word[0] = '[' then
                RemoveSurroundingSquareBrackets word ]
    
    ( id, name, aliases, hiddenAliases )

let CommandArchetypeFrom part =
    let ( id, name, aliases, hiddenAliases ) = ParseArechetypePart part
    CommandArchetype 
        { Id = id
          Name = name
          Aliases = aliases
          HiddenAliases = hiddenAliases}

let OptionArchetypeFrom (part: string) =
    let part = part.Replace("-","")
    let ( id, name, aliases, hiddenAliases ) = ParseArechetypePart part
    OptionArchetype 
        { Id = id
          Name = name
          Aliases = aliases
          HiddenAliases = hiddenAliases}

let ArgArchetypeFrom part =
    let part = RemoveSurroundingAngleBrackets part
    let ( id, name, aliases, hiddenAliases ) = ParseArechetypePart part
    ArgArchetype 
        { Id = id
          Name = name
          Aliases = aliases
          HiddenAliases = hiddenAliases}

let ParseArchtypeParts archetype =
    let words =
        (RemoveSurroundingDoubleQuote archetype)
            .Split(' ', stringSplitOptions)
        |> Array.toList
    let rawParts = [ for word in words do 
        match word with 
        | Arg x -> ArgArchetypeFrom x
        | Option x -> OptionArchetypeFrom x
        | Command x -> CommandArchetypeFrom x ]


let ParseArchetypeInfo archetype handler =
    let parts = ParseArchtypeParts archetype

    let commandNames = 
        [ for part in parts do 
            match part with 
            | CommandArchetype {Name=n} -> n
            | _ -> ()]

    { Path = 
        // Root command name is generally empty
        if commandNames.IsEmpty then
            [""]
        else
            commandNames
      ArchetypeParts = parts
      Handler = handler }


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
                { Path = parents 
                  ArchetypeParts = []
                  Handler = None }
        { Data = data; Children = childList }

    let getKey item = item.Path

    TreeFromList getKey mapBranch archetypeInfoList
