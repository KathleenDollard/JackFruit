module  Generator.LanguageHelpers


open Generator.LanguageExpressions
open Generator.LanguageStatements
open System
open Generator.Language
open Common
open DslKeywords

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
        if not (String.IsNullOrWhiteSpace(s)) then 
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

let GetExpression (value: obj) : IExpression =
    match value with 
    | null -> UnknownLiteralModel.Create("<Error: Actual Null>")
    | :? IExpression as exp -> exp
    | NullLiteral x -> x
    | BoolLiteral x -> x
    | StringLiteral x -> x
    | IntegerLiteral x -> x
    | DoubleLiteral x -> x
    | SymbolLiteral x -> x
    | _ -> UnknownLiteralModel.Create (value.ToString())


type Structural =
    static member Using (usingName: string) =
        UsingModel.Create usingName
    static member Using (usingName: string, ?w:AliasWord, ?alias: string) =
        match alias with 
        | None -> UsingModel.Create usingName
        | Some a ->
            if String.IsNullOrEmpty a then
                UsingModel.Create usingName
            else
                { UsingNamespace = usingName; Alias = Some a }

    static member Public ([<ParamArray>] modifiers: Modifier[]) =
        { Scope = Scope.Public; Modifiers = List.ofArray modifiers }
    static member Private ([<ParamArray>] modifiers: Modifier[]) =
        { Scope = Scope.Private; Modifiers = List.ofArray modifiers }
    static member Internal ([<ParamArray>] modifiers: Modifier[]) =
        { Scope = Scope.Internal; Modifiers = List.ofArray modifiers }
    static member Protected ([<ParamArray>] modifiers: Modifier[]) =
        { Scope = Scope.Protected; Modifiers = List.ofArray modifiers }

    static member InheritsFrom (baseClass: NamedItem) = 
        SomeBase baseClass
    static member ImplementsInterfaces ([<ParamArray>] interfaces: NamedItem[]) = 
        [ for i in interfaces do ImplementedInterface i ]
       

    static member Parameter(name: string, typeName: NamedItem, ?style: ParameterStyle) =
        let newStyle =
            match style with 
            | Some s -> s
            | _ -> Normal
        { ParameterName = name; Type = typeName; Style = newStyle }

module Statements =
    // Since these contain statements, they are Computation Expressions in DslCodeBuilder
    //    * Creating IfModel, ElseIfModel, and ElseModel
    //    * Creating ForEachModel

    let Assign(variable: string) (value: obj) =
        let expression = GetExpression value
        AssignmentModel.Create variable expression

    let AssignWithDeclare (variable: string) (typeName: NamedItem) (value: obj) =
        let expression = GetExpression value
        AssignWithDeclareModel.Create variable (Some typeName) expression

    let AssignWithVar (variable: string) (value: obj) =
        let expression = GetExpression value
        AssignWithDeclareModel.Create variable None expression

    let SimpleCall (expression: IExpression) =
        SimpleCallModel.Create expression

    let Comment (text: string) =
        CommentModel.Create text

    let CompilerDirective (directive: CompilerDirectiveType) =
        CompilerDirectiveModel.Create directive

    let Return (returnValue: IExpression) =
        ReturnModel.Create (Some returnValue)

    let ReturnVoid () =
        ReturnModel.Create None

    let Invoke instance methodName arguments =
        InvocationModel.Create  instance methodName arguments
