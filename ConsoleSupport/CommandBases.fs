namespace CommandBase

open System.CommandLine
open System

type CliCommand(commandName) =
    inherit Command(commandName)
    // KAD-Don: Could we be more forgiving of extra parens in the type signature. When there are errors, adding parens is common,and then when I got the rest of the sig right, this failed.
    // At one point this method was abstract
    //abstract member AddSubCommand (codeToRun: Delegate) -> unit
    //abstract member AddSubCommand : codeToRun: Delegate -> unit
    member _.AddSubCommand (codeToRun: Delegate) = ()



type CliRootCommand() =
    inherit RootCommand("")

    member _.AddSubCommand (codeToRun: Delegate) = ()
