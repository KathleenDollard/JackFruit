namespace Generator

open Microsoft.CodeAnalysis;
open Xunit
open ApprovalTests.Reporters
open System
open Generator.Tests.UtilsForTests
open ApprovalTests
open System.Linq
open Generator.SourceGenerator
open Generator
open Generator.Tests.MapExplicitAddData
open Generator.ExplicitAdd

type ExplicitAddTestGenerator() =
    inherit Jackfruit.ExplicitAddGenerator()

    override _.CodeModelBuilder commandDef =
        BuildNewerCliCodeModel.OutputCommandWrapper commandDef

//[<Generator>]
type ExplicitAddEndToEndTests() =
    let RunCliModelGenerator (inputCompilation: Compilation) generator =
        RunGenerator generator inputCompilation

    let RunTest (data: Data) generator =
        let compilation = CreateCompilation data.CliCode
        let inputDiagnostics = compilation.GetDiagnostics()
        if not (inputDiagnostics.IsEmpty) then invalidOp "Input code is invalid"
        let (driver, outputCompilation, diagnostics) = RunCliModelGenerator compilation generator
        if not (inputDiagnostics.IsEmpty) then invalidOp "Generation failed"
        let newTree = outputCompilation.SyntaxTrees.Skip(1).SingleOrDefault()
        Assert.NotNull newTree
        Approvals.Verify(newTree.ToString())   

     
    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``No commands``() =
        RunTest NoMapping (Jackfruit.ExplicitAddGenerator())
        
    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``One simple command``() =
        RunTest OneMapping (Jackfruit.ExplicitAddGenerator())
    
    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``Three commands``() =
        RunTest ThreeMappings (Jackfruit.ExplicitAddGenerator())

    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``No commands with test generator``() =
        RunTest NoMapping (ExplicitAddTestGenerator())
   
    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``One simple command with test generator``() =
        RunTest OneMapping (ExplicitAddTestGenerator())
    
    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``Three commands with test generator``() =
        RunTest ThreeMappings (ExplicitAddTestGenerator())
