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

let StringFromExpression syntaxNode =
    match syntaxNode with
    | StringLiteralExpression -> syntaxNode.ToFullString()
    | _ -> invalidOp "Only string literals currently supported"


let InvocationsFrom (syntaxTree: VisualBasicSyntaxTree) name =
    invalidOp "Not yet implemented"