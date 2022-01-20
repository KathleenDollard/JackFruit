module Generator.BuildCliCodeModel

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

    let rec CommandClass (recurseDepth: int) (commandDef: CommandDef) =
        if recurseDepth > 10 then invalidOp "Runaway recursion suspected!"
        let className = $"{commandDef.Name}Cli"
        let Command = "Command"
        let operationAsField = "_operation"
        let operationAsParam = "operation"
        let operationType = DelegateSignature commandDef.ParameterTypes commandDef.ReturnType
        let handlerName = "Command.Handler"
        let invokeReturnType = ReturnType.ReturnType (NamedItem.Create ("Task", ["int"]))
                
        [ Class className {
            Public2
            // TODO: Make this property Get only
            Property (operationAsField, operationType) { Private }
            Property (Command, Command) { Public2 }

            Constructor() {
                Public2
                Parameter operationAsParam operationType
                Assign operationAsField To operationAsParam
                Assign Command To (New Command [ StringLiteral commandDef.Name ])
                for mbr in commandDef.Members do
                    Assign mbr.NameAsProperty To (New mbr.SymbolType [ StringLiteral mbr.Name ])
                    match mbr.Description with 
                    | Some desc -> Assign $"{mbr.NameAsProperty}.Description" To mbr.Description
                    | None -> ()
                    Invoke Command "Add" [ Literal mbr.NameAsProperty ] 
                Assign handlerName To ThisLiteral
                }

            for mbr in commandDef.Members do
                if mbr.GenerateSymbol then 
                    Property (mbr.NameAsProperty, mbr.SymbolType) { Public2 }
                Method (mbr.NameAsResult) { 
                    Public2 
                    Parameter "context" "InvocationContext"
                    // TODO: Work on the following line
                    ReturnType (ReturnType.ReturnType mbr.TypeName)
                    // TODO: Work on Symbol in the following line
                    Return (Invoke "context.ParseResult" $"GetValueFor{mbr.SymbolType}<{mbr.TypeName}>" [ SymbolLiteral (Symbol mbr.NameAsProperty) ] )
                    }

            Method("InvokeAsync") {
                Public2
                ReturnType invokeReturnType
                Invoke "" operationAsField [ 
                    for mbr in commandDef.Members do
                        Invoke "" mbr.NameAsResult [ SymbolLiteral (Symbol "context") ] ]
                // TODO: Create call to property value
                Return (Invoke "Task" "FromResult" [ SymbolLiteral (Symbol "context.ExitCode") ] )
                }
            }
                
          for subCommand in commandDef.SubCommands do 
                for cls in CommandClass (recurseDepth + 1) subCommand do
                    cls
        ]


    try
        // KAD: Figure out right namespace: Should probably collect the correct namespace from the initial code. 
        let nspace = Namespace ("CliDefinition") {
            Using "System" 
            Using "System.CommandLine"
            Using "System.CommandLine.Invocation"
            Using "System.Threading.Tasks"
            
            for commandDef in commandDefs do 
                for cls in CommandClass 0 commandDef do
                    cls

            }
        Ok nspace.Model

    with
    | ex -> Error (Other $"Error creating code model {ex.Message}")

