module DslPlayground

open DslCodeBuilder
open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open Generator.RoslynUtils
open Microsoft.CodeAnalysis
open Generator.Language

type ``When using DSL``() =

    [<Fact>]
    member _.``Can add using to namespace``() =
        let Namespace = new NamespaceBuilder()

        let nSpace = NamespaceModel.Default()

        let code1 = Namespace.addUsing { Namespace = "Fred"; Alias = None } 

        let code = 
            Namespace {
                Name "George"
                Using { Namespace = "Fred"; Alias = None } 
                }

        Assert.NotNull code