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
open Generator.LanguageExpressions.ExpressionHelpers
open Generator.LanguageHelpers


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

let baseMemberName (memberDef: MemberDef) = $"{memberDef.Name}{memberDef.KindName}"
let memberVarName (memberDef: MemberDef) = $"{ToCamel (baseMemberName memberDef)}"
let memberPropName (memberDef: MemberDef) = $"{ToPascal (baseMemberName memberDef)}"
let memberParamName (memberDef: MemberDef) = ToCamel (baseMemberName memberDef)
let memberSymbolType (memberDef: MemberDef) = GenericNamedItem (memberDef.KindName, [ memberDef.TypeName ])

let commandVarName ="command"  
let commandPropName (_: CommandDef) = "Command"
let commandClassName (commandDef: CommandDef) = $"{ToPascal commandDef.Name}CommandWrapper"
let commandMethodName (commandDef: CommandDef) = $"{ToPascal commandDef.Name}CliCommandWrapper"

let generatedCommandHandlerName (_: CommandDef) = "GeneratedHandler"

// TODO: Flesh out these
let optionSpecificValues (memberDef:MemberDef) = []
let argumentSpecificValues (memberDef:MemberDef) = []


let OutputCommandWrapper (commandDefs: CommandDef list) : Result <NamespaceModel, AppErrors> =

//var option = new Option<int>("-i");
//var argument = new Argument<string>("-s");
//var command = new Command("Name")
//Command.
//    option,
//    argument
//};

//// This is new!
//rootCommand.SetHandler(
//    (int someInt, string someString) => { /* Do something exciting! */ }, 
//    option, argument);

     

    let methodForCommandDef (commandDef: CommandDef) =
        let methodName = commandMethodName commandDef
        Method (methodName, ReturnType "Command") {
            Public()
            AssignWithVar commandVarName (New "Command" [ StringLiteral commandDef.Name ])
            for mbr in commandDef.Members do
                let varName = memberVarName mbr
                AssignWithVar varName (New (memberSymbolType mbr) [])
                match mbr.Description with 
                | Some desc -> Assign $"{varName}.Description" mbr.Description
                | None -> ()
                Invoke commandVarName "Add" [ GetExpression varName ] 

            Comment "In the following, the hard coded handler name is wrong, but I am just getting code structure correct"
            Invoke commandVarName "SetHandler" [
                GetExpression """commandDef.MethodName"""
                for mbr in commandDef.Members do (GetExpression (memberVarName mbr))]
            Return (Symbol commandVarName) }

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
                    methodForCommandDef commandDef }
            }
        Ok nspace

    with
    | ex -> Error (Other $"Error creating code model {ex.Message}")

