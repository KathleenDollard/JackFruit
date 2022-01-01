module Generator.SourceGenerator

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.VisualBasic
open Generator
open Generator.NewMapping
open Generator.BuildCliCodeModel
open Generator.Transforms
open System.Linq
open LanguageRoslynOut

let GenerateFromAppModel<'T> (appModel: AppModel<'T>) language semanticModel (writer: IWriter) =
    let outputter = RoslynOut(language, writer)
    let x = 
        appModel.Initialize(semanticModel)                           // Passes back 'T
        |> Result.bind (CommandDefsFrom semanticModel appModel)      // Passes back CommandDefs shape
        |> Result.bind (ApplyTransformsToMany appModel.Transformers) // Passes back transformed CommandDefs
        |> Result.bind OutputCommandWrapper                          // Passes back CodeModel/Namespace
        //|> Result.map ()                   // Passes back a writer, such as an ArrayWriter or StringBuilderWriter
    match x with 
    | Error e -> invalidOp "Oops"
    | Ok codeModel -> outputter.Output codeModel


[<AbstractClass>]
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

            let modelResult = GenerateFromAppModel appModel language semanticModel (StringBuilderWriter 3)
            match modelResult with 
            | Error e -> invalidOp "Oops, we need to write error handling" 
                // We probably have added diagnostics and should here. But this Result is currently AppError and 
                // should probably be Diagnostic so more info can be carried
            | Ok writer -> 
                context.AddSource("myGeneratedFile.cs", writer.Output)
            ()
