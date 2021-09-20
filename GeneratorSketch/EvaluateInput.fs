// KAD: Discuss namespaces and capitilization in naming
// KAD: Top level modules are confusing. It appears here 
//      that these rules are loose (Is EvaluateInput a top level module?)
namespace GeneratorSketch

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open Utils
open Model

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

    let (|Command|Arg|Option|) (part: string) =
        let part = part.Trim()

        if part.Length = 0 then
            Command part
        else
            match part.[0] with
            | '<' when part.[part.Length - 1] = '>' -> Arg(RemoveLeadingTrailing '<' '>' part.[1..part.Length - 2])
            | '<' -> invalidOp "Unmatched '<' found"
            | '-' when part.Length = 1 -> invalidOp "Options must have a name, extra '-' found."
            | '-' when part.[1] = '-' -> Option part.[2..]
            | '-' -> Option part.[1..]
            | _ -> Command part


    let parts archetype =
        let stringSplitOptions =
            System.StringSplitOptions.RemoveEmptyEntries
            ||| System.StringSplitOptions.TrimEntries
        (RemoveLeadingTrailingDoubleQuote archetype)
            .Split(' ', stringSplitOptions)
        |> Array.toList


    //// a
    //// a b c
    //// a b
    //// b d e
    //let buildModel (archetypes: ArchetypeInfo list) (extras: (string->string->string) list) =

    //    let rec getCommandDefs (level: int) (archetypeInfos: ArchetypeInfo list) =
    //        let commandAt level (archetypeInfo: ArchetypeInfo) = archetypeInfo.AncestorsAndThis.[level]
    //        let groups = 
    //            archetypeInfos
    //            |> List.groupBy (commandAt level)
    //        let commandDefs = [
    //            for group in groups do
    //                let 
    //                for ai in group do
    //                    match 
    //        ()

    //    ()
            
        