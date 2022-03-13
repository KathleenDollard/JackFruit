module Generator.BuildNewerCliCodeModel

open Generator.LanguageModel
open Generator.Models
open DslForCode
open type Generator.LanguageHelpers.Structural
open Generator.LanguageHelpers.Statements
open Generator.LanguageExpressions
open Generator.LanguageExpressions.ExpressionHelpers
open Generator.LanguageHelpers
open Generator.JackfruitHelpers
open DslKeywords
open Common
open System
open System.Linq

let generatedCommandHandlerName (_: CommandDef) = "GeneratedHandler"

// TODO: Flesh out these
let optionSpecificValues (memberDef:MemberDef) = []
let argumentSpecificValues (memberDef:MemberDef) = []


let OutputCommandWrapper (commandDefs: CommandDef list) : Result<NamespaceModel, AppErrors> =


    let appClass(commandDef: CommandDef) = 
        let appName (commandDef:CommandDef) = $"{commandDef.Name}App"
        let commandName (commandDef: CommandDef) = $"{commandDef.Name}Command"
        let className = appName commandDef
        let rootCommandClass = commandName commandDef
        let rootCommandProperty = commandDef.Name
        Class (className) {
            Internal Partial
            InheritedFrom "ConsoleApplication"
            Constructor() { 
                Private 
                Parameter "rootCommand" rootCommandClass
                Base [SymbolLiteral (Symbol "rootCommand")]
                Assign rootCommandProperty To "rootCommand" 
                }
            Property (rootCommandProperty, rootCommandClass) { 
                Public2
            }
            Method("Create") {
                Public2 Static HideByName
                //Parameter "codeToRun" "Delegate"
                ReturnType className
                AssignWithVar "newApp" To (New className [Invoke rootCommandClass "Create" []])
                //Assign $"newApp.{rootCommandProperty}" To (Invoke rootCommandClass "Create" [])
                Return (SymbolLiteral (Symbol "newApp"))
            }
        }

    let memberResult(mbr: MemberDef) =
        let propertyAccess = Literal mbr.NameAsProperty
        let invocationName(mbr: MemberDef) = GenericNamedItem ($"GetValueFor{mbr.KindName}", [mbr.TypeName])
        Method (mbr.NameAsResult) { 
            Public2 
            Parameter "context" "InvocationContext"
            // KAD-Chet: Work on the following line
            ReturnType (ReturnType.ReturnType mbr.TypeName)
            // KAD-Chet: Work on Symbol in the following line
            Return (Invoke "context.ParseResult" (invocationName(mbr)) [ propertyAccess ] )
            }

    let invokeMethod (commandDef: CommandDef) =
        let handlerName = commandDef.HandlerMethodName
        let invokeReturnType = ReturnType.ReturnType (NamedItem.Create ("Task", ["int"]))
        Method("InvokeAsync") {
            Public2
            Parameter "context" "InvocationContext"
            ReturnType invokeReturnType
            Invoke "" handlerName [ 
                for mbr in commandDef.Members do
                    Invoke "" mbr.NameAsResult [ SymbolLiteral (Symbol "context") ] ]
            // TODO: Create call to property value
            Return (Invoke "Task" "FromResult" [ SymbolLiteral (Symbol "context.ExitCode") ] )
            }


    let commandClass (rootCommandDef: CommandDef) =
        // KAD: Make a test for Don
        let commandConstructor (isRoot: bool) (name: string) =
            if isRoot then
                Constructor() { 
                    Private }
            else
                Constructor() { 
                    Private 
                    Base [ StringLiteral name ] }

        let rec recurse (recurseDepth: int) (commandDef: CommandDef) =
            if recurseDepth > 10 then invalidOp "Runaway recursion suspected!"
            let thisClassName = commandDef.TypeName
            let isRoot = (recurseDepth = 0)
            let baseClassName = if isRoot then "CliRootCommand" else "CliCommand"

            [ Class thisClassName {
                Public2 Partial
                InheritedFrom baseClassName
                ImplementsInterface "ICommandHandler"

                commandConstructor isRoot commandDef.Name

                Method("Create") {
                    Public2 Static
                    ReturnType thisClassName
                    AssignWithVar "command" To (New thisClassName [])
                    for mbr in commandDef.Members do
                        let propertyName = $"command.{mbr.NameAsProperty}"
                        Assign propertyName To (New mbr.SymbolType [ StringLiteral mbr.Name ])
                        match mbr.Description with 
                        | Some desc -> Assign $"{propertyName}.Description" To mbr.Description
                        | None -> ()
                        Invoke "command" "Add" [ SymbolLiteral (Symbol propertyName) ] 
                    for subCommand in commandDef.SubCommands do 
                        let propertyName = $"command.{subCommand.NameAsPascal}"
                        let subCommandName = subCommand.TypeName
                        Assign propertyName To (Invoke subCommandName "Create" [])
                        Invoke "command" "Add" [ SymbolLiteral (Symbol propertyName) ] 
                    Assign "command.Handler" To (SymbolLiteral (Symbol "command"))
                    Return (SymbolLiteral (Symbol "command"))
                    }

                for mbr in commandDef.Members do
                    Property (mbr.NameAsProperty, mbr.SymbolType) { Public2 }
                    memberResult mbr

                for subCommand in commandDef.SubCommands do
                    Property (subCommand.Name, subCommand.TypeName) { Public2 }

                invokeMethod commandDef
                }

              for subCommand in commandDef.SubCommands do 
                for cls in recurse (recurseDepth + 1) subCommand do
                    cls
            ]

        recurse 0 rootCommandDef

    try
        let appNamespace = 
            if commandDefs.IsEmpty || String.IsNullOrWhiteSpace(commandDefs[0].AppNamespace) then 
                "CliApp"
            else
                commandDefs[0].AppNamespace
        let nspace = Namespace (appNamespace) {
            Using $"System; // Count = {commandDefs.Count()}" 
            Using "System.CommandLine"
            Using "System.CommandLine.Invocation"
            Using "System.Threading.Tasks"
            Using "CommandBase";
            Using "Generator.ConsoleSupport"
            
            match commandDefs with
            | [] -> ()
            | _ -> appClass commandDefs[0]
            
            for commandDef in commandDefs do 
                for cls in commandClass commandDef do
                    cls

            }
        Ok nspace.Model

    with
    | ex -> Error (Other $"Error creating code model {ex.Message}")

