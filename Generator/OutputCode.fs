module Generator.OutputCode

open Language
open Models
open Common
open LanguageExpression

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

let OutputCommandWrapper (commandDefs: CommandDef list) : Namespace =
    
    let methodSigFromCommandDef (commandDef: CommandDef) =
        let isAction = 
            match commandDef.ReturnType  with
            | Void -> true
            | _ -> false
        let memberTypes = 
            [ for mbr in commandDef.Members do 
                mbr.TypeName
              match commandDef.ReturnType with 
                | Type t -> t
                | Void -> () ]
        let name = if isAction then "Action" else "Func"
        GenericNamedItem (name, memberTypes)

    let kind (mbr: MemberDef) =
        match mbr.MemberKind with
        | Some value -> value
        | None -> Option   

    let kindName (mbr: MemberDef)  =
        let kind = 
            match mbr.MemberKind with
            | Some value -> value
            | None -> Option   
        match kind  with 
        | Argument -> "Option" 
        | Option -> "Argument" 
        | Service -> ""        

        
    let memberFieldName (mbr: MemberDef) =
        $"_{mbr.Name}{kindName mbr}"  // KAD: Check casing

    let memberPropName (mbr: MemberDef) =
        $"{mbr.Name}{kindName mbr}"  // KAD: Check casing

    let memberParamName (mbr: MemberDef) =
        $"{mbr.Name}{kindName mbr}"  // KAD: Check casing

    let commandFieldName (_: CommandDef) =
        "_command"  
  
    let commandPropName (_: CommandDef)  =
        "Command" 

    let generatedCommandHandlerName (_: CommandDef)  =
        "GeneratedHandler"


    let symbolType (mbr: MemberDef) =
        let kindName = kindName mbr
        GenericNamedItem (kindName, [ mbr.TypeName ])

    let optionSpecificValues (mbr:MemberDef) = []

    let argumentSpecificValues (mbr:MemberDef) = []

    let propStatements (mbr:MemberDef) =

        let symbolType =  symbolType mbr 
        let fieldName =  memberFieldName mbr 
        let expression =
            Instantiation
                { TypeName = symbolType
                  Arguments = [ StringLiteral mbr.Name] }
        [ Statement.Assign { Item = fieldName; Value = expression }
          match mbr.Description with 
          | Some desc ->
                Statement.Assign { Item = $"{fieldName}.Description"; Value = StringLiteral desc }
          | None -> ()

          let specificStatements = 
              match kind mbr with 
              | Option -> optionSpecificValues mbr
              | Argument -> argumentSpecificValues mbr
              | Service -> []
              // KAD: More here }
          for statement in specificStatements do
            statement
        ]

          
    let fieldFromMember (mbr: MemberDef) =
        match kind  mbr with 
        | Service -> None
        | Argument | Option  -> 
            Some (field (symbolType mbr ) (memberFieldName mbr ) Null)


    let propFromMember (mbr: MemberDef) =
        let symbolType =  symbolType mbr 
        let fieldName =  memberFieldName mbr 
        let propertyName = memberPropName mbr 
        let prop = 
            prop Public symbolType propertyName
                [ ifThen (Expression.Comparison {Left = Symbol fieldName; Right = Null; Operator = Operator.Equals})
                    (propStatements mbr)
                  Return (Symbol fieldName)
                ] []
        mbr.AddToPocket "PropertyName" propertyName
           
        match kind  mbr with 
        | Argument | Option -> (Some (prop ))
        | Service -> None        

    let commandProp (commandDef: CommandDef) = 
        // Check if it already exists 
        // Add each member property
        // Set commandHandler - be sure to include service properties here
        // return command
        let fieldName = commandFieldName commandDef
        let fieldNameSymbol = Symbol fieldName
        let commandHandler = $"{fieldName}.Handler"
        let handlerClass = generatedCommandHandlerName commandDef
        let memberFieldSymbol memberDef = Symbol (memberFieldName memberDef)

        [ ifThen (Expression.Compare fieldNameSymbol Equals Null)
            [
                // Should the following be name or first alias?
                Assign fieldName (New "Command" [ StringLiteral commandDef.Name ])

                for memberDef in commandDef.Members do
                    Statement.Invoke fieldName (SimpleNamedItem "Add") [ memberFieldSymbol memberDef ]
 
                Assign commandHandler 
                    (New handlerClass 
                        [Symbol "_method"
                         for memberDef in commandDef.Members do 
                            Symbol (memberPropName memberDef)])
            ]
          Return fieldNameSymbol
        
        ]

    let generatedHandler (commandDef: CommandDef) = 
        let methodSig = methodSigFromCommandDef commandDef
        let invokeMethodName = SimpleNamedItem "Invoke"
        let getValueForMember (memberDef: MemberDef) =
            Invoke 
                "context.ParseResult" 
                (GenericNamedItem ("GetValueForOption", [memberDef.TypeName]))
                [Symbol (memberFieldName memberDef)]
            
        [
          ctor Public 
            [ param operationName methodSig
              for memberDef in commandDef.Members do
                match kind memberDef with 
                | Option | Argument -> param (memberParamName memberDef) (symbolType memberDef)
                | Service -> () ]
            [ assign $"{operationFieldName}" (Symbol operationName)
              for memberDef in commandDef.Members do 
                assign (memberFieldName memberDef) (Symbol (memberParamName memberDef)) ]

          readonlyField methodSig operationFieldName
          for memberDef in commandDef.Members do
            readonlyField (symbolType memberDef) (memberFieldName memberDef)

          method Public commandDef.ReturnType invokeMethodName 
            [ param "context" (SimpleNamedItem "InvocationContext")]
            [ Return (Invoke operationName (SimpleNamedItem "Invoke") 
              [ for memberDef in commandDef.Members do 
                  getValueForMember memberDef] ) ]

        ]

    let commandDefClass commandDef = 
        let methodSig = methodSigFromCommandDef commandDef
        [ readonlyField methodSig $"_{operationName}"
          field (SimpleNamedItem "Command") (commandFieldName commandDef) Null
          
          for mbr in commandDef.Members do
            match fieldFromMember mbr with
            | None -> ()
            | Some field -> field

          ctor Public 
            [ param operationName methodSig]
            [ assign $"_{operationName}" (Symbol operationName)]

          for mbr in commandDef.Members do
            match propFromMember mbr with
            | None -> ()
            | Some prop -> prop

          prop Public (SimpleNamedItem "Command") (commandPropName commandDef)
            (commandProp commandDef)
            []

          // Nested generated handler class
          Member.Class
            
              { ClassName = SimpleNamedItem (generatedCommandHandlerName commandDef)
                StaticOrInstance = Instance
                Scope = Private
                Members = generatedHandler commandDef }
            
        ]

    let classForCommandDef (commandDef: CommandDef) =
        { ClassName = SimpleNamedItem $"{commandDef.Name}CommandWrapper"
          StaticOrInstance = Instance
          Scope = Public
          Members = commandDefClass commandDef }

    let classes = 
        [ for commandDef in commandDefs do
            classForCommandDef commandDef
        ]

    // KAD: Figure out right namespace
    { NamespaceName = "GeneratedHandlers"
      Usings = [ Using.Create "System.CommandLine" ]
      Classes = classes }


