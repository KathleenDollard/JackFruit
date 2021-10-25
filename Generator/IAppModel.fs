namespace Generator

open Microsoft.CodeAnalysis
open Generator.Models


type AppModelInfo =
    { InfoCommandId: string option
      Path: string list
      Method: IMethodSymbol option
      ForPocket: (string * obj) list }

type IAppModel<'T> =
    abstract member Children: 'T -> 'T list
    // Id, method, stuff for pocket
    abstract member Info: SemanticModel -> 'T -> AppModelInfo
    abstract member RunProviders: CommandDef -> CommandDef


