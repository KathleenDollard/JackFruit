module Generator.JackfruitHelpers

open Generator.LanguageModel
open Generator.Models
open Common
open Generator.LanguageExpression
open Generator.GeneralUtils
open Generator.LanguageRoslynOut

type CommandDef with
    member this.MethodName = $"{this.Name}Command".AsPublic
    member _.VariableName = "command"
    member _.PropertyName = "Command"

type MemberDef with
    member this.NameAsVariable = $"{this.Name}{this.KindName}"
    member this.NameAsProperty = $"{this.Name}{this.KindName}"
    member this.SymbolType = GenericNamedItem (this.KindName, [ this.TypeName ])




