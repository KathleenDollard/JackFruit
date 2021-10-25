
module JackFruitAppModel.Generator

    open Generator.RoslynUtils
    open Microsoft.CodeAnalysis
    open Jackfruit.ArchetypeMapping
    open Jackfruit
    open Generator.NewMapping

    let private mapMethodName = "MapInferred"
    let private appModel =  AppModel()

    let BuildModel (model:SemanticModel) = 


        InvocationsFromModel mapMethodName model
        |> Result.bind ArchetypeInfoListFrom
        |> Result.map ArchetypeInfoTreeFrom
        |> Result.map (CommandDefsFrom model appModel)
        // Find Archetypes and build tree
        // Use Generator to find CommandDef info for handlers
        // Run archetype provider
        // Run XmlComments provider
        // Run path provider
