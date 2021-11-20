module Generator.OutputCode

open Generator.Language
open Generator.LanguageExpression
open Models

let private OutputHeader (outputter: RoslynOut) =
    outputter.OutputComment(Comment "Copyright (c) .NET Foundation and contributors. All rights reserved.")

    outputter.OutputComment(
        Comment "Licensed under the MIT license. See LICENSE file in the project root for full license information."
    )

    outputter.BlankLine()

    outputter.OutputPragma(Pragma "warning disable")
    ()

//let private CommonMembersForUserMethod pos commandDef method : Method =
//    let mutable i = 0

//    Public.StaticMethod 
//    [ for mbr in commandDef.Members
//        i <- i + 1

//    ]


let private CommonMembers pos (commandDefs: CommandDef list) : Member list =
    let mutable i = pos
    // TODO: We need to get a distinct on the parameters as these are shared later
    [ for commandDef in commandDefs do
          let parameters =
              [ Parameter.Create "command" (SimpleNamedItem "Command")
                //Parameter.Create "method" NamedItem.Create commandDef.
           
                for mbr in commandDef.Members do 
                    Parameter.Create mbr.Name (NamedItem.Create mbr.TypeName [])
              ]

          let statements = []
   
          // ***** SetHandler will go away :) *****
          // Which of the following is nicest? (Combination suggestions are welcome)
          let generics = [1..parameters.Length] |> List.map (fun x -> SimpleNamedItem $"T{x}") 
          let setHandler = NamedItem.Create "SetHandler" generics
          method Public Static Void setHandler parameters statements

          //methodStatic Public Void setHandler parameters statements

          //method Public Void setHandler parameters statements


          let generics = [1..parameters.Length] |> List.map (fun x -> SimpleNamedItem $"T{x}") 
          PublicStaticMethodOf Void "SetHandler" generics parameters statements

          let generics = [1..parameters.Length] |> List.map (fun x -> $"T{x}") 
          Public.StaticMethodOf Void "SetHandler" generics parameters statements

          // Picking the last approach to play with for other items
         // Private.Class GenerateHandler_{pos}


    ]

   
let private TypeForSymbol symbolName typeName =
    { Name = symbolName 
      GenericTypes = [ { Name = typeName; GenericTypes = []} ]}
    

//public static void SetHandler<{string.Join(", ", Enumerable.Range(1, invocation.NumberOfGenerericParameters).Select(x => $@"T{x}"))}>(
//    this Command command,");
//        builder.Append($@"
//    {invocation.DelegateType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} method");

//        if (methodParameters.Length > 0)
//        {
//            builder.Append(",");
//            builder.AppendLine(string.Join(", ", methodParameters.Select(x => $@"
//    {x.Type} {x.Name}")) + ")");
//        }
//        else
//        {
//            builder.Append(")");
//        }



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
