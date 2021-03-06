namespace Jackfruit

// KAD-Chet: I am not getting Windows to correctly display the AssemblyVersion

open Microsoft.CodeAnalysis
open Generator.SourceGenerator
open Common
open Jackfruit.Models
open Generator.ExplicitAdd
open Generator


[<Generator(LanguageNames.VisualBasic)>]
type ExplicitAddGenerator() =
    inherit CliSourceGenerator<TreeNodeType<ExplicitAddInfo>>()

    override _.GetAppModel() =
        AppModel(EvalVisualBasic())


