module Generator.RoslynCSharpUtils

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

let (|StringLiteralExpression|_|) (n: CSharpSyntaxNode) =
    match n.Kind() with
    | SyntaxKind.StringLiteralExpression ->
        let t = n :?> LiteralExpressionSyntax
        Some()
    | _ -> None


let (|ArgumentSyntax|_|) (n: CSharpSyntaxNode) =
    match n with
    | :? ArgumentSyntax as t -> Some(t.Kind(), t.NameColon, t.RefOrOutKeyword, t.Expression)
    | _ -> None


let (|InvocationExpression|_|) (n: SyntaxNode) =
        match n.Kind() with
        | SyntaxKind.InvocationExpression ->
            let t = n :?> InvocationExpressionSyntax
            Some(t.Expression, t.ArgumentList)
        | _ -> None


let (|IdentifierNameSyntax|_|) (n: CSharpSyntaxNode) =
    match n with
    | :? IdentifierNameSyntax as t -> Some(t.Kind(), t.Identifier)
    | _ -> None


let (|SimpleMemberAccessExpression|_|) (n: CSharpSyntaxNode) =
    match n.Kind() with
    | SyntaxKind.SimpleMemberAccessExpression ->
        let t = n :?> MemberAccessExpressionSyntax
        Some(t.Expression, t.Name)
    | _ -> None


let (|SimpleInvocationByName|_|) (name:string) (node:SyntaxNode) = 
    match node with
    | InvocationExpression (expr, a) -> 
        match expr with 
            | SimpleMemberAccessExpression (c, n) when n.ToString() = name ->
                    Some( c.ToString(), Seq.toList a.Arguments )
            | IdentifierNameSyntax (_, n) when n.ToString() = name -> 
                    Some( "", Seq.toList a.Arguments)
            | _ -> None
    | _ -> None


let rec StringFrom (syntaxNode: CSharpSyntaxNode) =
    match syntaxNode with
    | ArgumentSyntax (_, _, _, expression)  -> StringFrom expression
    | StringLiteralExpression -> syntaxNode.ToFullString()
    | _ -> invalidOp "Only string literals currently supported"


let rec ExpressionFrom syntaxNode =
    match syntaxNode with
    | ArgumentSyntax (_, _, _, expression)  -> expression
    | _ -> invalidOp "Invalid argument syntax"


let InvocationsFrom (syntaxTree: CSharpSyntaxTree) name =
   [ for node in syntaxTree.GetRoot().DescendantNodes() do
        match node with 
        | SimpleInvocationByName name t -> t
        | _ -> ()] 