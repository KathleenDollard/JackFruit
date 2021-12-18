﻿module Generator.SourceGenerator

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.VisualBasic
open Generator
open Generator.NewMapping
open Generator.BuildCodePattern
open Generator.Transforms

let GenerateFromAppModel<'T> (appModel: AppModel<'T>) language semanticModel =
    appModel.Initialize(semanticModel)                           //Passes 'T
    |> Result.bind (CommandDefsFrom semanticModel appModel)      // Passes CommandDefs shape
    |> Result.bind (ApplyTransformsToMany appModel.Transformers) // Passes transformed CommandDefs
    |> Result.bind OutputCommandWrapper                          // Passes CodeModel/Namespace



//[<Generator>]
//type CliSourceGenerator() =
//    interface ISourceGenerator with
//        member _.Initialize(context) =
//            ()

//        member _.Execute(context) =
//            let language = match context.Compilation with
//                | :? CSharpCompilation -> LanguageCSharp()
//                | :? VisualBasicCompilation -> LanguageVisualBasic()
//                | _ -> invalidOp "Unexpected language encountered (not C# or VB)"
//            let output = GenerateFromAppModel appModel 
//            context.AddSource("myGeneratedFile.cs", output)
//            ()