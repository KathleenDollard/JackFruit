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


let generatedCommandHandlerName (_: CommandDef) = "GeneratedHandler"

// TODO: Flesh out these
let optionSpecificValues (memberDef:MemberDef) = []
let argumentSpecificValues (memberDef:MemberDef) = []


let OutputCommandWrapper (commandDefs: CommandDef list) : Result<NamespaceModel, AppErrors> =

    let className (commandDef:CommandDef) = $"{commandDef.Name}Cli"

    let appClass(commandDef: CommandDef) = 
        let className = className commandDef
        let classNamedItem = NamedItem.SimpleNamedItem className
        let fieldName = "rootCommand"
        Class ($"{commandDef.Name}App") {
            Internal Partial
            InheritedFrom "AppBase"
            Property ("RootCommand", commandDef.Name) { 
                Public2
            }
            Method("Create") {
                Public2 Static
                ReturnType className
                AssignWithVar "newApp" To (New commandDef.Name [])
                Return (SymbolLiteral (Symbol "newApp"))
            }
        }

    let commandConstructor (commandDef: CommandDef) =
        Constructor() {
            Public2
            Assign "Command" To (New "Command" [ StringLiteral commandDef.Name ])
            for mbr in commandDef.Members do
                Assign mbr.NameAsProperty To (New mbr.SymbolType [ StringLiteral mbr.Name ])
                match mbr.Description with 
                | Some desc -> Assign $"{mbr.NameAsProperty}.Description" To mbr.Description
                | None -> ()
                Invoke "Command" "Add" [ Literal mbr.NameAsProperty ] 
            Assign "Command.Handler" To ThisLiteral
            }

    let memberResult(mbr: MemberDef) =
        let propertyAccess = Literal mbr.NameAsProperty
        Method (mbr.NameAsResult) { 
            Public2 
            Parameter "context" "InvocationContext"
            // KAD-Chet: Work on the following line
            ReturnType (ReturnType.ReturnType mbr.TypeName)
            // KAD-Chet: Work on Symbol in the following line
            Return (Invoke "context.ParseResult" $"GetValueFor{mbr.KindName}<{mbr.TypeName}>" [ propertyAccess ] )
            }

    let invokeMethod (commandDef: CommandDef) =
        let handlerName = commandDef.CommandId
        let invokeReturnType = ReturnType.ReturnType (NamedItem.Create ("Task", ["int"]))
        Method("InvokeAsync") {
            Public2
            ReturnType invokeReturnType
            Invoke "" handlerName [ 
                for mbr in commandDef.Members do
                    Invoke "" mbr.NameAsResult [ SymbolLiteral (Symbol "context") ] ]
            // TODO: Create call to property value
            Return (Invoke "Task" "FromResult" [ SymbolLiteral (Symbol "context.ExitCode") ] )
            }

    let commandClass (rootCommandDef: CommandDef) =

        let rec recurse (recurseDepth: int) (commandDef: CommandDef) =
            if recurseDepth > 10 then invalidOp "Runaway recursion suspected!"
            let className = className commandDef

            [ Class className {
                Public2 Partial
                InheritedFrom "CliRootCommand"
                ImplementsInterface "ICommandHandler"
                Property ("Command", "Command") { Public2 }
                commandConstructor commandDef

                for mbr in commandDef.Members do
                    Property (mbr.NameAsProperty, mbr.SymbolType) { Public2 }
                    memberResult mbr

                for subCommand in commandDef.SubCommands do
                    Property (subCommand.Name, "CommandBase") { Public2 }

                invokeMethod commandDef
                }

              for subCommand in commandDef.SubCommands do 
                for cls in recurse (recurseDepth + 1) subCommand do
                    cls
            ]

        recurse 0 rootCommandDef

    try
        // KAD: Figure out right namespace: Should probably collect the correct namespace from the initial code. 
        let nspace = Namespace ("CliDefinition") {
            Using "System" 
            Using "System.CommandLine"
            Using "System.CommandLine.Invocation"
            Using "System.Threading.Tasks"
            Using "CliApp"
            
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

