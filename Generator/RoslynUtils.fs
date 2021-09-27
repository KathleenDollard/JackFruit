module Generator.RoslynUtils

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.VisualBasic

type Source =
    | CSharpTree of SyntaxTree
    | CSharpCode of string
    | VBTree of SyntaxTree
    | VBCode of string



let SyntaxTreeResult (source: Source) =
    let tree =
        match source with
        | CSharpTree tree -> tree
        | CSharpCode code -> (CSharpSyntaxTree.ParseText code)
        | VBTree tree -> tree
        | VBCode code -> (VisualBasicSyntaxTree.ParseText code)

    let errors =
        [ for diag in tree.GetDiagnostics() do
            if diag.Severity = DiagnosticSeverity.Error then
                    diag ]

    if errors.IsEmpty then
        Ok tree
    else
        Error errors


let StringFromExpression (node:  SyntaxNode) =
    match node with 
    | :? CSharpSyntaxNode as cSharpNode-> RoslynCSharpUtils.StringFromExpression cSharpNode
    | :? VisualBasicSyntaxNode as vbNode -> RoslynVBUtils.StringFromExpression vbNode
    | _ -> invalidOp "Unexpected type"


let InvocationsFrom (syntaxTree: SyntaxTree) name =
    match syntaxTree with 
    | :? CSharpSyntaxTree as tree-> RoslynCSharpUtils.InvocationsFrom tree name
    | :? VisualBasicSyntaxTree as tree -> RoslynVBUtils.InvocationsFrom tree name
    | _ -> invalidOp "Unexpected type"

let MethodFromHandler (model: SemanticModel) (expression: SyntaxNode) =
    let handler =
        model.GetSymbolInfo expression

    let symbol =
        match handler.Symbol with
        | null when handler.CandidateSymbols.IsDefaultOrEmpty -> invalidOp "Delegate not found"
        | null -> handler.CandidateSymbols.[0]
        | _ -> handler.Symbol

    match symbol with
    | :? IMethodSymbol as m -> Some m
    | _ -> None
