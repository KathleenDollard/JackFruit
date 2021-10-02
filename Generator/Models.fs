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
      TypeName: string}


type OptionDef =
    { OptionId: string
      Name: string
      Description: string option
      Aliases: string list
      //Arity: Arity
      Required: bool option
      TypeName: string}


type CommandDef =
    { CommandId: string
      Name: string
      Description: string option
      Aliases: string list
      Arg: ArgDef option
      Options: OptionDef list 
      SubCommands: CommandDef list}


type SymbolDef =
    | ArgDef of ArgDef
    | OptionDef of OptionDef
    | CommandDef of CommandDef


type ArchetypePart = 
    { Id: string
      Name: string
      Aliases: string list
      HiddenAliases: string list }

type ArchetypeParts =
    | CommandArchetype of part: ArchetypePart
    | OptionArchetype of part: ArchetypePart
    | ArgArchetype of part: ArchetypePart

type ArchetypeInfo =
    { AncestorsAndThis: string list 
      Raw: ArchetypeParts list
      HandlerExpression: SyntaxNode option }

