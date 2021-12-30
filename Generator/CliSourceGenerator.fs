module Generator.SourceGenerator

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.VisualBasic
open Generator
open Generator.NewMapping
open Generator.BuildCliCodeModel
open Generator.Transforms
open System.Linq

let GenerateFromAppModel<'T> (appModel: AppModel<'T>) language semanticModel (outputter =
    appModel.Initialize(semanticModel)                           // Passes back 'T
    |> Result.bind (CommandDefsFrom semanticModel appModel)      // Passes back CommandDefs shape
    |> Result.bind (ApplyTransformsToMany appModel.Transformers) // Passes back transformed CommandDefs
    |> Result.bind OutputCommandWrapper                          // Passes back CodeModel/Namespace
    |> Result.map (outputter.Output codeModel)                  // Passes back list of strings



[<Generator>]
type CliSourceGenerator<'T>() =
    abstract member GetAppModel: synataxTrees: SyntaxTree list -> semanticModel : SemanticModel -> AppModel<'T>

    interface ISourceGenerator with

        member _.Initialize(context) =
            ()

        member this.Execute(context) =
            let syntaxTrees = List.ofSeq context.Compilation.SyntaxTrees
            let semanticModel = context.Compilation.GetSemanticModel(syntaxTrees.First())
            let language: ILanguage = match context.Compilation with
                | :? CSharpCompilation -> LanguageCSharp()
                | :? VisualBasicCompilation -> LanguageVisualBasic()
                | _ -> invalidOp "Unexpected language encountered (not C# or VB)"

            let appModel = this.GetAppModel syntaxTrees semanticModel

            let modelResult = GenerateFromAppModel appModel language semanticModel
            match modelResult with 
            | Error e -> invalidOp "Oops, we need to write error handling" 
                // We probably have added diagnostics and should here. But this Result is currently AppError and 
                // should probably be Diagnostic so more info can be carried
            | Ok output -> 
                context.AddSource("myGeneratedFile.cs", output)
            ()
