module Generaor.OutputMapping

open Generator.Language
open Generator.Models

let OutputCode (outputter: RoslynOut) (root: CommandDef)=
    let code = 
        { Namespace.NamespaceName = "Foo"
          Usings = 
            [ { Namespace = "System"; Alias = None }; 
              { Namespace = "System.CommandLine"; Alias = None }]
          Classes = []}
    ()

