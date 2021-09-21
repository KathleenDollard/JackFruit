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
          Path = commandParts |> String.concat " "
          HandlerExpression = handler }

    type Source =
        | SyntaxTree of SyntaxTree
        | Code of string

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
                  | [ a; d ] -> GetArchetypeInfoFrom (stringFromExpression a.Expression) d.Expression

                  | _ -> () ]

        match syntaxTreeResult source with
        | Ok tree -> Ok(archetypeInfoListFromInvocations tree)
        | Error errors -> Error errors

    type Path = string

    type Person =
      { Name: string
        Age: int
        Address: string }
    
        static member Default =
            { Name = "Phillip"
              Age = 12
              Address = "123 happy fun street" }

    type InfoProvider = 
        {   CommandName: (Path -> string) option
            CommandDescription: (Path -> string) option
            CommandAliases: (Path -> string list) option
            ParentCommandNames: (Path -> string list) option
            ArgName: (Path -> string) option
            ArgDescription: (Path -> string) option
            OptionName: (Path -> string) option
            OptionDescription: (Path -> string) option
            OptionAliases: (Path -> string list) option }

        // Providers that only specify a few things can use the Default and with
        static member Default  = 
           {   CommandName = None
               CommandDescription = None
               CommandAliases = None
               ParentCommandNames = None
               ArgName = None
               ArgDescription = None
               OptionName = None
               OptionDescription = None
               OptionAliases = None }
    
    let defaultPerson = Person.Default

    let CommandDefFrom (source: Source) =
        let getKey item = item.AncestorsAndThis
            
        let mapBranch gId itemOption childList= 
            match itemOption with 
            | None -> 
                let commandId = gId |> List.last
                let parents = gId.[0..gId.Length - 2]
                { CommandId = commandId
                  Name = commandId
                  Description = None
                  ParentCommandNames = parents
                  Arg = None
                  Options = []
                  SubCommands = [] }

            | Some item ->
                let commandId = item.AncestorsAndThis |> List.last
                let parents = item.AncestorsAndThis.[0..item.AncestorsAndThis.Length - 2]
                { CommandId = commandId
                  Name = commandId
                  Description = None
                  ParentCommandNames = parents
                  Arg = None
                  Options = []
                  SubCommands = [] }

        let commandDefFrom (input: ArchetypeInfo list) = 
            TreeFromList getKey mapBranch input
        
        let archListResult = ArchetypeInfoListFrom source

        match archListResult with 
        | Ok archList -> 
            try
                commandDefFrom archList
            with 
            | _ -> reraise()
        | Error diagnostics -> invalidOp "TODO: format diagnostics"

         




            
        