module  Generator.LanguageHelpers

open Generator.LanguageExpressions
open System
open Generator.Language

let (|NullLiteral|_|)  (value: obj) =
    match value with 
    | :? string as s-> 
        if String.IsNullOrWhiteSpace(s) then 
            Some (NullModel.Create())
        else None
    | _ -> None

let (|BoolLiteral|_|)  (value: obj) =
    match value with 
    | :? string as s-> 
        match (s.Trim().ToUpperInvariant()) with
        | "TRUE" -> Some (BoolLiteralModel.Create(true))
        | "FALSE" -> Some (BoolLiteralModel.Create(false))
        | _ -> None
    | _ -> None

let (|StringLiteral|_|) (value: obj) =
    match value with 
    | :? string as s-> 
        let s = s.Trim()
        if String.IsNullOrWhiteSpace(s) then 
            None 
        else
            match s[0] with
            | '"' -> Some (StringLiteralModel.Create(s[0..]))
            | _ -> None     
    | _ -> None   
    
let (|IntegerLiteral|_|) (value: obj) =   // Long could be supported instead, some perf hit I assume
    match value with 
    | :? string as s->
        match Int32.TryParse(s) with
        | true, v -> Some (OtherLiteralModel.Create(v.ToString())) 
        | _ -> None
    | _ -> None   

let (|DoubleLiteral|_|) (value: obj) =
    match value with 
    | :? string as s->
        match Double.TryParse(s) with
        | true, v -> Some (OtherLiteralModel.Create(v.ToString()))
        | _ -> None
    | _ -> None   

let (|SymbolLiteral|_|) (value: obj) = 
    let allValidChars (s: string) =
        if Char.IsDigit(s[0]) then 
            false
        else
            [ let mutable found = false
              for c in s do
                if c = '_' || Char.IsLetterOrDigit(c) then 
                    () // all is well
                else 
                    found <- true
                    c
              ].IsEmpty

    match value with 
    | null -> None
    | :? string as s -> // probably unnecessary optimization
        if String.IsNullOrWhiteSpace(s) then 
            match s with 
            | NullLiteral _ 
            | BoolLiteral _
            | StringLiteral _ 
            | IntegerLiteral _
            | DoubleLiteral _ -> None
            | _ when allValidChars s -> Some (SymbolModel.Create(s))
            | _ -> None
        else None
    | _ -> None

let GetLiteral (value: obj) : IExpression =
    match value with 
    | NullLiteral x -> x
    | BoolLiteral x -> x
    | StringLiteral x -> x
    | IntegerLiteral x -> x
    | DoubleLiteral x -> x
    | SymbolLiteral x -> x
    | _ -> UnknownLiteralModel.Create (value.ToString())