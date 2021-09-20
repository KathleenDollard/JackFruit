namespace GeneratorSketch

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open FSharp.CodeAnalysis.CSharp.RoslynPatterns

module Generator =

    let initialize () = ()

    type ArgDef =
        { ArgName: string
          TypeName: string option }

    type OptionDef =
        { OptionName: string
          TypeName: string option }

    type CommandDef =
        { CommandName: string
          ParentCommandNames: string list
          Arg: ArgDef option
          Options: OptionDef list }

    type ArchetypeInfo =
        { Archetype: CommandDef
          HandlerExpression: ExpressionSyntax }

    type HandlerIdentifier =
        { HandlerName: string
          Parents: string list }

    type Source =
        | SyntaxTree of SyntaxTree
        | Code of string


    /// Categorize a command line parameter as command, argument or option
    //    "<a>" --> Arg "a"
    //    "" --> TODO - give an error
    //    "-" --> TODO - give an error
    //    "-foo" --> Option foo
    //    "--foo" --> Option foo
    //    "---foo" --> TODO - give an error

    let (|Command|Arg|Option|) (part: string) =
        let trimmed = part.Trim()
        match trimmed with
        | "" -> 
            Command ""
        | _ ->
            match trimmed.[0] with
            | '<' when trimmed.[trimmed.Length-2] = '>' ->
                // Extract the useful part of the argument as the result of the active pattern
                let argText = trimmed.[1..trimmed.Length-2]
                Arg argText

            | '-' when trimmed.Length > 1 -> 
                let optionText =
                    match (trimmed.[0], trimmed.[1]) with
                    | ('-','-') -> trimmed.[2..]
                    | ('-', _ ) -> trimmed.[1..]
                    | _ -> trimmed
                Option optionText

            | _ ->
                Command trimmed

    let syntaxTreeResult  (source: Source) =
        let tree =
            match source with 
            | SyntaxTree tree -> tree
            | Code code -> CSharpSyntaxTree.ParseText code
        let errors =
            [ for diag in tree.GetDiagnostics() do
                if diag.Severity = DiagnosticSeverity.Error then diag]
        if errors.IsEmpty then Ok tree
        else Error errors

    let parseArchetype (archetype: string) =

        let clean = 
            // KAD: How to simplify code in this method
            // KAD: Why do I need to specify the type if the first line is Trim?
            let temp = archetype.Trim()

            if temp.Length > 1 && temp.[0] = '"' && temp.[temp.Length-2] = '"' then
                temp.[1..temp.Length - 2]
            else
                temp

        let stringSplitOptions = System.StringSplitOptions.RemoveEmptyEntries ||| System.StringSplitOptions.TrimEntries

        let parts =
            clean.Split(' ',stringSplitOptions)  |> Array.toList

        // parts "dotnet add pacakge"
        //   Command3 --> package
        //   Command2  --> add
        //   Command1 --> dotnet
        let commandPartsReversed =
            [ for part in parts do
                match part with
                | Command command -> command
                | _ -> () ]
            |> List.rev

        let finalCommandName = if commandPartsReversed.IsEmpty then "" else commandPartsReversed.[0]

        let argDef =
            [ for part in parts do
                match part with
                | Arg arg -> 
                    { ArgName = arg; TypeName = Some "" }
                | _ -> () ]
            |> List.tryExactlyOne
            // TODO: multiple arguments

        let optionDefs =
            [ for part in parts do
                match part with
                | Option option ->
                    { OptionName = option
                      TypeName = None }
                | _ -> () ]

        { CommandName = finalCommandName
          ParentCommandNames = commandPartsReversed.[1..]
          Arg = argDef
          Options = optionDefs }

    let archetypeInfoFrom (source: Source) =
        let archetypeFromArgument (arg: ExpressionSyntax) =
            let argString =
                match arg with
                | StringLiteralExpression -> arg.ToFullString()
                | _ -> invalidOp "Only string literals currently supported"

            parseArchetype argString

        let archetypesFromInvocations tree = 
            let invocations = Patterns.mapInferredInvocations tree
            
            [ for invoke in invocations do
                match invoke.args with
                | [ a; d ] ->
                    { Archetype = archetypeFromArgument a.Expression
                      HandlerExpression = d.Expression }
                | _ -> () ]

        let result = syntaxTreeResult source
        match result with
        | Ok tree -> Ok (archetypesFromInvocations tree)
        | Error errors -> Error errors

    // Probably do not ned this
    let rec splitHandlerExpression expression =
        match expression with
        | SimpleMemberAccessExpression (leftExpression, identifier) ->
            identifier.ToString()
            :: splitHandlerExpression leftExpression
        | IdentifierNameSyntax (_, identifier) -> [ identifier.ToString() ]
        | _ -> invalidOp "Unexpected handler expression, for example lambdas aren't supported"

    let methodFromHandler (model: SemanticModel) (expression: ExpressionSyntax) =

        let handler =
            model.GetSymbolInfo(expression)

        let symbol =
            match handler.Symbol with
            | null when handler.CandidateSymbols.IsDefaultOrEmpty -> invalidOp "Delegate not found"
            | null -> handler.CandidateSymbols.[0]
            | _ -> handler.Symbol

        let methodSymbol =
            match symbol with
            | :? IMethodSymbol as m -> m
            | _ -> invalidOp "Symbol not method type"

        methodSymbol

    let copyUpdateArchetypeInfoFromSymbol archetypeInfo methodSymbol =
        let parameterFromMethodSymbol name (methodSymbol: IMethodSymbol) =
            let candidates = [for p in methodSymbol.Parameters do
                                if p.Name = name then p ]
            match candidates with 
            | [] -> None
            | _ -> Some candidates.[0]

        let mergeArgWithParameter initialArg methodSymbol =
            let newArg initialArg methodSymbol =
                match parameterFromMethodSymbol initialArg.ArgName methodSymbol with
                | Some parameter -> Some { initialArg with TypeName = Some (parameter.Type.ToString())}
                | None -> Some initialArg
            match initialArg with 
            | Some arg -> newArg arg methodSymbol
            | None -> None

        let mergeOptionsWithParameters initialOptions methodSymbol =
            [ for option in initialOptions do 
                match parameterFromMethodSymbol option.OptionName methodSymbol with
                | Some parameter -> { option with TypeName = Some (parameter.Type.ToString())}
                | None -> ()]

        { archetypeInfo with 
            Archetype = 
                {archetypeInfo.Archetype with 
                    Arg = mergeArgWithParameter archetypeInfo.Archetype.Arg methodSymbol;
                    Options = mergeOptionsWithParameters archetypeInfo.Archetype.Options methodSymbol }}


    let generate model commandInfo =
        let handlerDef =
            methodFromHandler model commandInfo.HandlerExpression
        //let method = findMethod semanticModel handlerName
        ()

    type Generator =
        interface ISourceGenerator with
            member ISourceGenerator.Initialize(context: GeneratorInitializationContext) : unit =
                initialize ()
                ()

            member ISourceGenerator.Execute(context: GeneratorExecutionContext) : unit =
            //    let syntaxTrees = Seq.toList context.Compilation.SyntaxTrees

            //    let commandsFromHandler = 
            //        let model =
            //            context.Compilation.GetSemanticModel tree

            //        for command in commands do
            //            generate model command
                ()                    

            //    let archetypeInfos = [for tree in syntaxTrees do
            //                            archetypeInfoFrom (Source.SyntaxTree tree)]
            //    let archetypes = [for archInfos in archetypeInfos do 
            //                        match archInfos with 
            //                        | Ok arch -> arch 
            //                        | Error _ -> () ]

            //    let syntaxErrors = [for archInfos in archetypeInfos do 
            //                            match archInfos with 
            //                            | Ok _ -> () 
            //                            | Error errors -> errors ]
                



