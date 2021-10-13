module Generator.OutputCode

open Language
open Models

let private OutputHeader (outputter: RoslynOut) = 
    outputter.OutputComment (Comment "Copyright (c) .NET Foundation and contributors. All rights reserved.")
    outputter.OutputComment (Comment "Licensed under the MIT license. See LICENSE file in the project root for full license information.")
    outputter.BlankLine()

    outputter.OutputPragma (Pragma "warning disable")
    ()

let private CommonMembers pos commandDefs : Member list =
    // TODO: We need to get a distinct on the parameters as these are shared later
    [ for commandDef in commandDefs do
        // TODO: Figure out the scenario where there are multiple generic types
        Method 
            { Name = { Name = "SetHandler"; GenericTypes = [ GenericNamedItem.Create "T1"] }
              ReturnType = None
              IsStatic = true
              IsExtension = true
              Scope = Public
              Parameters = []
              Statements = [] }

    ]


let private CommandMembers commandDefs =
    []
    //[ for (commandDef: CommandDef) in commandDefs do
    //     match commandDef.Arg with 
    //         Some arg -> 
    //             { VariableName = commandDef.Arg.
    //               TypeName = None
    //               Value = Instantiation     
    //                 { TypeName = GenericNamedItem "Argument" (GenericNamedItem "string")
    //                   Arguments = [] }}
    //         None -> () ]
    

let private OutputCommandCode commandDefs =
    { Name = GenericNamedItem.Create "GeneratedCommandHandlers"
      IsStatic = true
      Scope = Internal
      Members = CommandMembers commandDefs }


let NamespaceFrom commandDefs includeCommandCode =
    let commonCode = 
        // TODO: There are some important attributes to add
        { Name = GenericNamedItem.Create "GeneratedCommandHandlers"
          IsStatic = true
          Scope = Internal
          Members = CommonMembers 1 commandDefs }

    { Name = "System.CommandLine"
      Usings = 
        [ Using.Create "System.ComponentModel"
          Using.Create "System.CommandLine.Binding"
          Using.Create "System.Reflection"
          Using.Create "System.Threading.Tasks"
          Using.Create "System.CommandLine.Invocation;" ]
      Classes = 
        [ if includeCommandCode then OutputCommandCode commandDefs 
          commonCode ]}
            

let CodeFrom includeCommandCode (language: ILanguage) writer commandDefs =
    let outputter = RoslynOut(LanguageCSharp(),writer)
    let nspace = NamespaceFrom commandDefs includeCommandCode

    OutputHeader outputter
    outputter.Output nspace



    writer.Output
