module Prototype

open Xunit
open Generator.Language
open Generator.GeneralUtils
open System
open Common
open DslKeywords
open DslCodeBuilder
open type Generator.Language.LanguageHelpers

type System.String with 
    member this.AsFieldName() =
        ToCamel(this)
    member this.AsParamName() =
        ToCamel(this)

type ``When creating simple code``() =

    [<Fact>]
    member _.``these features are available``() =
        let nspaceName = "NamespaceA"
        let className = "ClassA"
        let propertyName = "Prop1"
        let propertyType = "string"
        let paramName = propertyName.AsParamName()
        let method = "MethodA"
        let code = 
            Namespace(nspaceName) {
                Using "System"
                Using "System.Linq"

                Class(className) {
                    Public

                    Members [
                        Field("A", "string") { Private }

                        Constructor() 
                            { Public 
                              //Parameter paramName propertyType
                              Assign propertyName paramName
                            }


                        Property(propertyName, "int") { Public }

                    
                    ]

                }
            }
        ()