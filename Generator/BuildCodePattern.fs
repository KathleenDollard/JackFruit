module Generator.BuildCodePattern

open Generator.Language
open Generator.Models
open Common
open Generator.LanguageExpression
open Generator.GeneralUtils

//let private operationName = "operation"
//let private operationFieldName = "_operation"

//let private OutputHeader (outputter: RoslynOut) =
//    outputter.OutputComment(Comment "Copyright (c) .NET Foundation and contributors. All rights reserved.")

//    outputter.OutputComment(
//        Comment "Licensed under the MIT license. See LICENSE file in the project root for full license information."
//    )

//    outputter.BlankLine()

//    outputter.OutputPragma(Pragma "warning disable")
//    ()
    
//let private methodSigFromCommandDef (commandDef: CommandDef) =
//    let isAction = 
//        match commandDef.ReturnType  with
//        | Void -> true
//        | _ -> false
//    let memberTypes = 
//        [ for memberDef in commandDef.Members do 
//            memberDef.TypeName
//            match commandDef.ReturnType with 
//            | Type t -> t
//            | Void -> () ]
//    let name = if isAction then "Action" else "Func"
//    GenericNamedItem (name, memberTypes)

//let baseMemberName (memberDef: MemberDef) = $"{memberDef.Name}{memberDef.KindName}"
//let memberFieldName (memberDef: MemberDef) = $"_{ToCamel (baseMemberName memberDef)}"
//let memberPropName (memberDef: MemberDef) = $"{ToPascal (baseMemberName memberDef)}"
//let memberParamName (memberDef: MemberDef) = ToCamel (baseMemberName memberDef)
//let memberSymbolType (memberDef: MemberDef) = GenericNamedItem (memberDef.KindName, [ memberDef.TypeName ])

//let commandFieldName (_: CommandDef)  ="_command"  
//let commandPropName (_: CommandDef) = "Command"
//let commandClassName (commandDef: CommandDef) = $"{ToPascal commandDef.Name}CommandWrapper"

//let generatedCommandHandlerName (_: CommandDef) = "GeneratedHandler"

//// TODO: Flesh out these
//let optionSpecificValues (memberDef:MemberDef) = []
//let argumentSpecificValues (memberDef:MemberDef) = []


let OutputCommandWrapper (commandDefs: CommandDef list) : Result <NamespaceModel, AppErrors> =


//    let propStatements (mbr:MemberDef) =

//        let symbolType =  memberSymbolType mbr 
//        let fieldName =  memberFieldName mbr 
//        let expression =
//            Instantiation
//                { TypeName = symbolType
//                  Arguments = [ StringLiteral mbr.Name] }
//        // TODO: Add other fields like Required
//        [ assign fieldName expression 
//          match mbr.Description with 
//          | Some desc ->
//                assign $"{fieldName}.Description" (StringLiteral desc)
//          | None -> ()

//          let specificStatements = 
//              match mbr.Kind with 
//              | Option -> optionSpecificValues mbr
//              | Argument -> argumentSpecificValues mbr
//              | Service -> []
//              // KAD: More here }
//          for statement in specificStatements do
//            statement
//        ]

          
//    let fieldFromMember (mbr: MemberDef) =
//        match mbr.Kind with 
//        | Service -> None
//        | Argument | Option  -> 
//            Some (field (memberSymbolType mbr ) (memberFieldName mbr ) Null)


//    let propFromMember (mbr: MemberDef) =
//        let symbolType =  memberSymbolType mbr 
//        let fieldName =  memberFieldName mbr 
//        let fieldNameSymbol = Symbol fieldName
//        let propertyName = memberPropName mbr 
//        let prop = 
//            prop Public symbolType propertyName
//                [ ifThen (ExpressionModel.Compare fieldNameSymbol Equals Null)
//                    (propStatements mbr)
//                  Return (Symbol fieldName)
//                ] []
//        // Do we actually use this?
//        mbr.AddToPocket "PropertyName" propertyName
           
//        match mbr.Kind with 
//        | Argument | Option -> (Some (prop ))
//        | Service -> None        

//    let commandProp (commandDef: CommandDef) = 
//        let commandFieldName = commandFieldName commandDef
//        let commandFieldSymbol = Symbol commandFieldName
//        let commandHandlerName = $"{commandFieldName}.Handler"
//        let handlerClass = generatedCommandHandlerName commandDef
//        let memberFieldSymbol memberDef = Symbol (memberFieldName memberDef)

