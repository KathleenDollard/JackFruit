module Generator.RoslynCSharpUtils

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open Generator.Models
open Generator


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

//let (|MethodDeclaration|_|) (name: string) (node:SyntaxNode) =
//    match node with
//    | :? MethodDeclarationSyntax as n ->
//        Some (n.AttributeLists |> Seq.toList, n.ReturnType, n.ExplicitInterfaceSpecifier, n.Identifier, n.TypeParameterList, n.ParameterList, n.ConstraintClauses |> Seq.toList, n.Body, n.ExpressionBody, n.SemicolonToken, n.Arity)
//    | _ -> None


let (|MethodDeclaration|_|) (node:SyntaxNode) =
    match node.Kind() with 
    | SyntaxKind.MethodDeclaration ->
        let t = node :?> MethodDeclarationSyntax
        let name = t.Identifier.ToString()
        Some(name, Seq.toList t.ParameterList.Parameters)
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

let (|MethodDeclarationByName|_|) (matchName: string) (node:SyntaxNode) =
    match node with 
    | MethodDeclaration (name, parameters) ->
        if name = matchName then
            Some(name, parameters)
        else
            None
    | _ -> None


let (|NullLiteralExpression|_|) (n: CSharpSyntaxNode) =
    match n.Kind() with
    | SyntaxKind.NullLiteralExpression ->
        let t = n :?> LiteralExpressionSyntax
        Some()
    | _ -> None


let IsNullLiteral (expression: CSharpSyntaxNode) =
    match expression with 
    | NullLiteralExpression-> true
    | _ -> false


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
   Ok [ for node in syntaxTree.GetRoot().DescendantNodes() do
        match node with 
        | SimpleInvocationByName name (caller, argList) 
            -> (caller, 
                [for arg in argList do arg :> SyntaxNode])
        | _ 
            -> ()] 

let MethodDeclarationsFrom (syntaxTree: CSharpSyntaxTree) = 
    Ok 
        [ for node in syntaxTree.GetRoot().DescendantNodes() do
           match node with 
           | MethodDeclaration (name, parameters) ->
                (name, parameters)
           | _ -> ()] 

let MethodDeclarationNodesFrom (syntaxTree: CSharpSyntaxTree) = 
    Ok 
        [ for node in syntaxTree.GetRoot().DescendantNodes() do
           match node with 
           | MethodDeclaration (_, _) -> node
           | _ -> () ] 

let MethodDeclarationsByNameFrom matchName (syntaxTree: CSharpSyntaxTree) = 
    Ok 
        [ for node in syntaxTree.GetRoot().DescendantNodes() do
           match node with 
           | MethodDeclarationByName matchName (name, parameters) ->
                (name, parameters)
           | _ -> ()] 