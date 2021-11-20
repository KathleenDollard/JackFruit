module Generator.OutputCode

open Language
open Models
open Common

let private OutputHeader (outputter: RoslynOut) =
    outputter.OutputComment(Comment "Copyright (c) .NET Foundation and contributors. All rights reserved.")

    outputter.OutputComment(
        Comment "Licensed under the MIT license. See LICENSE file in the project root for full license information."
    )

    outputter.BlankLine()

    outputter.OutputPragma(Pragma "warning disable")
    ()

let OutputCommandWrapper (commandDefs: CommandDef list) : Namespace =
    
    let classForCommandDef commandDef =
        { ClassName = $"{commandDef.Name}Command"
          

    let classes = 
        [ for commandDef in commandDefs do
            classForCommanDef commandDef
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

let private CommonMembers pos (commandDefs: CommandDef list) : Member list =
    let mutable i = pos
    // TODO: We need to get a distinct on the parameters as these are shared later
    [ for commandDef in commandDefs do
          let delegateParameter =
              Parameter.Create "Temp" (NamedItem.Create "Temp" [])

          let parameters =
              [ Parameter.Create "command" (NamedItem.Create "Command" [])
                delegateParameter
           
                //for opt in options do
                    //Parameter.Create
                    //    arg.Name
                    //    { Name = "Argument"
                    //      GenericTypes =
                    //        [ { Name = arg.TypeName
                    //            GenericTypes = [] } ] }

                ]
          // TODO: Figure out the scenario where there are multiple generic types
          Method
              { MethodName = NamedItem.Create "SetHandler"  [ SimpleNamedItem "T1" ]
                ReturnType = None
                IsStatic = true
                IsExtension = true
                Scope = Public
                Parameters = parameters
                Statements = [] }

          Class
              { ClassName = NamedItem.Create "GeneratedHandler"  []
                IsStatic = false
                Scope = Private
                Members = [] } ]






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
