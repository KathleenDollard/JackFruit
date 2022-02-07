namespace CliApp

open System
open System.Collections.Generic
open System.CommandLine

type DescriptionSource =
  | XmlComment = 2 
  | Attribute = 4
  | All = 6


type AliasSource =
  | XmlComment = 2 
  | Attribute = 4
  | All = 6


type IsArgumentSource =
  | XmlComment = 2 
  | Attribute = 4
  | All = 6


type OptionArgumentNameSource =
  | XmlComment = 2 
  | Attribute = 4
  | All = 6


type CommandBase() =
    member this.AddSubCommand(codeToRun: Delegate) = ()


type AppBase() =
    member val CommonAliases = new Dictionary<string, string>() with get

    static member DefaultPatterns = ["*"; "Run*";"*Handler"]

    /// <summary>
    /// The name of your command delegate may differ from the command name
    /// based on a pattern.Pattern portions are removed to determine the '
    /// command name. The default supported patterns are "YourName", 
    /// "RunYourName", and "YourNameHandler", all of which result in 
    /// command named YourName. Use AddDelegatePattern for additional 
    /// patterns with an asterisk where the name appears, like "Patternn*".
    /// </summary>
    /// <param name="pattern"></param>
    static member public AddCommandNamePattern(pattern: string) = ()

    /// <summary>
    /// The name of your command delegate may differ from the command name
    /// based on a pattern.Pattern portions are removed to determine the '
    /// command name. The default supported patterns are "YourName", 
    /// "RunYourName", and "YourNameHandler", all of which result in 
    /// command named YourName. Use RemoveDelegatePattern to remove 
    /// one of these patterns: "Run*" or "*Handler".
    /// </summary>
    /// <param name="pattern"></param>
    static member public RemoveCommandNamePattern(pattern: string) = ()


    member this.DescriptionSources = DescriptionSource.All;
    member this.AliasSources = AliasSource.All;
    member this.IsArgumentSource = IsArgumentSource.All;
    member this.OptionArgumentNameSource optionArgumentNameSource = OptionArgumentNameSource.All;

    static member AddRootCommand(codeToRun: Delegate) = ()
    static member AddSubCommand(codeToRun: Delegate) = ()

    member this.AddCommonAliases(pairs: IEnumerable<(string * string)>) =
        for pair in pairs do
            match pair with
            | alias, optionName -> this.CommonAliases.Add(alias, optionName)
        ()
