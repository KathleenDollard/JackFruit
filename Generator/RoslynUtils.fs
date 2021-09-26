module Generator.RoslynUtils

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open Microsoft.CodeAnalysis.VisualBasic
open Microsoft.CodeAnalysis.VisualBasic.Syntax

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


