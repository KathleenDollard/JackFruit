namespace Generator

open Microsoft.CodeAnalysis
open Generator.Models

type IAppModel<'T> =
    abstract member Children: 'T -> 'T list
    // Id, method, stuff for pocket
    abstract member Info: SemanticModel -> 'T -> string * IMethodSymbol option * (string * obj) list
    abstract member RunProviders: CommandDef -> CommandDef


