module Generator.NewMapping

open Microsoft.CodeAnalysis
open Generator.Models

let CommandDefFromMethod model (method: IMethodSymbol option) id =

    let members = 
        match method with
             | Some method -> 
                 [ for parameter in method.Parameters do
                     // KAD-Don: Why does this error without parens? What is "Function application"?
                     MemberDef.Create parameter.Name (parameter.Type.ToDisplayString()) ]
             | None -> [] 

    { CommandId = id
      Path = []
      Description = None
      Aliases = [] 
      Members = members
      SubCommands = []
      Pocket = 
      [ "Method", method 
        "SemanticModel", model] }


let CommandDefFrom<'T> model (appModel: IAppModel<'T>) (items: 'T list)  =

    let rec depthFirstCreate item  =
        let subCommands = 
            [ for child in (appModel.Children item) do
                yield depthFirstCreate child ]
        let (id, method, forPocket) = appModel.Info model item
        let commandDef = CommandDefFromMethod model method id
        appModel.RunProviders commandDef

    [ for item in items do
        depthFirstCreate item ]