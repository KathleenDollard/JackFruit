module Generator.RoslynUtils

open Microsoft.CodeAnalysis
open Generator


type Source =
    | CSharpTree of SyntaxTree
    | CSharpCode of string
    | VBTree of SyntaxTree
    | VBCode of string



let SyntaxTreeResult (source: Source) =
    let tree =
        match source with
        | CSharpTree tree -> tree
        | CSharpCode code -> (Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText code)
        | VBTree tree -> tree
        | VBCode code -> (Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxTree.ParseText code)

    let errors =
        [ for diag in tree.GetDiagnostics() do
            if diag.Severity = DiagnosticSeverity.Error then diag ]

    if errors.IsEmpty then
        Ok tree
    else
        Error (Roslyn errors)


let StringFrom (node: SyntaxNode) =
    match node with 
    | :? CSharp.CSharpSyntaxNode as cSharpNode-> RoslynCSharpUtils.StringFrom cSharpNode
    | :?  VisualBasic.VisualBasicSyntaxNode as vbNode -> RoslynVBUtils.StringFrom vbNode
    | _ -> invalidOp "Unexpected type"


let ExpressionFrom (node: SyntaxNode)  =
    match node with 
    | :? CSharp.CSharpSyntaxNode as cSharpNode-> RoslynCSharpUtils.ExpressionFrom cSharpNode
    | :?  VisualBasic.VisualBasicSyntaxNode as vbNode -> RoslynVBUtils.ExpressionFrom vbNode
    | _ -> invalidOp "Invalid node type"


let InvocationsFrom name (syntaxTree: SyntaxTree) =
    match syntaxTree with 
    | :? CSharp.CSharpSyntaxTree as tree -> RoslynCSharpUtils.InvocationsFrom tree name
    | :? VisualBasic.VisualBasicSyntaxTree as tree -> RoslynVBUtils.InvocationsFrom tree name
    | _ -> invalidOp "Invalid node type"


let MethodDeclarationsFrom (syntaxTree: SyntaxTree) = 
    match syntaxTree with 
    | :? CSharp.CSharpSyntaxTree as tree -> RoslynCSharpUtils.MethodDeclarationsFrom tree
    | :? VisualBasic.VisualBasicSyntaxTree as tree -> RoslynVBUtils.MethodDeclarationFrom tree
    | _ -> invalidOp "Invalid node type"

let MethodDeclarationNodesFrom (syntaxTree: SyntaxTree) = 
    match syntaxTree with 
    | :? CSharp.CSharpSyntaxTree as tree -> RoslynCSharpUtils.MethodDeclarationNodesFrom tree
    | :? VisualBasic.VisualBasicSyntaxTree as tree -> RoslynVBUtils.MethodDeclarationNodesFrom tree
    | _ -> invalidOp "Invalid node type"

let IsNullLiteral (expression: SyntaxNode) =
    match expression with 
    | :? CSharp.CSharpSyntaxNode as node-> RoslynCSharpUtils.IsNullLiteral node
 //   | :? VisualBasic.VisualBasicSyntaxNode as node -> RoslynVBUtils.IsNullLiteral node
    | _ -> invalidOp "Invalid node type"


let InvocationsFromModel name (model:SemanticModel) =
    InvocationsFrom name model.SyntaxTree

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