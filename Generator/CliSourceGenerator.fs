module Generator.SourceGenerator

open Microsoft.CodeAnalysis
open Generator
open Generator.NewMapping
open Generator.BuildCliCodeModel
open Generator.Transforms
open System.Linq
open LanguageRoslynOut
open Models
open Generator.LanguageModel

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

    abstract member CodeModelBuilder: CommandDef list -> Result<NamespaceModel, AppErrors>
    default _.CodeModelBuilder commandDef =
        OutputCommandWrapper commandDef

    interface ISourceGenerator with

        member _.Initialize(context) =
            ()

        member this.Execute(context) =
            let syntaxTrees = List.ofSeq context.Compilation.SyntaxTrees
            let semanticModel = context.Compilation.GetSemanticModel(syntaxTrees.First())

            let language: ILanguage = 
                match context.Compilation.Language with
                | LanguageNames.CSharp -> LanguageCSharp()
                | LanguageNames.VisualBasic -> LanguageVisualBasic()

                | _ -> invalidOp "Unexpected language encountered (not C# or VB)"

            let appModel = this.GetAppModel syntaxTrees semanticModel

            let outputter = RoslynOut(language, (StringBuilderWriter 3))
            let modelResult = 
                appModel.Initialize(semanticModel)                           // Passes back 'T
                |> Result.bind (CommandDefsFrom semanticModel appModel)      // Passes back CommandDefs shape
                |> Result.bind (ApplyTransformsToMany appModel.Transformers) // Passes back transformed CommandDefs
                |> Result.bind this.CodeModelBuilder                         // Passes back CodeModel/Namespace
                //|> Result.map ()                   // Passes back a writer, such as an ArrayWriter or StringBuilderWriter

            match modelResult with 
            | Error e -> invalidOp "Oops, we need to write error handling" 
                // We probably have added diagnostics and should here. But this Result is currently AppError and 
                // should probably be Diagnostic so more info can be carried
            | Ok codeModel -> 
                match outputter.Output codeModel with 
                | Error e -> invalidOp "Oops, we need to write error handling" 
                | Ok writer -> context.AddSource("myGeneratedFile.cs", writer.Output)
            ()
