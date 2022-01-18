namespace Jackfruit

open Generator.RoslynUtils
open Generator.GeneralUtils
open Jackfruit.Models
open Generator
open System
open Microsoft.CodeAnalysis
open Jackfruit.ArchetypeMapping
open Generator.NewMapping
open Common
open Generator.CodeEval


type AppModel(evalLanguage: EvalBase) =
    inherit AppModel<TreeNodeType<ArchetypeInfo>>() with 

    let mapMethodName = "MapInferred"
        
    let GetCommandArchetype parts =
        let commandArchetpes =
            [for part in parts do
                match part with  
                | CommandArchetype c -> c
                | _ -> ()]
        commandArchetpes |> List.exactlyOne

    override _.Initialize semanticModel =
        let invocations = evalLanguage.InvocationsFromModel mapMethodName semanticModel
        let getArchetypeList = ArchetypeInfoListFrom evalLanguage
        evalLanguage.InvocationsFromModel mapMethodName semanticModel
        |> Result.bind getArchetypeList
        |> Result.bind ArchetypeInfoTreeFrom
        
    override _.Children archTree =
        archTree.Children
        
    // Id, method, stuff for pocket
    override _.Info semanticModel archTree =
        let archetypeInfo = archTree.Data
        let commandArchetype = GetCommandArchetype archetypeInfo.ArchetypeParts
        let method = 
            match archetypeInfo.Handler with
            | None -> None
            | Some handler -> evalLanguage.MethodSymbolFromMethodCall semanticModel handler
        let id = 
            if String.IsNullOrEmpty(commandArchetype.Id) then 
                None
            else 
                Some commandArchetype.Id
        { InfoCommandId = id 
          Path = archetypeInfo.Path 
          Method = method 
          ForPocket = ["ArchetypeInfo", archetypeInfo] }
        
    //member _.CommandDefTransformers = []            

    //member _.MemberDefTransformers = []

   