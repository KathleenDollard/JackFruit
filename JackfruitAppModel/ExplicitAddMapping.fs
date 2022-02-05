namespace Jackfruit

open Generator.CodeEval
open Generator.GeneralUtils
open Microsoft.CodeAnalysis
open Common
open Generator
open System

type ExplicitAddInfo =
    { Path: string list 
      Handler: SyntaxNode option }


module ExplicitAddMapping =
    let GetNamePatterns defaultPatterns (evalLanguage: EvalBase) semanticModel =
        // We are not checking the caller. If there is another use of these method name on another class
        // with a single string argument, we will use it as a pattern

        let patternsFromSyntaxNodes list =
            [ for item in list do
                match item with 
                | _, [arg] -> 
                    let patternResult = 
                        evalLanguage.ExpressionFrom arg
                        |> Result.bind evalLanguage.StringFrom
                
                    // KAD-Don: This seems to be something I repeat. Do some stuff with results and then add 
                    //          to a list if it is all OK. Is there a way to shortcut this? I don't think I     
                    //          can have a function that returns a value or unit.
                    match patternResult with
                    | Ok pattern -> pattern.Replace("\"", "")
                    | Error _ -> () 
                | _ -> () ]

        let patternsFromInvocations methodName =
            let invocations = evalLanguage.InvocationsFromModel methodName semanticModel
            let result = evalLanguage.InvocationsFromModel methodName semanticModel
            match result with
            | Ok list -> patternsFromSyntaxNodes list
            | Error _ -> []

        let addPatterns = patternsFromInvocations "AddCommandNamePattern"
        let removePatterns = patternsFromInvocations "RemoveCommandNamePattern"
        let patterns = 
            defaultPatterns @ addPatterns
            |> List.except removePatterns
        patterns

    let GetPathAndHandler methodName (evalLanguage: EvalBase) semanticModel =
        let commandPairsResult = evalLanguage.InvocationsFromModel methodName semanticModel
        match commandPairsResult with
        | Ok commandPairs -> 
            let pairResults = 
                [ for commandPair in commandPairs do
                  match commandPair with
                  | (name, [handler]) -> Ok (name, handler)
                  | (name, _) -> Error UnexpectednumberOfArguments ]

            let errors = AppErrors.CreateErrorListeFromResults pairResults
            let good = [ for p in pairResults do match p with | Ok pair -> pair | Error _ -> () ]
            match errors with
            | [] -> Ok good
            | _ -> Error (Aggregate errors)
        | Error err -> Error err

    let private stringSplitOptions =
         System.StringSplitOptions.RemoveEmptyEntries

    let ParseExplicitAddTarget target =
        (RemoveSurroundingDoubleQuote target)
            .Split([|'.'|], stringSplitOptions)
        |> Array.toList

    let ParseExplicitAddInfo path handler =
        let commandNames = (ParseExplicitAddTarget path)[1..]

        { Path = 
            // Root command name is generally empty
            if commandNames.IsEmpty then
                [""]
            else
                commandNames
          Handler = handler }


    let ExpresionOption (evalLanguage: EvalBase) expression =
        if evalLanguage.IsNullLiteral expression then
            None
        else
            Some expression

    let ExplicitAddInfoListFrom (evalLanguage: EvalBase) (invocations: (string * SyntaxNode) list) : Result<ExplicitAddInfo list, AppErrors> =
            let addCommandInfoWithResults =
                let mutable pos = 0
                [ for invoke in invocations do
                    let pos = pos + 1
                    match invoke with
                    | (target, d ) ->
                        let expression = evalLanguage.ExpressionFrom d
                        match expression with
                        | Ok expr -> Ok (ParseExplicitAddInfo target (ExpresionOption evalLanguage expr))
                        | Error _->  Error (Generator.AppModelIssue $"Unexpected expression {pos}")
                    | _ -> Error Generator.UnexpectednumberOfArguments ]
            let errors = [
                for result in addCommandInfoWithResults do
                    match result with 
                    | Error err -> err
                    | _ -> ()]
            match errors with 
            | [] -> Ok [
                for result in addCommandInfoWithResults do
                    match result with
                    | Ok arch -> arch
                    | _ -> () ]
            | _ -> Error (Generator.Aggregate errors)

    let ExplicitAddInfoTreeFrom (addCommandInfoList: ExplicitAddInfo list) = 
        let mapBranch parents item childList=
            let data = 
                match item with
                | Some x -> x
                | None -> 
                    { Path = parents 
                      Handler = None }
            { Data = data; Children = childList }

        let getKey (item: ExplicitAddInfo) = item.Path

        try
          Ok (TreeFromKeyedList getKey mapBranch addCommandInfoList)
        with 
        | ex -> Error (Other ex.Message)