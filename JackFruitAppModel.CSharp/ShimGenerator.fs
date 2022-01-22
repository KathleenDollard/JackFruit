namespace Generator.CSharpShim

open Microsoft.CodeAnalysis;
open Microsoft.CodeAnalysis.Text;
open System.Text;

[<Generator(LanguageNames.CSharp)>]
type CustomGeneratorCSharp() =
    interface ISourceGenerator with
        member this.Initialize (context: GeneratorInitializationContext) =
            ()

        member this.Execute (context: GeneratorExecutionContext) =
            context.AddSource("myGeneratedFile.cs", SourceText.From(@"
    namespace GeneratedNamespace
    {
    public class GeneratedClass
        {
          // Hello world
        }
    }", Encoding.UTF8))

//[<Generator(LanguageNames.VisualBasic)>]
//type CustomGeneratorVB() =
//    interface ISourceGenerator with
//        member this.Initialize (context: GeneratorInitializationContext) =
//            ()

//        member this.Execute (context: GeneratorExecutionContext) =
//            context.AddSource("myGeneratedFile.cs", SourceText.From(@"
//   ' TODO: !! ", Encoding.UTF8))
