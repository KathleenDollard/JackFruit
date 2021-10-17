module Generator.CommandDefMapping

open Generator.GeneralUtils
open Generator.Models
open RoslynUtils
open Microsoft.CodeAnalysis
open System

// TODO: More work is needed because there is more we can get from the parameter
// But it was being difficult and I wanted to get the main logic done. This will 
// need to switch to a record or similar to carry the parameter information. 
let ParametersFromArchetype (archetypeInfo: ArchetypeInfo) model =
    match archetypeInfo.Handler with 
    | Some handler -> 
        match MethodFromHandler model handler with 
        | Some method -> 
            [ for parameter in method.Parameters do
                parameter.Name, 
                parameter.Type.ToDisplayString() ]
        | None -> invalidOp "Test failed because specified method not found"
    | None -> [] // Interim subcommands do not have handlers


let private getMemberDef part id typeName : MemberDef =
    // TODO: Get values from part
    MemberDef.Create id typeName


let getCommandDef parts archInfo members subCommands=
    let commandArchetpes =
         [for part in parts do
            match part with  
            | CommandArchetype c -> c
            | _ -> ()]
    let commandArch = commandArchetpes |> List.exactlyOne
    let desc = None

    { CommandId = commandArch.Id 
      Path = archInfo.Path
      Description = desc
      Aliases = [commandArch.Name] 
      Members = members
      SubCommands = subCommands}

let KnownService = ["IConsole"]

let private ArgOptions (parameters: (string * string) list) (parts: ArchetypePart list) :  MemberDef option list = 
    let lookup = 
        [ for part in parts do
            match part with 
            | ArgArchetype x
            | OptionArchetype x -> (ToCamel x.Id, part)
            | _ -> () ]
        |> Map.ofList
    [ for (id, typeName) in parameters do 
        let part = lookup.TryFind id
        match part with
        | Some a ->
            match a with 
            | ArgArchetype arg -> getArgDef (arg, id, typeName)
            | OptionArchetype option -> getOptionDef (option, id, typeName)
            | _ -> invalidOp "There should be no other archetype types"
        | None -> 
            if KnownServices.Contains typeName then
                { ServiceId = id
                  TypeName = typeName }
            else 
                None, id, typeName
    ]


    // KAD-Don: Any shortcuts to this pattern?
    let isArg (part: ArchetypePart option, _, _) =
        match part with 
        | Some p ->
            match p with 
            | ArgArchetype a -> true
            | _ -> false
        | None -> false

    // KAD-Don: Is there a way I could have designed ArchetypeParts so that I would not have to match here


    let pairWithPart (id: string, typeName: string) =
         let x = (lookup.TryFind id), id, typeName
         x

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
        let (argOptions) = ArgOptions parameters archetypeInfo.ArchetypeParts
        
        getCommandDef archetypeInfo.ArchetypeParts archetypeInfo argOptions subCommands

    [ for topLevelArch in archTree do
        depthFirstCreate topLevelArch ]
