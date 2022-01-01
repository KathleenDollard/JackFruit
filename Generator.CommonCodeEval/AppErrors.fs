namespace Generator

open Microsoft.CodeAnalysis

type AppErrors =
    | Roslyn of Diagnostics: Diagnostic list
    | UnexpectedExpression of Message: string
    | UnexpectednumberOfArguments
    | BuildingCommanDef of Message: string
    | NotImplemented of Message: string
    | Other of Message: string
    | Aggregate of Errors: AppErrors list
    | AppModelIssue of Message: string
    | RoslynOutIssue of Message: string


