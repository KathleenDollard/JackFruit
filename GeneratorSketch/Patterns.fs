namespace GeneratorSketch

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open FSharp.CodeAnalysis.CSharp.RoslynPatterns

module Patterns =

    let (|InvocationExpressionSyntax|_|) (n: SyntaxNode) =
           match n with
           | :? InvocationExpressionSyntax as t -> Some(t.Kind(), t.Expression, t.ArgumentList)
           | _ -> None 

    let (|InvocationExpression|_|) (n: SyntaxNode) =
            match n.Kind() with
            | SyntaxKind.InvocationExpression ->
                let t = n :?> InvocationExpressionSyntax
                Some(t.Expression, t.ArgumentList)
            | _ -> None

          
    type InvocationInfo = {caller: string; methodName: string; args: ArgumentSyntax list }

    let (|SimpleInvocationByName|_|) (name:string) (node:SyntaxNode) = 
        match node with
        | InvocationExpression (expr, a) -> 
            match expr with 
                | SimpleMemberAccessExpression (c, n) when n.ToString() = name ->
                        Some({caller = c.ToString(); methodName = name; 
                            args = Seq.toList a.Arguments })
                | IdentifierNameSyntax (_, n) when n.ToString() = name -> 
                        Some({caller = ""; methodName = name;
                            args = Seq.toList a.Arguments})
                | _ -> None
        | _ -> None

    
    let memberInvocations (syntaxTree: SyntaxTree) =
        [ for node in syntaxTree.GetRoot().DescendantNodes() do
             match node with 
             | SimpleInvocationByName "MapInferred" t -> t
             | _ -> ()]

