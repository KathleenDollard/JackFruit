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


type CliBase() =
    member this.AddSubCommand(codeToRun: Delegate) = ()

type AppBase() =

    static let mutable defaultPatterns = ["*"; "Run*";"*Handler"]

    member val CommonAliases = Dictionary<string, string>() with get
    member this.AddCommonAliases(pairs: IEnumerable<(string * string)>) =
        for pair in pairs do
            match pair with
            | alias, optionName -> this.CommonAliases.Add(alias, optionName)
        ()

    static member val DefaultPatterns = defaultPatterns 
    /// <summary>
    /// The name of your command delegate may differ from the command name
    /// based on a pattern.Pattern portions are removed to determine the '
    /// command name. The default supported patterns are "YourName", 
    /// "RunYourName", and "YourNameHandler", all of which result in 
    /// command named YourName. Use AddDelegatePattern for additional 
    /// patterns with an asterisk where the name appears, like "Patternn*".
    /// </summary>
    /// <param name="pattern"></param>
    static member public AddCommandNamePattern(pattern: string) = 
        defaultPatterns <- List.distinct (pattern :: defaultPatterns)
    /// <summary>
    /// The name of your command delegate may differ from the command name
    /// based on a pattern.Pattern portions are removed to determine the '
    /// command name. The default supported patterns are "YourName", 
    /// "RunYourName", and "YourNameHandler", all of which result in 
    /// command named YourName. Use RemoveDelegatePattern to remove 
    /// one of these patterns: "Run*" or "*Handler".
    /// </summary>
    /// <param name="pattern"></param>
    static member public RemoveCommandNamePattern(pattern: string) = 
        defaultPatterns <- 
            match List.tryFindIndex (fun x -> x = pattern) defaultPatterns with
            | Some i -> List.removeAt i defaultPatterns
            | None -> defaultPatterns

    member _.DescriptionSources = DescriptionSource.All;
    member _.AliasSources = AliasSource.All;
    member _.IsArgumentSource = IsArgumentSource.All;
    member _.OptionArgumentNameSource optionArgumentNameSource = OptionArgumentNameSource.All;

    static member CreateWithRootCommand(codeToRun: Delegate) = ()


type AppBase<'T when 'T :> CliBase and 'T: (new: unit -> 'T)>(rootCli: 'T)  =   
    //inherit AppBase()
    member _.Temp = ()

    //static member CreateWithRootCommand(codeToRun: Delegate) = 
    //    AppBase<'T> ('T)

type OriginalSeries() =
    inherit CliBase()
    let temp = ""



type MyAppBase() =
    inherit AppBase<OriginalSeries>(OriginalSeries())

    let _rootCommand = OriginalSeries()
    // Deliberate shadowing 
    static member CreateWithRootCommand() = MyAppBase()
    member _.RootCommand = _rootCommand
