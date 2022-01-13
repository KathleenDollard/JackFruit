module Generator.RoslynUtils

open Microsoft.CodeAnalysis
open Generator

type Source =
    | CSharpTree of SyntaxTree
    | CSharpCode of string
    | VBTree of SyntaxTree
    | VBCode of string

let MethodSymbolFromMethodDeclaration (model: SemanticModel) (expression: SyntaxNode) =
    let handler =
        model.GetDeclaredSymbol expression

    let symbol =
        match handler with
        | null -> invalidOp "Delegate not found"
        | _ -> handler

    match symbol with
    | :? IMethodSymbol as m -> Some m
    | _ -> None


let MethodSymbolFromMethodCall (model: SemanticModel) (expression: SyntaxNode) =
    let handler =
        model.GetSymbolInfo expression

    match handler.Symbol with 
    | null when handler.CandidateSymbols.IsEmpty -> None
    | null -> 
        match handler.CandidateSymbols.[0] with 
        | :? IMethodSymbol as m -> Some m
        | _ -> None
    | _ -> 
        match handler.Symbol with
        | :? IMethodSymbol as m -> Some m
        | _ -> None