//let private SymbolProperties (commandDef: CommandDef) =
//    let optionProperty (mbr: MemberDef) =
//        { PropertyName = $"{mbr.Name}Option"
//          Type = 

//        }
//    let argumentProperty mbr =
//    [ for mbr in commandDef.Members do
//        let mbrKind = match mbr.MemberKind with | Some kind -> kind | None -> Option
//        match mbrKind with 
//        | Option -> optionProperty mbr
//        | Argument -> argumentProperty mbr
//        | Service -> () ]

//let private CommonMembers pos (commandDefs: CommandDef list) : Member list =
//    let mutable i = pos
//    // TODO: We need to get a distinct on the parameters as these are shared later
//    [ for commandDef in commandDefs do
//          let delegateParameter =
//              Parameter.Create "Temp" (NamedItem.Create "Temp" [])

//          let parameters =
//              [ Parameter.Create "command" (NamedItem.Create "Command" [])
//                delegateParameter
           
//                //for opt in options do
//                    //Parameter.Create
//                    //    arg.Name
//                    //    { Name = "Argument"
//                    //      GenericTypes =
//                    //        [ { Name = arg.TypeName
//                    //            GenericTypes = [] } ] }

//                ]
//          // TODO: Figure out the scenario where there are multiple generic types
//          Method
//              { MethodName = NamedItem.Create "SetHandler"  [ SimpleNamedItem "T1" ]
//                ReturnType = Void
//                StaticOrInstance = Instance
//                IsExtension = true
//                Scope = Public
//                Parameters = parameters
//                Statements = [] }

