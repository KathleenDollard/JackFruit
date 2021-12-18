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
                Usings [ { Namespace = "Fred"; Alias = None } ]
                }

        Assert.NotNull code

    
    [<Fact>]
    member _.``Can add multiple usings to namespace``() =
        let code = 
            // If we use this style, rename "NamespaceBuilder" to "Namespace"
            NamespaceBuilder("George") 
                {
                    Using "Jill"
                    // KAD-Don: Why doesn't this work? (I have not gotten any overloads to work)
                    //Using "Jack" Alias "Hill"
                    Usings 
                        [ { Namespace = "Fred"; Alias = None } 
                          { Namespace = "Sally"; Alias = None } 
                          { Namespace = "Sue"; Alias = None } ]
                }

        Assert.NotNull code

    [<Fact>]
    member _.``Can add class to namespace``() =
        let Namespace = NamespaceBuilder("George")
        let code = 
            Namespace {
                Classes [ ClassModel.Create("Fred", Public, [])]
                }

        Assert.NotNull code

    
    [<Fact>]
    member _.``Can create class``() =
        let Class = ClassBuilder("GeorgeClass")
        let code = 
            Class {
                Public
                // KAD-Don: Why doesn't this work? (I have not gotten most overloads to work)
                // Public Static
                }

        Assert.NotNull code