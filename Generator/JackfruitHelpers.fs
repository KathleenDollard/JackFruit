module Generator.JackfruitHelpers

open Generator.Language
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


//let baseMemberName (memberDef: MemberDef) = $"{memberDef.Name}{memberDef.KindName}"
//let memberVarName (memberDef: MemberDef) = $"{ToCamel (baseMemberName memberDef)}"
//let memberPropName (memberDef: MemberDef) = $"{ToPascal (baseMemberName memberDef)}"
//let memberParamName (memberDef: MemberDef) = ToCamel (baseMemberName memberDef)
//let memberSymbolType (memberDef: MemberDef) = GenericNamedItem (memberDef.KindName, [ memberDef.TypeName ])

//let commandVarName ="command"  
//let commandPropName (_: CommandDef) = "Command"
//let commandClassName (commandDef: CommandDef) = $"{ToPascal commandDef.Name}CommandWrapper"
//let commandMethodName (commandDef: CommandDef) = $"{ToPascal commandDef.Name}CliCommandWrapper"


