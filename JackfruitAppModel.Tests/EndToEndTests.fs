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
     
    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``One simple command``() =
        let code = String.Join("\n", MapData.NoMapping.HandlerStatements)
        let compilation = CreateCompilation code
        let (driver, outputCompilation, diagnostics) = RunCliModelGenerator compilation
        Assert.Equal(0, diagnostics.Length)
        let newTree = outputCompilation.SyntaxTrees.Skip(1).SingleOrDefault()
        Assert.NotNull newTree
        Approvals.Verify(newTree.ToString())
        
    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``No commands``() =
        let code = String.Join("\n", MapData.OneMapping.HandlerStatements)
        let compilation = CreateCompilation code
        let (driver, outputCompilation, diagnostics) = RunCliModelGenerator compilation
        Assert.Equal(0, diagnostics.Length)
        let newTree = outputCompilation.SyntaxTrees.Skip(1).SingleOrDefault()
        Assert.NotNull newTree
        Approvals.Verify(newTree.ToString())        