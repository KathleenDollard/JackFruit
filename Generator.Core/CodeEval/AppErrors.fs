namespace Generator

open Microsoft.CodeAnalysis
open System

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

    override this.ToString() =
        match this with
        | Roslyn diagnostics-> $"Roslyn input issue: {diagnostics}"
        | UnexpectedExpression message -> $"Unexpected expression: {message}"
        | UnexpectednumberOfArguments -> "Unexpected number of arguments"
        | BuildingCommanDef message-> $"Error building CommandDef: {message}"
        | NotImplemented message-> $"Not implemented: {message}"
        | Other message-> $"Other: {message}"
        | Aggregate errors-> $"Aggregate errors: {errors}"
        | AppModelIssue message-> $"Error with AppModel: {message}"
        | RoslynOutIssue message-> $"Issue outputting Roslyn: {message}"

    static member CreateErrorListeFromResults (results: Result<'T, AppErrors> list) =
        [ for r in results do
            match r with 
            | Ok _ -> ()
            | Error err -> err ]
