namespace Jackfruit

open Microsoft.CodeAnalysis;
open Xunit
open ApprovalTests.Reporters
open Jackfruit.Tests
open System
open Generator.Tests.UtilsForTests
open ApprovalTests
open System.Linq

[<Generator>]
type End2End() =
    let RunCliModelGenerator (inputCompilation: Compilation) =
        RunGenerator (Jackfruit.Generator()) inputCompilation

    let RunTest (data: MapData.Data) =
        let compilation = CreateCompilation data.CliCode
        let inputDiagnostics = compilation.GetDiagnostics()
        if not (inputDiagnostics.IsEmpty) then invalidOp "Input code is invalid"
        let (driver, outputCompilation, diagnostics) = RunCliModelGenerator compilation
        Assert.Equal(0, diagnostics.Length)
        let newTree = outputCompilation.SyntaxTrees.Skip(1).SingleOrDefault()
        Assert.NotNull newTree
        Approvals.Verify(newTree.ToString())   

     
    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``No commands``() =
        RunTest MapData.NoMapping
        
    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``One simple command``() =
        RunTest MapData.OneMapping
    
    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``Three commands``() =
        RunTest MapData.ThreeMappings
