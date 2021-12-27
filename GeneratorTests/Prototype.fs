module Prototype

open Xunit
open Generator.Language
open Generator.GeneralUtils
open Common
open DslKeywords
open DslCodeBuilder
open type Generator.Language.LanguageHelpers
open System

type System.String with 
    member this.AsFieldName() =
        ToCamel(this)

type ``When creating simple code``() =

    [<Fact>]
    member _.``these features are available``() =
        let nspaceName = "NamespaceA"
        let className = "ClassA"
        let propertyName = "Prop1"
        let method = "MethodA"
        let code = 
            Namespace(nspaceName) {
                Using "System"
                Using "System.Linq"

                Class(className) {
                    Public

                    Members [
                        Field("A", "string") { Private }

                        //Constructor() 
                        //    { Public 
                        //      Parameters 
                        //    }


                        Property(propertyName, "int") { Public }

                    
                    ]

                }
            }
        ()