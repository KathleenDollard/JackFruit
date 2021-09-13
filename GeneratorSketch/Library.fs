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

    let (|Command|Arg|Option|) (part: string) =
        match part.Trim().ToCharArray() with
        | [||] -> Command
        | _ ->
            match part.[0] with
            | '<' -> Arg
            | '-' -> Option
            | _ -> Command

    let syntaxTreeResult  (source: Source) =
        let tree = match source with 
                   | SyntaxTree tree -> tree
                   | Code code -> CSharpSyntaxTree.ParseText code
        let errors = [ for diag in tree.GetDiagnostics() do
                        if diag.Severity = DiagnosticSeverity.Error then diag]
        if errors.IsEmpty then Ok tree
        else Error errors

    let parseArchetype (archetype: string) =
        let cleanArchetype (arch: string) =
            // KAD: How to simplify code in this method
            // KAD: Why do I need to specify the type if the first line is Trim?
            let temp = arch.Trim()

            let temp1 =
                if temp.[0] = '"' then
                    temp.[1..]
                else
                    temp

            let pos = temp1.Length - 1

            if temp1.[pos] = '"' then
                temp1.[0..pos - 1]
            else
                temp1
        let cleanArgName (name: string) =
            match name.[0] with
            | '<' -> name.[1..name.Length-2]  // assume if open angle, also close angle
            | _ -> name
        let cleanOptionName(name: string) =
            match (name.[0], name.[1]) with
            | ('-','-') -> name.[2..]
            | ('-', _ ) -> name.[1..]
            | _ -> name

        let stringSplitOptions = System.StringSplitOptions.RemoveEmptyEntries ||| System.StringSplitOptions.TrimEntries
        let parts =
            (cleanArchetype archetype).Split(' ',stringSplitOptions)  |> Array.toList
        let commandParts =
            [ for part in parts do
                  match part with
                  | Command -> part
                  | _ -> () ]
            |> List.rev
        let commandName = if commandParts.IsEmpty then "" else commandParts.[0]
        let argDef =
            [ for part in parts do
                  match part with
                  | Arg -> { ArgName = cleanArgName part; TypeName = Some "" }
                  | _ -> () ]
            |> List.tryExactlyOne
        let optionDefs =
            [ for part in parts do
                  match part with
                  | Option ->
                      { OptionName = cleanOptionName part
                        TypeName = None }
                  | _ -> () ]

        { CommandName = commandName
          ParentCommandNames = commandParts.[1..]
          Arg = argDef
          Options = optionDefs }

    let archetypeInfoFrom (source: Source) =
        let archetypeFromArgument (arg: ExpressionSyntax) =
            let argString =
                match arg with
                | StringLiteralExpression -> arg.ToFullString()
                | _ -> invalidOp "Only string literals currently supported"

            parseArchetype argString

        let archetypesFromInvocatios tree = 
            let invocations = Patterns.mapInferredInvocations tree
            
            [ for invoke in invocations do
                    match invoke.args with
                    | [ a; d ] ->
                        { Archetype = (archetypeFromArgument a.Expression)
                          HandlerExpression = d.Expression }
                    | _ -> () ]


        let result = syntaxTreeResult source
        match result with
        | Ok tree -> Ok (archetypesFromInvocatios tree)
        | Error errors -> Error errors

    // Probably do not ned this
    let rec splitHandlerExpression expression =
        match expression with
        | SimpleMemberAccessExpression (leftExpression, identifier) ->
            identifier.ToString()
            :: splitHandlerExpression leftExpression
        | IdentifierNameSyntax (_, identifier) -> [ identifier.ToString() ]
        | _ -> invalidOp "Unexpected handler expression, for example lambdas aren't supported"

    let methodFromHandler (model: SemanticModel) expression =
        let identifier = splitHandlerExpression expression

        let handler =
            model.GetSymbolInfo(expression: ExpressionSyntax)

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
                



