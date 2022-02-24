namespace Generator

open Generator.CodeEval
open Microsoft.CodeAnalysis
open Generator.RoslynCSharpUtils

type EvalCSharp() =
    inherit EvalBase("C#")

    override _.StringFrom syntaxNode : Result<string, AppErrors>  = // Error (AppErrors.NotImplemented "Not yet")
        match syntaxNode with 
        | :? CSharp.CSharpSyntaxNode as cSharpNode-> StringFrom cSharpNode
        | _ -> invalidOp "Unexpected type"
    
    override _.InvocationsFromModel names semantiModel = // Error (AppErrors.NotImplemented "Not yet")
        let syntaxTree = semantiModel.SyntaxTree
        Ok 
            [ for node in syntaxTree.GetRoot().DescendantNodes() do
                match node with 
                | SimpleInvocationByName names (caller, argList) 
                    -> (caller, 
                        [for arg in argList do arg :> SyntaxNode])
                | _ 
                    -> ()]

    override _.MethodSymbolFromMethodCall semanticModel expression = // None
        let handler =
            semanticModel.GetSymbolInfo expression

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

    override _.ExpressionFrom syntaxNode  : Result<SyntaxNode, AppErrors> = // Error (AppErrors.NotImplemented "Not yet")
        match syntaxNode with 
        | :? CSharp.CSharpSyntaxNode as cSharpNode-> RoslynCSharpUtils.ExpressionFrom cSharpNode
        | _ -> invalidOp "Invalid node type"
        
    override _.IsNullLiteral syntaxNode = //false
        match syntaxNode with 
        | :? CSharp.CSharpSyntaxNode as node-> RoslynCSharpUtils.IsNullLiteral node
        | _ -> invalidOp "Invalid node type"

    override _.NamespaceFromdDescendant node semanticModel = 
        let info = semanticModel.GetSymbolInfo node
        let symbol = info.Symbol

        match symbol with 
        | :? INamedTypeSymbol as s ->
            s.ContainingNamespace.ToString()
        | _ -> ""
