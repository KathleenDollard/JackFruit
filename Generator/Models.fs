module Generator.Models

open Microsoft.CodeAnalysis

type AppErrors =
    | Roslyn of Diagnostics: Diagnostic list
    | UnexpectedExpression of Message: string
    | UnexpectednumberOfArguments
    | UnexpectedExpressionForHandler of Archetype: string
    | UnexpectedExpressionForArchetype of Position: int
    | ParsingArchetype of Message: string
    | BulidingTree of Message: string
    | BuildingCommanDef of Message: string
    | NotImplemented of Message: string
    | Other of Message: string
    | Aggregate of Errors: AppErrors list


type ArgDef =
    { ArgId: string
      Name: string
      Description: string option
      Required: bool option
      TypeName: string }
    with static member Create argId typeName =
        { ArgId = argId
          Name = argId
          Description = None
          Required = None
          TypeName = typeName }


type OptionDef =
    { OptionId: string
      Name: string
      Description: string option
      Aliases: string list
      //Arity: Arity
      Required: bool option
      TypeName: string }
    static member Create optionId typeName=
        { OptionId = optionId
          Name = optionId
          Description = None
          Aliases = []
          Required = None
          TypeName = typeName }


type CommandDef =
    { CommandId: string
      Path: string list
      Name: string
      Description: string option
      Aliases: string list
      Arg: ArgDef option
      Options: OptionDef list 
      SubCommands: CommandDef list}
    static member Create commandId =
        { CommandId = commandId
          Path = []
          Name = commandId
          Description = None
          Aliases = []
          Arg = None
          Options = []
          SubCommands = [] }
    static member CreateRoot =
        { CommandId = ""
          Path = []
          Name = ""
          Description = None
          Aliases = []
          Arg = None
          Options = []
          SubCommands = [] }


type SymbolDef =
    | ArgDef of ArgDef
    | OptionDef of OptionDef
    | CommandDef of CommandDef


type ArchPart = 
    { Id: string
      Name: string
      Aliases: string list
      HiddenAliases: string list }


type ArchetypePart =
    | CommandArchetype of part: ArchPart
    | OptionArchetype of part: ArchPart
    | ArgArchetype of part: ArchPart


type ArchetypeInfo =
    { Path: string list 
      ArchetypeParts: ArchetypePart list
      Handler: SyntaxNode option }