//          Class
//              { ClassName = NamedItem.Create "GeneratedHandler"  []
//                StaticOrInstance = Instance
//                Scope = Private
//                Members = [] } ]






//let private TypeForSymbol symbolName typeName =
//    { Name = symbolName 
//      GenericTypes = [ { Name = typeName; GenericTypes = []} ]}
    
//let private CommandCreateStatements (commandDef: CommandDef) : Statement list =
//    let Argument (arg: ArgDef) =
//        let name = $"{arg.ArgId}Argument"
//        let args = [ StringLiteral arg.Name ] // Add other fields
//        name, args
        
//    let Option (option: OptionDef) =
//        let name = $"{option.OptionId}Option"
//        let args = [ StringLiteral option.Name ] // Add Other fields
//        name, args

//    let ArgumentsForCommand =
//        [ StringLiteral commandDef.Name ] // Other fields

//    // This needs to be done as we go because it's essential the order be the same
//    let mutable handlerTypes = []
//    let addHandlerType typeName =
//        handlerTypes <- handlerTypes 
//        |> List.insertAt 
//            handlerTypes.Length { Name = typeName; GenericTypes = []}
 
//    [
//        AssignVar "command" (New "Command" ArgumentsForCommand)

//        match commandDef.Arg with 
//        | Some arg -> 
//            let name, args = Argument arg
//            AssignVar name (NewGeneric "Argument" arg.TypeName args)
//            SimpleCall (Invoke "command" "AddSymbol" [(Symbol name)])
//            addHandlerType arg.TypeName
            
//        | None -> ()

//        for option in commandDef.Options do
//            let name, args = Option option
//            AssignVar name (NewGeneric "Option" option.TypeName args)
//            SimpleCall (Invoke "command" "AddSymbol" [(Symbol name)])
//            addHandlerType option.TypeName
//    ]
    

//let private CommandMembers commandDefs : Member list = 
//    [ for (commandDef: CommandDef) in commandDefs do
//        // TODO: Change to path and camel
//        Method
//            { MethodName = GenericNamedItem.Create $"{commandDef.CommandId}Symbols"
//              ReturnType = Some { Name = "Command"; GenericTypes=[] } 
//              IsStatic = true
//              IsExtension = false
//              Scope = Public
//              Parameters = []
//              Statements = CommandCreateStatements commandDef } ]


//let private OutputCommandCode commandDefs =
//    { ClassName = GenericNamedItem.Create "GeneratedCommandHandlers"
//      IsStatic = true
//      Scope = Internal
//      Members = CommandMembers commandDefs }


//let NamespaceFrom commandDefs includeCommandCode =
//    let commonCode =
//        // TODO: There are some important attributes to add
//        { ClassName = GenericNamedItem.Create "GeneratedCommandHandlers"
//          IsStatic = true
//          Scope = Internal
//          Members = CommonMembers 1 commandDefs }

//    { NamespaceName = "System.CommandLine"
//      Usings =
//        [ Using.Create "System.ComponentModel"
//          Using.Create "System.CommandLine.Binding"
//          Using.Create "System.Reflection"
//          Using.Create "System.Threading.Tasks"
//          Using.Create "System.CommandLine.Invocation;" ]
//      Classes =
//        [ if includeCommandCode then
//              OutputCommandCode commandDefs
//          commonCode ] }


//let CodeFrom includeCommandCode (language: ILanguage) writer commandDefs =
//    let outputter = RoslynOut(LanguageCSharp(), writer)

//    let nspace =
//        NamespaceFrom commandDefs includeCommandCode

//    OutputHeader outputter
//    outputter.Output nspace



//    writer.Output
