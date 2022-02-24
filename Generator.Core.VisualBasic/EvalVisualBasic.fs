namespace Generator

open Generator.CodeEval
open Microsoft.CodeAnalysis

type EvalVisualBasic() =
    inherit EvalBase("VisualBasic")

    override _.StringFrom syntaxNode : Result<string, AppErrors>  = Error (AppErrors.NotImplemented "Not yet")
    override _.InvocationsFromModel name semantiModel : Result<(string * SyntaxNode list) list, AppErrors> = Error (AppErrors.NotImplemented "Not yet")
    override _.MethodSymbolFromMethodCall semanticModel expression = None
    override _.ExpressionFrom syntaxNode  : Result<SyntaxNode, AppErrors> = Error (AppErrors.NotImplemented "Not yet")
    override _.IsNullLiteral syntaxNode = false
    override _.NamespaceFromdDescendant node semanticModel = ""