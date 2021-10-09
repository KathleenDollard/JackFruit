module Generaor.OutputMapping

open Generator.Roslyn
open Generator.Models

let OutputCode (outputter: Output) (root: CommandDef)=
    let code = 
        { Namespace.Name = NamedItem "Foo"
          Usings = 
            [ { Namespace = "System"; Alias = None }; 
              { Namespace = "System.CommandLine"; Alias = None }]
          Classes = []}
    ()

