module Generator.BuildCliCodeModel

open Generator.LanguageModel
open Generator.Models
open DslCodeBuilder
open type Generator.LanguageHelpers.Structural
open Generator.LanguageHelpers.Statements
open Generator.LanguageExpressions
open Generator.LanguageExpressions.ExpressionHelpers
open Generator.LanguageHelpers
open Generator.JackfruitHelpers
open DslKeywords


let generatedCommandHandlerName (_: CommandDef) = "GeneratedHandler"

// TODO: Flesh out these
let optionSpecificValues (memberDef:MemberDef) = []
let argumentSpecificValues (memberDef:MemberDef) = []


let OutputCommandWrapper (commandDefs: CommandDef list) : Result<NamespaceModel, AppErrors> =

    let methodForCommandDef (parentCommandDef: CommandDef) : IMember list =
        let rec recurse (recurseDepth: int) (commandDef: CommandDef) : IMember list =
            if recurseDepth > 10 then invalidOp "Runaway recursion suspected!"
            let commandMethodName = "Command"
            let setHandlerName = "SetHandler"
            let add = "Add"
            
            [ Method commandDef.MethodName {
                ReturnType commandMethodName
                Public()
                AssignWithVar commandDef.VariableName To (New commandMethodName [ StringLiteral commandDef.Name ])
                for mbr in commandDef.Members do
                    AssignWithVar mbr.NameAsVariable To (New mbr.SymbolType [ StringLiteral mbr.Name ])
                    match mbr.Description with 
                    | Some desc -> Assign $"{mbr.NameAsVariable}.Description" To mbr.Description
                    | None -> ()
                    Invoke commandDef.VariableName add [ Literal mbr.NameAsVariable ] 

                Invoke commandDef.VariableName setHandlerName [
                    SymbolLiteral (Symbol commandDef.HandlerMethodName)
                    for mbr in commandDef.Members do (Literal (mbr.NameAsVariable))]
                Return (SymbolLiteral (Symbol commandDef.VariableName)) }
              // KAD-Chet: Does this need to be this ugly? Help :( also below within the try
              let subCommandMethods = 
                [ for subCommand in commandDef.SubCommands do 
                    for m in recurse (recurseDepth + 1) subCommand do m ]
              for method in subCommandMethods do method ]
        recurse 0 parentCommandDef

    try
        // KAD: Figure out right namespace: Should probably collect the correct namespace from the initial code. 
        let nspace = Namespace ("CliDefinition") {
            UsingModel.Create "System" 
            UsingModel.Create "System.CommandLine"
            UsingModel.Create "System.CommandLine.Invocation"
            UsingModel.Create "System.Threading.Tasks"

            for commandDef in commandDefs do 
                Class($"{commandDef.Name}Cli") {
                    Public()
                    for method in methodForCommandDef commandDef do method }
            }
        Ok nspace

    with
    | ex -> Error (Other $"Error creating code model {ex.Message}")

