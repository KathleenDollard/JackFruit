namespace Generator.ExplicitAdd

open Generator.CodeEval
open Generator.GeneralUtils
open Microsoft.CodeAnalysis
open Common
open Generator
open System
open System.Linq
open Models


type ExplicitAddInfo =
    { Path: string list 
      Handler: RoslynSymbol }


module ExplicitAddMapping =
    let GetNamePatterns defaultPatterns (evalLanguage: EvalBase) syntaxTree =
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
                    //          can have a function that returns a value or unit. It feels like a railway pattern, but not quite
                    match patternResult with
                    | Ok pattern -> pattern.Replace("\"", "")
                    | Error _ -> () 
                | _ -> () ]

        let patternsFromInvocations methodName =
            let invocations = evalLanguage.InvocationsFromSyntaxTree methodName syntaxTree
            let result = evalLanguage.InvocationsFromSyntaxTree methodName syntaxTree
            match result with
            | Ok list -> patternsFromSyntaxNodes list
            | Error _ -> []

        let addPatterns = patternsFromInvocations ["AddCommandNamePattern"]
        let removePatterns = patternsFromInvocations ["RemoveCommandNamePattern"]
        let patterns = 
            defaultPatterns @ addPatterns
            |> List.except removePatterns
        patterns

    //let GetPathAndHandler methodName (evalLanguage: EvalBase) semanticModel =
    //    let commandPairsResult = evalLanguage.InvocationsFromModel methodName semanticModel
    //    match commandPairsResult with
    //    | Ok commandPairs -> 
    //        let pairResults = 
    //            [ for commandPair in commandPairs do
    //              match commandPair with
    //              | (name, [handler]) -> Ok (name, handler)
    //              | (name, _) -> Error UnexpectednumberOfArguments ]

    //        let errors = AppErrors.CreateErrorListeFromResults pairResults
    //        let good = [ for p in pairResults do match p with | Ok pair -> pair | Error _ -> () ]
    //        match errors with
    //        | [] -> Ok good
    //        | _ -> Error (Aggregate errors)
    //    | Error err -> Error err

    let private stringSplitOptions =
         System.StringSplitOptions.RemoveEmptyEntries

    let ParseExplicitAddTarget target =
        (RemoveSurroundingDoubleQuote target)
            .Split([|'.'|], stringSplitOptions)
        |> Array.toList

    let ParseExplicitAddInfo path handler currentName =
        let commandNames = (ParseExplicitAddTarget path)[1..] @ [currentName]

        { Path = commandNames
          Handler = handler }


    let ExpresionOption (evalLanguage: EvalBase) expression =
        if evalLanguage.IsNullLiteral expression then
            None
        else
            Some expression

    let ExplicitAddInfoListFrom (evalLanguage: EvalBase) (compilation: Compilation) (invocations: (string * SyntaxNode list) list) : Result<ExplicitAddInfo list, AppErrors> =
            Console.WriteLine($"Invation Count = {invocations.Length}")
            let addCommandInfoWithResults =
                let mutable pos = 0
                [ for invoke in invocations do
                    let pos = pos + 1
                    match invoke with
                    | (target, d ) ->
                        let expressionResult = evalLanguage.ExpressionFrom d[0]
                        match expressionResult with
                        | Ok expr -> 
                            let exprOption = (ExpresionOption evalLanguage expr)
                            match exprOption with
                            | Some expression ->
                                let semanticModel = compilation.GetSemanticModel(expression.SyntaxTree)
                                let method = evalLanguage.MethodSymbolFromMethodCall semanticModel expression
                                match method with
                                | Some m -> 
                                    let roslynSymbol = MethodSymbol m
                                    Ok (ParseExplicitAddInfo target roslynSymbol m.Name)
                                | None -> Error (Generator.AppModelIssue $"Parameter must be a {pos}")
                            | None -> Error (Generator.AppModelIssue $"Method is required {pos}")
                        | Error _->  Error (Generator.AppModelIssue $"Unexpected expression {pos}") ]
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
                      Handler = NoSymbolFound }
            { Data = data; Children = childList }

        let getKey (item: ExplicitAddInfo) = item.Path

        try
          Ok (TreeFromKeyedList getKey mapBranch addCommandInfoList)
        with 
        | ex -> Error (Other ex.Message)