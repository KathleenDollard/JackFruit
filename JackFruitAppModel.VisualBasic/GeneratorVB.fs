namespace Jackfruit

// KAD-Chet: I am not getting Windows to correctly display the AssemblyVersion

// KAD-Chetx: I changed from .NET 6.0 to .NET Standard and got new errors in string behavior. What's that about?
//  Example: TrimEntries was missing and string split needed the separator as an explicit array.
//  Nevermind: These are changes in .NET 5 and .NET 6


open Microsoft.CodeAnalysis
open Generator.SourceGenerator
open Common
open Jackfruit.Models
open Generator


[<Generator(LanguageNames.CSharp, LanguageNames.VisualBasic)>]
type Generator() =
    inherit CliSourceGenerator<TreeNodeType<ArchetypeInfo>>()

    override _.GetAppModel synataxTrees semanticModel =
        Jackfruit.AppModel(EvalVisualBasic())


