module Jackfruit.ArchetypeMapping


open Generator.GeneralUtils
open Jackfruit.Models
open Generator.RoslynUtils
open Microsoft.CodeAnalysis
open Generator


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
        if name = "" && word.Length > 0 then
            if word[0] <> '[' then
                name <- word 
    if name = "" then
        None
    else
        Some name

let private ParseArechetypePart (part: string) =
    let words = part.Trim().Split('|', stringSplitOptions)
    let id = 
        if words.Length > 0 then
            RemoveSurroundingSquareBrackets words[0]
        else
            ""
    let firstNonHidden = FirstNonHiddenWord words
    let name = 
        match firstNonHidden with
        | Some name -> name
        | None -> id
    let aliases = [
        for word in words do
            if word.Length > 0 && word[0] <> '[' && word <> name then
                word ]
    let hiddenAliases = [
        for word in words do
            if word.Length > 0 && word[0] = '[' then
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
    let argOptions =
        [ for word in words do 
            match word with 
            | Arg x -> ArgArchetypeFrom x
            | Option x -> OptionArchetypeFrom x
            | _ -> ()]
    let commands = 
        [ for word in words do 
            match word with 
            | Command x -> CommandArchetypeFrom x
            | _ -> ()]
        |> UseDefaultIfEmpty (CommandArchetypeFrom "")

    (commands, argOptions)


let ParseArchetypeInfo archetype handler =
    let (commandParts, argOptionParts) = ParseArchtypeParts archetype

    let commandNames = 
        [ for part in commandParts do 
            match part with 
            | CommandArchetype {Name=n} -> n
            | _ -> invalidOp "This list should only contain CommandArchetype" ]

    let command = commandParts |> List.last

    { Path = 
        // Root command name is generally empty
        if commandNames.IsEmpty then
            [""]
        else
            commandNames
      ArchetypeParts = command::argOptionParts
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
                    | Error _, _ ->  Error (Generator.AppModelIssue $"Unexpected expression for frchetype at {pos}")
                    | Ok arch, Error _ ->  Error (Generator.AppModelIssue $"Unexpected expression for handler {arch}")
                | _ -> Error Generator.UnexpectednumberOfArguments ]
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
        | _ -> Error (Generator.Aggregate errors)

let ArchetypeInfoTreeFrom (archetypeInfoList: ArchetypeInfo list): TreeNodeType<ArchetypeInfo> list = 
    let mapBranch parents item childList=
        let data = 
            match item with
            | Some x -> x
            | None -> 
                { Path = parents 
                  ArchetypeParts = []
                  Handler = None }
        { Data = data; Children = childList }

    let getKey (item: ArchetypeInfo) = item.Path

    TreeFromList getKey mapBranch archetypeInfoList
