module PrototypeNew

open Xunit
open Generator.LanguageModel
open Generator.GeneralUtils
open System
open Common
open DslKeywords
open DslForCode
open Generator.LanguageExpressions.ExpressionHelpers
open Generator.LanguageHelpers
open type Generator.LanguageHelpers.Structural
open Generator.LanguageHelpers.Statements


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
        let methodName = "MethodA"
        let methodReturn = "int"
        let comparison = Compare (InvokeExpression propertyName "Length" []) Equals (Literal 0)
        let code = 
            Namespace(nspaceName) {
                Using "System" ""
                Using "System.Linq" ""

                Class(className) {
                    Public2

                    Field("A", "string") { Private }

                    Constructor() 
                        { Public2
                          Parameter paramName propertyType
                          Assign propertyName To paramName
                        }

                    Property(propertyName, "int") { Public2 }

                    Method(methodName)
                        { Public2
                          ReturnType methodReturn
                          AssignWithVar "x" To "0" 
                          If (comparison) {
                                Return (Literal 0)
                                }
                         }

                }
            }
        ()