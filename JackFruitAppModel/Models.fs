module Jackfruit.Models

open Microsoft.CodeAnalysis

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