//        [ ifThen (ExpressionModel.Compare commandFieldSymbol Equals Null)
//            [
//                // Should the following be name or first alias?
//                assign commandFieldName (New "Command" [ StringLiteral commandDef.Name ])

//                for memberDef in commandDef.Members do
//                    StatementModel.Invoke commandFieldName (SimpleNamedItem "Add") [ memberFieldSymbol memberDef ]
 
//                assign commandHandlerName 
//                    (New handlerClass 
//                        [Symbol operationFieldName
//                         for memberDef in commandDef.Members do 
//                            Symbol (memberPropName memberDef)])
//            ]
//          Return commandFieldSymbol
        
//        ]

//    let generatedHandler (commandDef: CommandDef) = 
//        let methodSig = methodSigFromCommandDef commandDef
//        let invokeMethodName = SimpleNamedItem "InvokeAsync"
//        let invokeReturnType = GenericNamedItem ("Task", [SimpleNamedItem "int"] )
//        let getValueForMember (memberDef: MemberDef) =
//            Invoke 
//                "context.ParseResult" 
//                (GenericNamedItem ($"GetValueFor{memberDef.KindName}", [memberDef.TypeName]))
//                [Symbol (memberFieldName memberDef)]
            
//        [
//          ctor Public (generatedCommandHandlerName commandDef)
//            [ param operationName methodSig
//              for memberDef in commandDef.Members do
//                match memberDef.Kind with 
//                | Option | Argument -> param (memberParamName memberDef) (memberSymbolType memberDef)
//                | Service -> () ]
//            [ assign $"{operationFieldName}" (Symbol operationName)
//              for memberDef in commandDef.Members do 
//                assign (memberFieldName memberDef) (Symbol (memberParamName memberDef)) ]

//          readonlyField methodSig operationFieldName
//          for memberDef in commandDef.Members do
//            readonlyField (memberSymbolType memberDef) (memberFieldName memberDef)

//          method Public (Type invokeReturnType) invokeMethodName 
//            [ param "context" (SimpleNamedItem "InvocationContext")]
//            [ StatementModel.Invoke operationFieldName (SimpleNamedItem "Invoke") 
//                [ for memberDef in commandDef.Members do 
//                    getValueForMember memberDef]
//              Return (invokeAwait "Task" "FromResult" [Symbol "context.ExitCode"]) ]
//        ]

//    //let commandDefClass (commandDef: CommandDef) = 
//    //    let methodSig = methodSigFromCommandDef commandDef
//    //    [ readonlyField methodSig operationFieldName
//    //      field (SimpleNamedItem "Command") (commandFieldName commandDef) Null
          
//    //      for mbr in commandDef.OptionsAndArgs do
//    //        match fieldFromMember mbr with
//    //        | None -> ()
//    //        | Some field -> field

//    //      ctor Public (commandClassName commandDef)
//    //        [ param operationName methodSig]
//    //        [ assign $"_{operationName}" (Symbol operationName)]

//    //      for mbr in commandDef.Members do
//    //        match propFromMember mbr with
//    //        | None -> ()
//    //        | Some prop -> prop

//    //      prop Public (SimpleNamedItem "Command") (commandPropName commandDef)
//    //        (commandProp commandDef)
//    //        []

//    //      let handlerClassName = generatedCommandHandlerName commandDef
//    //      let commandHandlerInterface = SimpleNamedItem "ICommandHandler"
//    //      clsWithInterfaces Private handlerClassName [commandHandlerInterface] (generatedHandler commandDef)
    
            
//    //    ]

//    //let classForCommandDef (commandDef: CommandDef) =
//    //    ClassModel.Create 
//    //        ( SimpleNamedItem (commandClassName commandDef),
//    //          Public,
//    //          commandDefClass commandDef)

//    let classForCommandDef (commandDef: CommandDef) = ()


//    let classes = 
//        [ for commandDef in commandDefs do
//            classForCommandDef commandDef
//        ]

    try
        Error (Other "Rewriting")
        // //KAD: Figure out right namespace
        //Ok ({ NamespaceName = "GeneratedHandlers"
        //      Usings = 
        //        [ UsingModel.Create "System" 
        //          UsingModel.Create "System.CommandLine"
        //          UsingModel.Create "System.CommandLine.Invocation"
        //          UsingModel.Create "System.Threading.Tasks"]
        //      Classes = classes })
    with
    | ex -> Error (Other $"Error creating code model {ex.Message}")

