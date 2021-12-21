module Generator.BuildCliCodeModel

open Generator.Language
open Generator.Models
open Common
open Generator.LanguageExpression
open Generator.GeneralUtils
open DslCodeBuilder

let private operationName = "operation"
let private operationFieldName = "_operation"

let private OutputHeader (outputter: RoslynOut) =
    outputter.OutputComment(Comment "Copyright (c) .NET Foundation and contributors. All rights reserved.")

    outputter.OutputComment(
        Comment "Licensed under the MIT license. See LICENSE file in the project root for full license information."
    )

    outputter.BlankLine()

    outputter.OutputPragma(Pragma "warning disable")
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
            | Type t -> t
            | Void -> () ]
    let name = if isAction then "Action" else "Func"
    GenericNamedItem (name, memberTypes)

let baseMemberName (memberDef: MemberDef) = $"{memberDef.Name}{memberDef.KindName}"
let memberFieldName (memberDef: MemberDef) = $"_{ToCamel (baseMemberName memberDef)}"
let memberPropName (memberDef: MemberDef) = $"{ToPascal (baseMemberName memberDef)}"
let memberParamName (memberDef: MemberDef) = ToCamel (baseMemberName memberDef)
let memberSymbolType (memberDef: MemberDef) = GenericNamedItem (memberDef.KindName, [ memberDef.TypeName ])

let commandFieldName (_: CommandDef)  ="_command"  
let commandPropName (_: CommandDef) = "Command"
let commandClassName (commandDef: CommandDef) = $"{ToPascal commandDef.Name}CommandWrapper"

let generatedCommandHandlerName (_: CommandDef) = "GeneratedHandler"

// TODO: Flesh out these
let optionSpecificValues (memberDef:MemberDef) = []
let argumentSpecificValues (memberDef:MemberDef) = []


let OutputCommandWrapper (commandDefs: CommandDef list) : Result <NamespaceModel, AppErrors> =



    let classForCommandDef (commandDef: CommandDef) =
        let className = commandClassName commandDef
        Class (className)
            {
                Public
                Members
                    [ 
                        Field ("operation", GenericNamedItem ("Action", [SimpleNamedItem "int"]))
                            {
                                // TODO: Make private the default and support Zero so body can be empty
                                Private // TODO: Add private support
                            }
                        Constructor(className) 
                            {
                                Public
                            }
                        // TODO: Add singleton support for the following two members
                        Field ("command", SimpleNamedItem "Command")
                            {
                                Public // TODO: Add private support
                            }
                        Property("Command", SimpleNamedItem "Command")
                            {
                                Public
                            }
                        Class("GeneratedHandler")
                            {
                                Public
                            }
                    ]
            }

    let classForCommandDef (commandDef: CommandDef) = ()


    let classes = 
        [ for commandDef in commandDefs do
            classForCommandDef commandDef
        ]

    try
        // KAD: Figure out right namespace
        Ok ({ NamespaceName = "GeneratedHandlers"
              Usings = 
                [ UsingModel.Create "System" 
                  UsingModel.Create "System.CommandLine"
                  UsingModel.Create "System.CommandLine.Invocation"
                  UsingModel.Create "System.Threading.Tasks"]
              Classes = classes })
    with
    | ex -> Error (Other $"Error creating code model {ex.Message}")

