module Generator.SubCommandMapping

open Generator.CodeEval

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