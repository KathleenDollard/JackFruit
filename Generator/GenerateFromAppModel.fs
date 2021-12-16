module GenerateFromAppModel

open Generator
open Generator.NewMapping
open Generator.BuildCodePattern

//let GenerateFromAppModel<'T> (appModel: AppModel<'T>) language semanticModel =
    //try //  Move this into each step, then at least we can tell the user the step that failed
        //appModel.Initialize(semanticModel)                     //Passes 'T
        //|> Result.map (CommandDefsFrom semanticModel appModel) // Passes CommandDefs shape
        //|> Result.map AppModel.RunTransformers                 // Passes transformed CommandDefs
        //|> Result.map OutputCommandWrapper                     // Passes CodeModel

 // bind might have issues. Map shoudl be bullet proof.
    

