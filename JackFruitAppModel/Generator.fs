namespace Jackfruit

open Generator.RoslynUtils
open Microsoft.CodeAnalysis
open Jackfruit.ArchetypeMapping
open Jackfruit
open Generator.NewMapping
open Generator.SourceGenerator
open Common
open Jackfruit.Models


[<Generator>]
type Generator() =
    inherit CliSourceGenerator<TreeNodeType<ArchetypeInfo>>()

    override _.GetAppModel synataxTrees semanticModel =
        Jackfruit.AppModel()

