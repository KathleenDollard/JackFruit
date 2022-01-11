namespace Generator.CSharpShim

open Microsoft.CodeAnalysis;
open Microsoft.CodeAnalysis.Text;
open System.Text;

[<Generator>]
type CustomGenerator() =
    interface ISourceGenerator with
        member this.Initialize (context: GeneratorInitializationContext) =
            ()

        member this.Execute (context: GeneratorExecutionContext) =
            context.AddSource("myGeneratedFile.cs", SourceText.From(@"
    namespace GeneratedNamespace
    {
    public class GeneratedClass
        {
        }
    }", Encoding.UTF8))
