module SourceGeneratorTests

open Microsoft.CodeAnalysis.CSharp;

open Microsoft.CodeAnalysis;
open System.Collections.Immutable
open System.Threading
open Xunit
open Microsoft.CSharp.RuntimeBinder
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open System.Linq
open ApprovalTests.Reporters
open ApprovalTests
open Generator.Tests
open System
open Generator.Tests.UtilsForTests
open Generator.SourceGenerator

[<Generator>]
type TestGenerator() =
    interface ISourceGenerator with 
        member _.Initialize(context) =
            ()

        member _.Execute(context) =
            let output = @"
            namespace GeneratedNamespace
            {
                public class GeneratedClass
                {
                    public static void GeneratedMethod()
                    {
                        // generated code
                    }
                }
            }"
            context.AddSource("myGeneratedFile.cs", output)
            ()


type ``When testing source generators``() =

    //let RunGenerator (generator: ISourceGenerator) (inputCompilation: Compilation)=
    //    let driver = CSharpGeneratorDriver.Create(generator)
    //    let c = CancellationToken.None
    //    driver.RunGeneratorsAndUpdateCompilation (inputCompilation, cancellationToken = c)

    //let CreateCompilation (source: string) =
    //    CSharpCompilation.Create("compilation",
    //        seq {CSharpSyntaxTree.ParseText(source)},
    //        seq {MetadataReference.CreateFromFile(typeof<Binder>.Assembly.Location)},
    //        CSharpCompilationOptions(OutputKind.ConsoleApplication))       

    [<Fact>]
    member _.``Simple tests work``() =
        let input = @"
namespace MyCode
{
    public class Program
    {
        public static void Main(string[] args)
        {
        }
    }
}
"
        let inputCompilation = CreateCompilation(input);
        let _, output, diagnostics = RunGenerator (TestGenerator()) inputCompilation
        Assert.Equal (2, (output.SyntaxTrees.Count()))
        Assert.Equal (0, (diagnostics.Count()))
        Assert.True (output.SyntaxTrees.Skip(1).First().ToString().Contains("namespace GeneratedNamespace"))

//    [<Fact>]
//    [<UseReporter(typeof<DiffReporter>)>]
//    member _.``Code outputs for three simple commands``() =
//        let code = String.Join("\n", MapData.OneSimpleMapping.HandlerCode)
//        let compilation = CreateCompilation code
//        VerifyTestResult RunCliModelGenerator compilation

//    [<Fact>]
//    [<UseReporter(typeof<DiffReporter>)>]
//    member _.``No command does noto throw``() =
//        let code = String.Join("\n", MapData.OneComplexMapping.HandlerCode)
//        let compilation = CreateCompilation code
//        VerifyTestResult RunCliModelGenerator compilation

//    [<Fact>]
//    [<UseReporter(typeof<DiffReporter>)>]
//    member _.``No command does noto throw``() =
//        let code = String.Join("\n", MapData.ThreeMappings.HandlerCode)
//        let compilation = CreateCompilation code
//        VerifyTestResult RunCliModelGenerator compilation


