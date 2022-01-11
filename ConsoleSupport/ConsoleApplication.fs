namespace Generator.ConsoleSupport

open System.CommandLine;
open System.Collections.Generic
open System

type ConsoleApplication(rootCommand: RootCommand) =

    new () = ConsoleApplication (RootCommand())

    member _.RootCommand: RootCommand = rootCommand 
    
    member val CommonAliases: Dictionary<string, string> = 
           Dictionary<string, string>()
        with get

    member this.AddCommonAlias(alias: string, optionName: string) : ConsoleApplication =
        this.CommonAliases.Add(alias, optionName)
        this

    member this.MapInferred(def: string,del: Delegate) = this // expect future implementation

    member this.UseExceptionHandler(?path: string) = this // example of helpful console things - future

    member this.Run (args: string array) =
        this.RootCommand.Invoke(args)

    member this.Run (args: string) =
        this.RootCommand.Invoke(if args = null then "" else args)

    // The following method has very little testing,so is guarded to avoid infinite recursion 
    member this.RunInLoop (args: string[], message: string) =
        let rec run recurseDepth lastRet: int =
            if recurseDepth > 10 then invalidOp "Can currently run just ten times"
            Console.WriteLine(message)
            let input = Console.ReadLine()
            if input = null then // user is exiting
                lastRet
            else
                let ret = this.Run input
                run (recurseDepth + 1) ret

        let ret =
            match args with 
            | [||] -> 0
            | _ -> this.Run args

        run 0 ret

    // The following method has very little testing,so is guarded to avoid infinite recursion 
    member this.RunInLoop (args: string[]) =
        this.RunInLoop (args, "Experiment with your CLI! Ctl-Z or Ctl-C to exit.")
        



