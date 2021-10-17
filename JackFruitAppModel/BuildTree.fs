module JackfruitAppModel.CommandDefMapping

open Generator.RoslynUtils
open Generator.GeneralUtils
open Generator.Models

let private GetCommandArchetype parts =
    let commandArchetpes =
         [for part in parts do
            match part with  
            | CommandArchetype c -> c
            | _ -> ()]
    commandArchetpes |> List.exactlyOne

let private MethodAndMembers model archetypeInfo =
    let method = 
        match archetypeInfo.Handler with
        | None -> None
        | Some handler -> MethodFromHandler model handler
    let members =
        match method with
        | Some method -> 
            [ for parameter in method.Parameters do
                // KAD-Don: Why does this error without parens? What is "Function application"?
                MemberDef.Create parameter.Name (parameter.Type.ToDisplayString()) ]
        | None -> [] 
    method, members

let CommandDefFrom model (archTree: seq<TreeNodeType<ArchetypeInfo>>) =

    let rec depthFirstCreate archTree  =
        let subCommands = 
            [ for child in archTree.Children do
                yield depthFirstCreate child ]

        let archetypeInfo = archTree.Data
        let method, members = MethodAndMembers model archetypeInfo
        let commandArchetype = GetCommandArchetype archetypeInfo.ArchetypeParts

        { CommandId = commandArchetype.Id 
          Path = archetypeInfo.Path
          Description = None
          Aliases = [commandArchetype.Name] 
          Members = members
          SubCommands = subCommands
          Pocket = 
            [ "Method", method
              "ArchetypeInfo", archetypeInfo]}
        
        // GetCommandDef archetypeInfo.ArchetypeParts archetypeInfo members subCommands

    [ for topLevelArch in archTree do
        depthFirstCreate topLevelArch ]
