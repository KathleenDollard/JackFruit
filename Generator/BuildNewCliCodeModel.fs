module Generator.BuildNewCliCodeModel

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

    let CommandConstructor (commandDef: CommandDef) =
        Constructor() {
            Public()
            Assign "Command" To (New "Command" [ StringLiteral commandDef.Name ])
            for mbr in commandDef.Members do
                Assign mbr.NameAsProperty To (New mbr.SymbolType [ StringLiteral mbr.Name ])
                match mbr.Description with 
                | Some desc -> Assign $"{mbr.NameAsProperty}.Description" To mbr.Description
                | None -> ()
                Invoke "Command" "Add" [ Literal mbr.NameAsProperty ] 
            Invoke commandDef.VariableName "SetHandler" [
                SymbolLiteral (Symbol commandDef.HandlerMethodName)
                for mbr in commandDef.Members do (Literal (mbr.NameAsVariable))]
            }


    let CommandClass (rootCommandDef: CommandDef) =

        let rec recurse (recurseDepth: int) (commandDef: CommandDef) =
            if recurseDepth > 10 then invalidOp "Runaway recursion suspected!"
            let className = $"{commandDef.Name}Cli"
                
            [ Class className {
                Public()
                Property ("Command", "Command") {
                    Public()
                }
                CommandConstructor commandDef
                for mbr in commandDef.Members do
                    Property (mbr.NameAsProperty, mbr.TypeName) {
                        Public()
                    }
                }
                
              for subCommand in commandDef.SubCommands do 
                    for cls in recurse (recurseDepth + 1) subCommand do
                        cls
            ]

        recurse 0 rootCommandDef

    try
        // KAD: Figure out right namespace: Should probably collect the correct namespace from the initial code. 
        let nspace = Namespace ("CliDefinition") {
            UsingModel.Create "System" 
            UsingModel.Create "System.CommandLine"
            UsingModel.Create "System.CommandLine.Invocation"
            UsingModel.Create "System.Threading.Tasks"
            
            for commandDef in commandDefs do 
                for cls in CommandClass commandDef do
                    cls

            }
        Ok nspace

    with
    | ex -> Error (Other $"Error creating code model {ex.Message}")

