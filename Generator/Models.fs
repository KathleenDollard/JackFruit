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


type MemberKind =
    | Option
    | Argument
    | Service

type MemberDef =
    { MemberId: string
      TypeName: string
      MemberKind: MemberKind option
      Aliases: string list
      ArgDisplayName: string option
      Description: string option
      RequiredOverride: bool option }
    static member Create memberId typeName =
        { MemberId = memberId
          TypeName = typeName
          MemberKind = None
          Aliases = []
          ArgDisplayName = None
          Description = None
          RequiredOverride = None }
    member this.UseMemberKind =
        match this.MemberKind with
        | Some k -> k
        | None -> MemberKind.Option
    // KAD-Don: Remind me the downside of using "this"
    member this.Name =
        let innerName = 
            if this.Aliases.IsEmpty then
                this.MemberId
            else
                this.Aliases.Head
        match this.UseMemberKind with
        | Argument -> 
            match this.ArgDisplayName with 
            | Some n -> n
            | None -> innerName
        | _ -> innerName


type CommandDef =
    { CommandId: string
      Path: string list
      Aliases: string list
      Description: string option
      Members: MemberDef list
      SubCommands: CommandDef list
      Pocket: (string * obj) list}
    static member Create commandId =
        { CommandId = commandId
          Path = []
          Description = None
          Aliases = []
          Members = []
          SubCommands = [] 
          Pocket = [] }
    static member CreateRoot =
        { CommandId = ""
          Path = []
          Aliases = []
          Description = None
          Members = []
          SubCommands = [] 
          Pocket =[] }


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
      Handler: SyntaxNode option
      }

