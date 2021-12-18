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
        let Namespace = NamespaceBuilder("George")
        let code = 
            Namespace {
                Using { Namespace = "Fred"; Alias = None } 
                }

        Assert.NotNull code

    
    [<Fact>]
    member _.``Can add using to namespace2``() =
        let code = 
            // If we use this style, rename "NamespaceBuilder" to "Namespace"
            NamespaceBuilder("George") 
                {
                    Using { Namespace = "Fred"; Alias = None } 
                    Using { Namespace = "Fred"; Alias = None } 
                    Using { Namespace = "Fred"; Alias = None } 
                }

        Assert.NotNull code