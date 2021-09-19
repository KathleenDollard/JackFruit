module Model

open Microsoft.CodeAnalysis.CSharp.Syntax

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
          ParentCommandNames: string list
          Arg: ArgDef option
          Options: OptionDef list }


    type CommandInfo =
        { Raw: string
          Path: string
          Arg: string
          Options: string list
          HandlerExpression: ExpressionSyntax }


    type ArchetypeInfo =
        { AncestorsAndThis: string list 
          CommandInfo: CommandInfo }