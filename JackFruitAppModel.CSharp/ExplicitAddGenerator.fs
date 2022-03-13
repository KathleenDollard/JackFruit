namespace Jackfruit

// KAD-Chet: I am not getting Windows to correctly display the AssemblyVersion

open Microsoft.CodeAnalysis
open Generator.SourceGenerator
open Common
open Generator.ExplicitAdd
open Generator
open Generator.NewMapping
open Generator.Transforms
open System.Linq
open LanguageRoslynOut


[<Generator(LanguageNames.CSharp)>]
type ExplicitAddGenerator() =
    inherit CliSourceGenerator<TreeNodeType<ExplicitAddInfo>>()

    override _.GetAppModel() =
        AppModel(EvalCSharp())


