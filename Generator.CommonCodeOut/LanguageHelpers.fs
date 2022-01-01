module  Generator.LanguageHelpers


open Generator.LanguageExpressions
open Generator.LanguageStatements
open System
open Generator.Language
open Common
open DslKeywords

let (|NullLiteral|_|)  (value: obj) : IExpression option =
    match value with 
    | :? string as s-> 
        if String.IsNullOrWhiteSpace(s) then 
            Some LiteralsModel.NullLiteral
        else None
    | _ -> None

let (|ExplicitLiteral|_|)  (value: obj) : IExpression option =
    match value with 
    | :? string as s-> 
        match (s.Trim().ToUpperInvariant()) with
        | "TRUE" -> Some TrueLiteral
        | "FALSE" -> Some FalseLiteral
        | "NULL" -> Some NullLiteral
        | _ -> None
    | _ -> None


let (|StringLiteral|_|) (value: obj) : IExpression option =
    match value with 
    | :? string as s-> 
        let s = s.Trim()
        if String.IsNullOrWhiteSpace(s) then 
            None 
        else
            match s[0] with
            | '"' -> Some (LiteralsModel.StringLiteral (s.Replace("\"", "")))
            | _ -> None     
    | _ -> None   
    
let (|IntegerLiteral|_|) (value: obj) : IExpression option =   // Long could be supported instead, some perf hit I assume
    match value with 
    | :? int as i -> Some (LiteralsModel.IntegerLiteral i)
    | :? string as s ->
        match Int32.TryParse(s) with
        | true, v -> Some (LiteralsModel.IntegerLiteral v)
        | _ -> None
    | _ -> None   

let (|DoubleLiteral|_|) (value: obj) : IExpression option =
    match value with 
    | :? double as d -> Some (LiteralsModel.DoubleLiteral d)
    | :? string as s->
        match Double.TryParse(s) with
        | true, v -> Some (LiteralsModel.DoubleLiteral v)
        | _ -> None
    | _ -> None   

let (|SymbolLiteral|_|) (value: obj) : IExpression option = 
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
    | :? string as s -> 
        let s = s.Trim()
        if (String.IsNullOrWhiteSpace(s)) then
            None
        elif (Char.IsDigit(s[0])) then
            None
        else
            match s with 
            | NullLiteral _ 
            | ExplicitLiteral _
            | StringLiteral _ -> None
            | _ when allValidChars s -> Some (LiteralsModel.SymbolLiteral (Symbol s))
            | _ -> None
    | _ -> None

let Literal (value: obj) : IExpression =
    match value with 
    | null -> UnknownLiteral "<Error: Actual Null>"
    | :? IExpression as exp -> exp
    | NullLiteral x -> x
    | ExplicitLiteral x -> x
    | StringLiteral x -> x
    | IntegerLiteral x -> x
    | DoubleLiteral x -> x
    | SymbolLiteral x -> x
    | _ -> UnknownLiteral value


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

    static member ReturnType (returnType: ReturnType) =
        

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
    let Assign(variable: string) (_: ToWord) (value: obj) =
        let expression = Literal value
        AssignmentModel.Create variable expression

    let AssignWithDeclare (variable: string) (_: ToWord) (typeName: NamedItem) (value: obj) =
        let expression = Literal value
        AssignWithDeclareModel.Create variable (Some typeName) expression

    let AssignWithVar (variable: string) (_: ToWord)  (value: obj) =
        let expression = Literal value
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
