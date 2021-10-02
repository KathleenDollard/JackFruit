module Generator.CommandDefMapping

open Generator.GeneralUtils
open Generator.Models
open RoslynUtils
open Microsoft.CodeAnalysis

// TODO: More work is needed because there is more we can get from the parameter
// But it was being difficult and I wanted to get the main logic done. This will 
// need to switch to a record or similar to carry the parameter information. 
let ParametersFromArchetype archetypeInfo model =
    match archetypeInfo.HandlerExpression with 
    | Some handler -> 
        match MethodFromHandler model handler with 
        | Some method -> 
            [ for parameter in method.Parameters do
                parameter.Name, 
                parameter.Type.ToDisplayString() ]
        | None -> invalidOp "Test failed because specified method not found"
    | None -> [] // Interim subcommands do not have handlers


let getKey (part: string) = 
    let aliases = part.Trim().Split("|")
    let first = aliases.[0]
    first.Replace("-","")
        .Replace("<","")
        .Replace(">","")
        .Replace("[","")
        .Replace("]","")
        |> ToCamel


let getArgDef (part, id, typeName) =
    let name = id
    let desc = None
    let required = None
    let typeName = typeName

    { ArgId = id 
      Name = name
      Description = desc
      Required = required
      TypeName = typeName }


let getOptionDef (part, id, typeName) =
    let name = id
    let desc = None
    let aliases = []
    let required = None
    let typeName = typeName

    { OptionId = id 
      Name = name
      Description = desc
      Aliases = aliases
      Required = required
      TypeName = typeName }


let getCommandDef part id arg options subCommands=
    let name = id
    let desc = None
    let aliases = []

    { CommandId = id 
      Name = name
      Description = desc
      Aliases = aliases 
      Arg = arg
      Options = options
      SubCommands = subCommands}


let argAndOptions (parameters: (string * string) list) parts = 

    let lookup = 
        [ for part in parts do
            (getKey part, part)]
        |> Map.ofList

    let isArg (part, id, typeName) =
        match part with 
        | Some p ->
            match p with 
            | Arg a -> true
            | _ -> false
        | _ -> false

    let pairWithPart (id: string, typeName: string) =
         lookup.TryFind id, id, typeName

    let args, options = 
        parameters 
        |> List.map pairWithPart
        |> List.partition isArg

    let argDefs = args |>  List.map getArgDef
    // KAD: tryHead is documented as returning an Option, but it is returning null when list is empty
    // let argDef = argDefs |> List.tryHead
    let argDef: ArgDef option = 
        match argDefs with 
        | [] -> None
        | head::_ -> Some head
    let optionDefs = options |> List.map getOptionDef

    argDef, optionDefs


let CommandDefFrom model archTree =

    let rec depthFirstCreate archTree  =
        let subCommands = 
            [ for child in archTree.Children do
                yield depthFirstCreate child ]

        let archetypeInfo = archTree.Data
        let parameters = ParametersFromArchetype archetypeInfo model
        let (arg, options) = argAndOptions parameters archetypeInfo.Raw
        
        let id = archetypeInfo.AncestorsAndThis.[archTree.Data.AncestorsAndThis.Length-1]
        let name = id // TODO: Pass this as default to providers, same for remainder except Options/Arg

        getCommandDef archetypeInfo.Raw id arg options subCommands

    [ for topLevelArch in archTree do
        depthFirstCreate topLevelArch ]
