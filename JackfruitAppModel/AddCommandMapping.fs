namespace Jackfruit

open Generator.CodeEval
open Generator.GeneralUtils
open Microsoft.CodeAnalysis
open Common
open Generator

type AddCommandInfo =
    { Path: string list 
      Handler: SyntaxNode option }


module AddCommandMapping =
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

    //let GetPathAndHandler methodName (evalLanguage: EvalBase) semanticModel =
    //    let commandPairsResult = evalLanguage.InvocationsFromModel methodName semanticModel
    //    match commandPairsResult with
    //    | Ok commandPairs -> 
    //        [ for commandPair in commandPairs do
    //            match commandPair with
    //            | (name, [handler]) -> name, handler
    //            | _ -> () ]
    //    | _ -> []

    let private stringSplitOptions =
         System.StringSplitOptions.RemoveEmptyEntries

    let ParseAddCommandTarget target =
        (RemoveSurroundingDoubleQuote target)
            .Split([|'.'|], stringSplitOptions)
        |> Array.toList

    let ParseAddCommandInfo path handler =
        let commandNames = (ParseAddCommandTarget path)[1..]

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

    let AddCommandInfoListFrom (evalLanguage: EvalBase) (invocations: (string * SyntaxNode list) list) : Result<AddCommandInfo list, AppErrors> =
            let addCommandInfoWithResults =
                let mutable pos = 0
                [ for invoke in invocations do
                    let pos = pos + 1
                    match invoke with
                    | (_, [ a; d ]) ->
                        let target = evalLanguage.StringFrom a
                        let expression = evalLanguage.ExpressionFrom d
                        match (target, expression) with
                        | Ok arch, Ok expr -> Ok (ParseAddCommandInfo arch (ExpresionOption evalLanguage expr))
                        | Error _, _ ->  Error (Generator.AppModelIssue $"Unexpected expression for frchetype at {pos}")
                        | Ok arch, Error _ ->  Error (Generator.AppModelIssue $"Unexpected expression for handler {arch}")
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

    let AddCommandInfoTreeFrom (addCommandInfoList: AddCommandInfo list) = 
        let mapBranch parents item childList=
            let data = 
                match item with
                | Some x -> x
                | None -> 
                    { Path = parents 
                      Handler = None }
            { Data = data; Children = childList }

        let getKey (item: AddCommandInfo) = item.Path

        try
          Ok (TreeFromKeyedList getKey mapBranch addCommandInfoList)
        with 
        | ex -> Error (Other ex.Message)