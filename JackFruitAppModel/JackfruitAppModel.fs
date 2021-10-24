namespace Jackfruit

open Generator.RoslynUtils
open Generator.GeneralUtils
open Jackfruit.Models
open Generator

type AppModel() =

    let GetCommandArchetype parts =
        let commandArchetpes =
             [for part in parts do
                match part with  
                | CommandArchetype c -> c
                | _ -> ()]
        commandArchetpes |> List.exactlyOne
    
    interface IAppModel<TreeNodeType<ArchetypeInfo>> with 
        
        member _.Children archTree =
            archTree.Children
        
        // Id, method, stuff for pocket
        member _.Info model archTree =
            let archetypeInfo = archTree.Data
            let method = 
                match archetypeInfo.Handler with
                | None -> None
                | Some handler -> MethodFromHandler model handler
            let commandArchetype = GetCommandArchetype archetypeInfo.ArchetypeParts
            commandArchetype.Id, method, ["ArchetypeInfo", archetypeInfo]
        
        member _.RunProviders commandDef =
            commandDef



