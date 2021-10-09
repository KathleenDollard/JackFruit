module Generaor.OutputMapping

open Generator.Language
open Generator.Models

let OutputCode (outputter: RoslynWriter) (root: CommandDef)=
    let code = 
        { Namespace.Name = "Foo"
          Usings = 
            [ { Namespace = "System"; Alias = None }; 
              { Namespace = "System.CommandLine"; Alias = None }]
          Classes = []}
    ()

