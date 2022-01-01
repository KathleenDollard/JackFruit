module Generator.BuildCliCodeModel

open Common
open Generator.Language
open Generator.Models
open Generator.LanguageStatements
open Generator.GeneralUtils
open DslCodeBuilder
open Generator.LanguageRoslynOut
open type Generator.LanguageHelpers.Structural
open Generator.LanguageHelpers.Statements
open Generator.LanguageExpressions
open Generator.LanguageExpressions.ExpressionHelpers
open Generator.LanguageHelpers
open System
open Generator.JackfruitHelpers


let private operationName = "operation"
let private operationFieldName = "_operation"

let private OutputHeader (outputter: RoslynOut) =
    outputter.OutputComment(CommentModel.Create "Copyright (c) .NET Foundation and contributors. All rights reserved.")

    outputter.OutputComment(
        CommentModel.Create "Licensed under the MIT license. See LICENSE file in the project root for full license information."
    )

    outputter.BlankLine()

    outputter.OutputCompilerDirective(CompilerDirectiveModel.Create (CompilerWarning ""))
    ()
    
let private methodSigFromCommandDef (commandDef: CommandDef) =
    let isAction = 
        match commandDef.ReturnType  with
        | Void -> true
        | _ -> false
    let memberTypes = 
        [ for memberDef in commandDef.Members do 
            memberDef.TypeName
            match commandDef.ReturnType with 
            | ReturnType t -> t
            | Void -> () ]
    let name = if isAction then "Action" else "Func"
    GenericNamedItem (name, memberTypes)

let generatedCommandHandlerName (_: CommandDef) = "GeneratedHandler"

// TODO: Flesh out these
let optionSpecificValues (memberDef:MemberDef) = []
let argumentSpecificValues (memberDef:MemberDef) = []


let OutputCommandWrapper (commandDefs: CommandDef list) : Result <NamespaceModel, AppErrors> =

    let rec methodForCommandDef (commandDef: CommandDef) =
        Method (commandDef.MethodName, ReturnType "Command") {
            Public()
            AssignWithVar commandDef.VariableName (New "Command" [ StringLiteral commandDef.Name ])
            for mbr in commandDef.Members do
                AssignWithVar mbr.NameAsVariable (New mbr.SymbolType [ StringLiteral mbr.Name ])
                match mbr.Description with 
                | Some desc -> Assign $"{mbr.NameAsVariable}.Description" mbr.Description
                | None -> ()
                Invoke commandDef.VariableName "Add" [ Literal mbr.NameAsVariable ] 

            Comment "In the following, the hard coded handler name is wrong, but I am just getting code structure correct"
            Invoke commandDef.VariableName "SetHandler" [
                SymbolLiteral (Symbol commandDef.HandlerMethodName)
                for mbr in commandDef.Members do (Literal (mbr.NameAsVariable))]
            Return (SymbolLiteral (Symbol commandDef.VariableName)) }

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
                    methodForCommandDef commandDef
                    for subCommand in commandDef.SubCommands do 
                       methodForCommandDef subCommand }
            }
        Ok nspace

    with
    | ex -> Error (Other $"Error creating code model {ex.Message}")

