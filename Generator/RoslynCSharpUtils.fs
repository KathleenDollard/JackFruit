module Generator.RoslynCSharpUtils

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open Generator.Models

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
    // KAD: Why do I need the parens around the syntaxNode.ToFullString()? 
    | StringLiteralExpression -> Ok (syntaxNode.ToFullString())
    | _ -> Error (NotImplemented "Only string literals currently supported")


let ExpressionFrom (syntaxNode: CSharpSyntaxNode) =
    match syntaxNode with
    | ArgumentSyntax (_, _, _, expression)  -> Ok (expression :> SyntaxNode)
    | _ -> Error (UnexpectedExpression "Argument not passed")


let InvocationsFrom (syntaxTree: CSharpSyntaxTree) name =
   [ for node in syntaxTree.GetRoot().DescendantNodes() do
        match node with 
        | SimpleInvocationByName name (caller, argList) 
            -> (caller, 
                [for arg in argList do arg :> SyntaxNode])
        | _ 
            -> ()] 