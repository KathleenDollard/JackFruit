
module JackFruitAppModel.Generator

open Generator.RoslynUtils
open Microsoft.CodeAnalysis

    let private mapMethodName = "MapInferred"

    let BuildModel (model:SemanticModel) = 

        let invocations = 
            InvocationsFromModel mapMethodName model
            |> Result.bind ArchetypeInfoListFrom
            |> Result.map ArchetypeInfoTreeFrom
            |> Result.map (CommandDefFrom model)
        // Find Archetypes and build tree
        // Use Generator to find CommandDef info for handlers
        // Run archetype provider
        // Run XmlComments provider
        // Run path provider
