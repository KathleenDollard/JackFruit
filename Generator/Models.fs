module Generator.Models

open Microsoft.CodeAnalysis

let (|Command|Arg|Option|) (part: string) =
    let part = part.Trim()

    if part.Length = 0 then
        Command part
    else
        match part.[0] with
        | '<' -> Arg part
        | '-' -> Option part
        | _ -> Command part

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


type SymbolDef =
    | ArgDef of ArgDef
    | OptionDef of OptionDef
    | CommandDef of CommandDef


type ArchetypeInfo =
    { AncestorsAndThis: string list 
      Raw: string list
      HandlerExpression: SyntaxNode option }

