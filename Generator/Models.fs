module Generator.Models

open Microsoft.CodeAnalysis

type ArgDef =
    { ArgId: string
      Name: string
      Description: string option
      Required: bool option
      TypeName: string option }


type OptionDef =
    { OptionId: string
      Name: string
      Description: string option
      Aliases: string list
      //Arity: Arity
      Required: bool option
      TypeName: string option}


type CommandDef =
    { CommandId: string
      Name: string
      Description: string option
      Arg: ArgDef option
      Options: OptionDef list 
      SubCommands: CommandDef list}


type ArchetypeInfo =
    { AncestorsAndThis: string list 
      Raw: string list
      HandlerExpression: SyntaxNode option }