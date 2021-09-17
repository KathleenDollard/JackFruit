namespace GeneratorSketch

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open FSharp.CodeAnalysis.CSharp.RoslynPatterns
open Utils

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
        // KAD: For arg and option, it seems easiest to return the ArgDef and OptionDef currently created
        //      in parseArchteype. However, I do not have enough info here to return the full CommandDef
        //      and it seems anti-symmetric to either return just the Command name, or to return a partially
        //      created Command that is not yet valid.
        let part = part.Trim()

        if part.Length = 0 then
            Command part
        else
            match part.[0] with
            | '<' when part.[part.Length - 1] = '>' -> Arg(removeLeadingTrailing '<' '>' part.[1..part.Length - 2])
            | '<' -> invalidOp "Unmatched '<' found"
            | '-' when part.Length = 1 -> invalidOp "Options must have a name, extra '-' found."
            | '-' when part.[1] = '-' -> Option part.[2..]
            | '-' -> Option part.[1..]
            | _ -> Command part


    let syntaxTreeResult (source: Source) =
        let tree =
            match source with
            | SyntaxTree tree -> tree
            | Code code -> CSharpSyntaxTree.ParseText code

        let errors =
            [ for diag in tree.GetDiagnostics() do
                if diag.Severity = DiagnosticSeverity.Error then
                     diag ]

        if errors.IsEmpty then
            Ok tree
        else
            Error errors


    let parseArchetype (archetype: string) =
        let stringSplitOptions =
            System.StringSplitOptions.RemoveEmptyEntries
            ||| System.StringSplitOptions.TrimEntries

        let parts =
            (removeLeadingTrailingDoubleQuote archetype)
                .Split(' ', stringSplitOptions)
            |> Array.toList

        // This is the Commands in reverse order so the actual command is first
        // Given 'dotnet add package'
        //      The actual command is 'package'
        //      It's parent is 'add' and it's grandparent is 'dotnet'
        let commandParts =
            [ for part in parts do
                  match part with
                  | Command commandName -> commandName
                  | _ -> () ]
            |> List.rev

        // The root command does not have to be stated, and it may have options/args
        let commandName =
            if commandParts.IsEmpty then
                ""
            else
                commandParts.[0]

        let argDef =
            [ for part in parts do
                  match part with
                  | Arg argName ->
                      { ArgName = argName
                        TypeName = Some "" }
                  | _ -> () ]
            |> List.tryExactlyOne

        let optionDefs =
            [ for part in parts do
                  match part with
                  | Option optionName ->
                      { OptionName = optionName
                        TypeName = None }
                  | _ -> () ]

        { CommandName = commandName
          ParentCommandNames = commandParts.[1..]
          Arg = argDef
          Options = optionDefs }


    let archetypeInfoFrom (source: Source) =
        let archetypeFromStringExpression (arg: ExpressionSyntax) =
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
                      { Archetype = (archetypeFromStringExpression a.Expression)
                        HandlerExpression = d.Expression }
                  | _ -> () ]

        match syntaxTreeResult source with
        | Ok tree -> Ok(archetypesFromInvocations tree)
        | Error errors -> Error errors


    let methodFromHandler (model: SemanticModel) expression =
        let handler =
            model.GetSymbolInfo(expression: ExpressionSyntax)

        let symbol =
            match handler.Symbol with
            | null when handler.CandidateSymbols.IsDefaultOrEmpty -> invalidOp "Delegate not found"
            | null -> handler.CandidateSymbols.[0]
            | _ -> handler.Symbol

        match symbol with
        | :? IMethodSymbol as m -> Some m
        | _ -> None


    let copyUpdateArchetypeInfoFromSymbol archetypeInfo methodSymbol =
        let parameterFromMethodSymbol name (methodSymbol: IMethodSymbol) =
            let candidates =
                [ for p in methodSymbol.Parameters do
                      if p.Name = name then p ]

            match candidates with
            | [] -> None
            | _ -> Some candidates.[0]

        let mergeArgWithParameter initialArg methodSymbol =
            let newArg initialArg methodSymbol =
                match parameterFromMethodSymbol initialArg.ArgName methodSymbol with
                | Some parameter ->
                    Some
                        { initialArg with
                              TypeName = Some(parameter.Type.ToString()) }
                | None -> Some initialArg

            match initialArg with
            | Some arg -> newArg arg methodSymbol
            | None -> None

        let mergeOptionsWithParameters initialOptions methodSymbol =
            [ for option in initialOptions do
                  match parameterFromMethodSymbol option.OptionName methodSymbol with
                  | Some parameter ->
                      { option with
                            TypeName = Some(parameter.Type.ToString()) }
                  | None -> () ]
        // KAD: The logic for options is backward, peruse the method parameters and attempt match
        //      If the method does not take the parameter, we do not care. 
        { archetypeInfo with
              Archetype =
                  { archetypeInfo.Archetype with
                        Arg = mergeArgWithParameter archetypeInfo.Archetype.Arg methodSymbol
                        Options = mergeOptionsWithParameters archetypeInfo.Archetype.Options methodSymbol } }


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
