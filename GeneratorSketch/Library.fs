namespace GeneratorSketch

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open FSharp.CodeAnalysis.CSharp.RoslynPatterns

module Generator =

    let initialize() = ()

    type ArgDef = {argName: string; typeName: string}
    type OptionDef = {optionName: string; typeName: string}
    type CommandDef = {commandName: string; parentCommandNames: string list; args: ArgDef option; options: OptionDef list}
    type CommandInfo = {archetype: CommandDef; handlerExpression: ExpressionSyntax}
    type HandlerIdentifier = { handlerName: string; parents: string list}

    let (|Command|Arg|Option|) (part:string) =
        match part.ToCharArray() with 
        | [||] -> Command
        | _ -> match part.[0] with 
               | '<' -> Arg
               | '-' -> Option
               | _ -> Command

    let parseArchetype (archetype: string) = 
        // KAD: How to simplify the following 3 lines
        let temp = archetype.Trim()
        let strippedArchetype = temp.[1..(temp.Length-2)]                       
        let parts = strippedArchetype.Split ' '
                    |> Array.toList
        let commandParts = [ for part in parts do
                                match part with 
                                | Command -> part
                                | _ -> () ]
                            |> List.rev
        let argDef = [ for part in parts do
                            match part with
                            | Arg -> {argName = part; typeName = ""}
                            | _ -> ()]
                    |> List.tryExactlyOne
        let optionDefs = [ for part in parts do
                                match part with 
                                | Option -> {optionName=part;typeName=""}
                                | _ -> () ]

        {commandName = commandParts.[0]; parentCommandNames = commandParts.[1..]; args=argDef; options=optionDefs}
    
    let commandInfo (syntaxTree: SyntaxTree) =
        let archetypeFromArgument (arg: ExpressionSyntax) = 
            let argString = match arg with
                            | StringLiteralExpression -> arg.ToFullString()
                            | _ -> invalidOp "Only string literals currently supported"
            parseArchetype argString
        let invocations = Patterns.memberInvocations syntaxTree
        [for invoke in invocations do
            match invoke.args with
            | [a; d] -> {archetype = (archetypeFromArgument a.Expression); handlerExpression = d.Expression}
            | _ -> ()
        ]


    // Probably do not ned this
    let rec splitHandlerExpression expression = 
        match expression with 
        | SimpleMemberAccessExpression (leftExpression, identifier) -> identifier.ToString()::splitHandlerExpression leftExpression
        | IdentifierNameSyntax  (_, identifier)-> [identifier.ToString()]
        | _ -> invalidOp "Unexpected handler expression, for example lambdas aren't supported"

    let evaluateHandler (model:SemanticModel) expression = 
        let identifier = splitHandlerExpression expression
        let handler = model.GetSymbolInfo(expression:ExpressionSyntax)
        let symbol = match handler.Symbol with 
                     | null when handler.CandidateSymbols.IsDefaultOrEmpty
                            -> invalidOp "Delegate not found"
                     | null -> handler.CandidateSymbols.[0]
                     | _ -> handler.Symbol
        let methodSymbol = match symbol with
                           | :? IMethodSymbol as m -> m
                           | _ -> invalidOp "Symbol not method type"
        methodSymbol
   
        
 
    let generate model commandInfo = 
        let handlerDef = evaluateHandler model commandInfo.handlerExpression
        //let method = findMethod semanticModel handlerName
        ()

    type Generator =
        interface ISourceGenerator with
            member ISourceGenerator.Initialize (context: GeneratorInitializationContext) : unit = 
                initialize()
                ()
            member ISourceGenerator.Execute (context: GeneratorExecutionContext) :  unit  = 
                let syntaxTrees = Seq.toList context.Compilation.SyntaxTrees
                for tree in syntaxTrees do
                    let commands = commandInfo tree
                    let model = context.Compilation.GetSemanticModel tree
                    for command in commands do 
                        generate model command
                        ()


                




