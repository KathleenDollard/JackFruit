namespace Jackfruit

open Microsoft.CodeAnalysis;
open Xunit
open ApprovalTests.Reporters
open Jackfruit.Tests
open System
open Generator.Tests.UtilsForTests
open ApprovalTests
open System.Linq
open Generator.SourceGenerator
open Generator

type TestGenerator() =
    inherit Jackfruit.Generator()

    override _.CodeModelBuilder commandDef =
        BuildNewCliCodeModel.OutputCommandWrapper commandDef

[<Generator>]
type End2End() =
    let RunCliModelGenerator (inputCompilation: Compilation) generator =
        RunGenerator generator inputCompilation

    let RunTest (data: MapData.Data) generator =
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
        RunTest MapData.NoMapping (Jackfruit.Generator())
        
    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``One simple command``() =
        RunTest MapData.OneMapping (Jackfruit.Generator())
    
    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``Three commands``() =
        RunTest MapData.ThreeMappings (Jackfruit.Generator())

    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``No commands with test generator``() =
        RunTest MapData.NoMapping (TestGenerator())
   
    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``One simple command with test generator``() =
        RunTest MapData.OneMapping (TestGenerator())
    
    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``Three commands with test generator``() =
        RunTest MapData.ThreeMappings (TestGenerator())
