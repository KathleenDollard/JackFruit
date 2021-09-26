// KAD: Discuss namespaces and capitilization in naming
// KAD: Top level modules are confusing. It appears here 
//      that these rules are loose (Is EvaluateInput a top level module?)
namespace GeneratorSketch

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open FSharp.CodeAnalysis.CSharp.RoslynPatterns
open Utils
open Model
open GeneratorSketch.Patterns

// New algorithm:
//  Crete the defs from the archetype, except do the commands in order, not reversed
//  Group by the head and pass the tail, when you get to the end
//      Evaluate the handler
//      Look for any path based additions (Description)
//      Look for any global additions (common abbreviations
//      Put it all together in the nested Commands


// Naming: Things that might be called Option, Arg or Command
//    Record to use for generation - this is what we are building (Currently called ...Def, let's leave this)
//    Active pattern to evaluate archetype and return sub parts (Currently called just ... let's leave this)
//    Possibly an interim thing from the active pattern (it doesn't make sense not to cache the archetype breakdown)
//      * This could be the Def, but it is interim and shouldn't be allowed to escape

module EvaluateInput =


    let private  (|Command|Arg|Option|) (part: string) =
        let part = part.Trim()

        if part.Length = 0 then
            Command part
        else
            match part.[0] with
            | '<' -> Arg part
            | '-' -> Option part
            | _ -> Command part

    let private parts archetype =
        let stringSplitOptions =
            System.StringSplitOptions.RemoveEmptyEntries
            ||| System.StringSplitOptions.TrimEntries

        (RemoveLeadingTrailingDoubleQuote archetype)
            .Split(' ', stringSplitOptions)
        |> Array.toList

    let private GetArchetypeInfoFrom archetype handler = 
        let parts = parts archetype

        let commandParts =
            [ for part in parts do
                  match part with
                  | Command commandName -> commandName
                  | _ -> () ]

        { AncestorsAndThis = commandParts
          Raw = parts
          HandlerExpression = handler }

    type Source =
        | SyntaxTree of SyntaxTree
        | Code of string

    
    let MethodFromHandler (model: SemanticModel) expression =
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

    let private syntaxTreeResult (source: Source) =
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
            
    let ArchetypeInfoListFrom (source: Source) =
        let stringFromExpression (arg: ExpressionSyntax) =
            let argString =
                match arg with
                | StringLiteralExpression -> arg.ToFullString()
                | _ -> invalidOp "Only string literals currently supported"

            argString

        let archetypeInfoListFromInvocations tree =
            let invocations = Patterns.MapInferredInvocations tree

            [ for invoke in invocations do
                  match invoke.args with
                  | [ a; d ] -> GetArchetypeInfoFrom (stringFromExpression a.Expression) (Some d.Expression)

                  | _ -> () ]

        match syntaxTreeResult source with
        | Ok tree -> Ok(archetypeInfoListFromInvocations tree)
        | Error errors -> Error errors

    type Path = string list

    type InfoProvider = 
        {   CommandName: (Path -> string option) option
            CommandDescription: (Path -> string option) option
            CommandAliases: (Path -> string list option) option
            ParentCommandNames: (Path -> string list) option
            ArgName: (Path -> string option) option
            ArgTypeName: (Path -> string option) option
            ArgDescription: (Path -> string option) option
            OptionName: (Path -> string option) option
            OptionTypeName: (Path -> string option) option
            OptionDescription: (Path -> string option) option
            OptionAliases: (Path -> string list option) option
            Children: (Path -> string list) option
            IsArg: (Path -> bool) option }

        static member Empty  = 
           {   CommandName = None
               CommandDescription = None
               CommandAliases = None
               ParentCommandNames = None
               ArgName = None
               ArgTypeName = None
               ArgDescription = None
               OptionName = None
               OptionTypeName = None
               OptionDescription = None
               OptionAliases = None 
               Children = None
               IsArg = None }

    let rec AggregateProvider fs path =
        match fs with
        | [] -> 
            None
        | head::[] -> 
            head path
        | head::tail ->
            match head path with 
            | Some v -> Some v
            | None -> AggregateProvider tail path



    //let private AggregateProvider infoProviders = 
    //    { 
    //        CommandName = [ for provider in infoProviders do
    //                          match provider.CommandName with 
    //                          | Some f -> Some f
    //                          | None -> () ]
                
    //    }

    //let MapCommandDef infoProviders commandId path raw =
            


    //let MapCommand infoProviders model groupId archetypeInfoOption childList= 
    //    // CLI Layout always comes from archetype (with inferrence for missing branches)
    //    // TypeName always comes from the handler parameters
    //    // Everything else can come from any InfoProvider
    //    match archetypeInfoOption with 
    //    | Some arch -> 
    //        let commandId = arch.AncestorsAndThis |> List.last
    //        let raw = arch.Raw
    //        match arch.HandlerExpression with 
    //        | Some handler -> 
    //            let handlerProvider = GetHandlerProvider model handler
    //            MapCommandDef handlerProvider::infoProviders commandId groupId raw
    //        | None -> 
    //            MapCommandNoHandler commandId groupId raw
    //    | None -> 
    //        let commandId = groupId |> List.last
    //        let raw = groupId |> String.concat " "
    //        MapCommandNoHandler commandId groupId raw




    //    // Providers that only specify a few things can use the Default and with

    
    ////let CommandDefFrom (source: Source) =
    ////    let getKey item = item.AncestorsAndThis
            

    //    let commandDefFrom (input: ArchetypeInfo list) = 
    //        TreeFromList getKey mapBranch input
        
    //    let archListResult = ArchetypeInfoListFrom source

    //    match archListResult with 
    //    | Ok archList -> 
    //        try
    //            commandDefFrom archList
    //        with 
    //        | _ -> reraise()
    //    | Error diagnostics -> invalidOp "TODO: format diagnostics"

