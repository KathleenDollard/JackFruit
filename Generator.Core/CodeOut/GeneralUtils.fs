module Generator.GeneralUtils 

open System
open System.Text

/// Removes leading and trailing characters
let RemoveLeadingTrailing startChar endChar (input:string) =
    if input = null then
        null
    else
        let input = input.Trim()

        if input.Length < 2 then
            input
        elif input.[0] = startChar && input.[input.Length - 1] = endChar then
            input.[1..input.Length-2]
        else
            input


/// Removes leading and trailing double 
// TODO: Solve naming challenge: RemoveLeadingTrailing was too long. UnSurround isn't a word
let RemoveSurroundingDoubleQuote (input:string) =
    RemoveLeadingTrailing '"' '"' input

let RemoveSurroundingAngleBrackets (input:string) =
    RemoveLeadingTrailing '<' '>' input

let RemoveSurroundingSquareBrackets(input:string) =
    RemoveLeadingTrailing '[' ']' input

let UseDefaultIfEmpty defaultValue (list: 'a list) =
    if list.IsEmpty then 
        [ defaultValue ]
    else 
        list

let private RemoveCharsAndUpper (remove: char list) (input:string) =
    let charIsSpecial c = remove |> List.contains c
    match input with 
    | null -> null
    | "" -> ""
    | _ ->
        let mutable lastIsSpecial = false
        new string 
            [| for c in input do
                if charIsSpecial c then
                    lastIsSpecial <- true
                    ()
                elif lastIsSpecial then
                    lastIsSpecial <- false
                    System.Char.ToUpper c
                else
                    System.Char.ToLower c |]

/// Adds character between words. Sequential upper case is treated as a word. 
/// where 'HTTPResult' would be 'http_result'
let private AddCharsAndLower (add:string) (input: string) =
    let lastWasUpper i =
        if i > 0 then
            System.Char.IsUpper(input.[i-1])
        else
            false
    let nextIsLower i =
        if i < input.Length - 2 then
            System.Char.IsLower(input.[i+1])
        else
            false
    match input with 
    | null -> null
    | "" -> ""
    | _ ->
        let x = [
            for i in (0) .. (input.Length - 1) do
                let c = input[i]
                let thisIsUpper = System.Char.IsUpper(c)
                let partOfCappedWord = (lastWasUpper i) && not (nextIsLower i)
                if i <> 0 && thisIsUpper && (not partOfCappedWord) then
                    add + System.Char.ToLower(c).ToString()
                elif thisIsUpper then
                    System.Char.ToLower(c).ToString()
                else
                    c.ToString()]
        String.Join("", x)

let ToCamel input =
    RemoveCharsAndUpper ['-'; '_'] input

let ToPascal input =
    match input with 
    | null -> null
    | "" -> ""
    | _ ->
        let camel = RemoveCharsAndUpper ['-'; '_'] input
        let first = System.Char.ToUpper(camel[0]).ToString()
        first + camel[1..]

let ToSnake input =
    AddCharsAndLower "_" input

let ToKebab input =
    AddCharsAndLower "-" input

type System.String with 
    member this.SubstringBefore(lookFor: string, input: string) =
        let pos = input.IndexOf(lookFor)
        input[0..pos - 1]
    member this.SubstringAfter(lookFor: string, input: string) =
        let pos = input.IndexOf(lookFor)
        input[pos..]



