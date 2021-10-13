module Generator.OutputCode

open Language
open Models

let private OutputHeader (outputter: RoslynOut) =
    outputter.OutputComment(Comment "Copyright (c) .NET Foundation and contributors. All rights reserved.")

    outputter.OutputComment(
        Comment "Licensed under the MIT license. See LICENSE file in the project root for full license information."
    )

    outputter.BlankLine()

    outputter.OutputPragma(Pragma "warning disable")
    ()

let private CommonMembers pos commandDefs : Member list =
    let mutable i = pos
    // TODO: We need to get a distinct on the parameters as these are shared later
    [ for commandDef in commandDefs do
          let delegateParameter =
              Parameter.Create "Temp" (GenericNamedItem.Create "Temp")

          let parameters =
              [ Parameter.Create "command" (GenericNamedItem.Create "Command")
                delegateParameter
                match commandDef.Arg with
                | Some arg ->
                    Parameter.Create
                        arg.Name
                        { Name = "Argument"
                          GenericTypes =
                            [ { Name = arg.TypeName
                                GenericTypes = [] } ] }
                | None -> ()
                //for opt in options do

                ]
          // TODO: Figure out the scenario where there are multiple generic types
          Method
              { Name =
                  { Name = "SetHandler"
                    GenericTypes = [ GenericNamedItem.Create "T1" ] }
                ReturnType = None
                IsStatic = true
                IsExtension = true
                Scope = Public
                Parameters = parameters
                Statements = [] }

          Class
              { Name =
                  { Name = "GeneratedHandler"
                    GenericTypes = [] }
                IsStatic = false
                Scope = Private
                Members = [] } ]


let private CommandMembers (commandDefs: CommandDef list) : Member list = []
    //[ for (commandDef: CommandDef) in commandDefs do
    //    // TODO: Change to path and camel
    //    { Name = GenericNamedItem.Create $"{CommandDef.Name}Symbols"
    //      } 

    //     match commandDef.Arg with
    //         Some arg ->
    //             { VariableName = commandDef.Arg.
    //               TypeName = None
    //               Value = Instantiation
    //                 { TypeName = GenericNamedItem "Argument" (GenericNamedItem "string")
    //                   Arguments = [] }}
    //         None -> () ]


//let private OutputCommandCode commandDefs =
//    { Name = GenericNamedItem.Create "GeneratedCommandHandlers"
//      IsStatic = true
//      Scope = Internal
//      Members = CommandMembers commandDefs }


//let NamespaceFrom commandDefs includeCommandCode =
//    let commonCode =
//        // TODO: There are some important attributes to add
//        { Name = GenericNamedItem.Create "GeneratedCommandHandlers"
//          IsStatic = true
//          Scope = Internal
//          Members = CommonMembers 1 commandDefs }

//    { Name = "System.CommandLine"
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
