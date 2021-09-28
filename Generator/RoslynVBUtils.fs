module RoslynVBUtils

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.VisualBasic
open Microsoft.CodeAnalysis.VisualBasic.Syntax

let (|StringLiteralExpression|_|) (n: VisualBasicSyntaxNode) =
    match n.Kind() with
    | SyntaxKind.StringLiteralExpression ->
        let t = n :?> LiteralExpressionSyntax
        Some()
    | _ -> None

let rec StringFrom syntaxNode =
    match syntaxNode with
    //| SimpleArgumentSyntax (_, _, _, expression)  -> StringFrom expression
    | StringLiteralExpression -> syntaxNode.ToFullString()
    | _ -> invalidOp "Only string literals currently supported"


let rec ExpressionFrom syntaxNode =
    match syntaxNode with
    //| SimpleArgumentSyntax (_, _, _, expression)  -> expression
    | _ -> invalidOp "Invalid argument syntax"



let InvocationsFrom (syntaxTree: VisualBasicSyntaxTree) name =
    invalidOp "Not yet implemented"