namespace Jackfruit

open Generator.RoslynUtils
open Generator.GeneralUtils
open Jackfruit.Models
open Generator
open System
open Microsoft.CodeAnalysis


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
            let commandArchetype = GetCommandArchetype archetypeInfo.ArchetypeParts
            let method = 
                match archetypeInfo.Handler with
                | None -> None
                | Some handler -> MethodSymbolFromMethodCall model handler
            let id = 
                if String.IsNullOrEmpty(commandArchetype.Id) then 
                    None
                else 
                    Some commandArchetype.Id
            { InfoCommandId = id 
              Path = archetypeInfo.Path 
              Method = method 
              ForPocket = ["ArchetypeInfo", archetypeInfo] }
        
        member _.CommandDefTransformers = []            

        member _.MemberDefTransformers = []

   