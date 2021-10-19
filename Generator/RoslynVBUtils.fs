module RoslynVBUtils

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.VisualBasic
open Microsoft.CodeAnalysis.VisualBasic.Syntax
open Generator.Models
open Generator.AppErrors


let (|StringLiteralExpression|_|) (n: VisualBasicSyntaxNode) =
    match n.Kind() with
    | SyntaxKind.StringLiteralExpression ->
        let t = n :?> LiteralExpressionSyntax
        Some()
    | _ -> None

let rec StringFrom (syntaxNode: VisualBasicSyntaxNode) =
    match syntaxNode with
    //| ArgumentSyntax (_, _, _, expression)  -> StringFrom expression
    | StringLiteralExpression -> Ok (syntaxNode.ToFullString())
    | _ -> Error (NotImplemented "Only string literals currently supported")



let rec ExpressionFrom syntaxNode =
    match syntaxNode with
    //| SimpleArgumentSyntax (_, _, _, expression)  -> expression
    | _ -> invalidOp "Invalid argument syntax"



let InvocationsFrom (syntaxTree: VisualBasicSyntaxTree) name =
    invalidOp "Not yet implemented"