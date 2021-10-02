﻿module Generator.ArchetypeMapping

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

type CommandArchetype = 
    { CommandId: string
      Name: string
      Aliases: string list
      HiddenAliases: string list }

type OptionArchetype = 
    { OptionId: string
      Name: string
      Aliases: string list
      HiddenAliases: string list }

type ArgArchetype = 
    { ArgId: string
      Name: string
      Aliases: string list }

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
    let words = part.Trim().Split('|')
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
    { CommandId = id
      Name = name
      Aliases = aliases
      HiddenAliases = hiddenAliases}


let OptionArchetypeFrom (part: string) =
    let part = part.Replace("-","")
    let ( id, name, aliases, hiddenAliases ) = ParseArechetypePart part
    { OptionId = id
      Name = name
      Aliases = aliases
      HiddenAliases = hiddenAliases}

let ArgArchetypeFrom part =
    let part = RemoveSurroundingAngleBrackets part
    let ( id, name, aliases, hiddenAliases ) = ParseArechetypePart part
    { ArgId = id
      Name = name
      Aliases = aliases}


let ParseArchetypeInfo archetype handler =
    let stringSplitOptions =
        System.StringSplitOptions.RemoveEmptyEntries
        ||| System.StringSplitOptions.TrimEntries

    let parts =
        (RemoveSurroundingDoubleQuote archetype)
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
