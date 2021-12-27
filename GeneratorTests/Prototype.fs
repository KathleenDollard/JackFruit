module Prototype

open Xunit
open Generator.Language
open Common
open DslKeywords
open DslCodeBuilder
open type Generator.Language.LanguageHelpers


type ``When creating simple code``() =

    [<Fact>]
    member _.``these features are available``() =
        let nspaceName = "NamespaceA"
        let className = "ClassA"
        let method = "MethodA"
        let code = 
            Namespace(nspaceName) {
                Using "System"
                Using "System.Linq"

                Class(className) {
                    Public

                }
            }
        ()