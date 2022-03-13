module Generator.JackfruitHelpers

open Generator.LanguageModel
open Generator.Models
open Common
open Generator.LanguageExpression
open Generator.GeneralUtils
open Generator.LanguageRoslynOut
open System

type CommandDef with
    member this.NameAsPascal =
        Char.ToUpper(this.Name[0]).ToString() + this.Name[1..]
    member this.TypeName = $"{this.Name}Command".AsPublic
    member _.VariableName = "command"
    member _.PropertyName = "Command"

type MemberDef with
    member this.NameAsPascal =
        Char.ToUpper(this.Name[0]).ToString() + this.Name[1..]
    member this.NameAsVariable = $"{this.NameAsPascal}{this.KindName}"
    member this.NameAsProperty = $"{this.NameAsPascal}{this.KindName}"
    member this.NameAsResult =   $"{this.NameAsPascal}{this.KindName}Result"
    member this.SymbolType = GenericNamedItem (this.KindName, [ this.TypeName ])




