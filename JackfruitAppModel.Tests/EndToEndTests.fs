namespace Jackfruit

open Microsoft.CodeAnalysis;
open Xunit
open ApprovalTests.Reporters
open Generator.Tests
open System
open Generator.Tests.UtilsForTests
open ApprovalTests
open System.Linq

[<Generator>]
type ``When doing end to end generation``() =
    let RunCliModelGenerator (inputCompilation: Compilation) =
        RunGenerator (Jackfruit.Generator()) inputCompilation
     
    let VerifyTestResult  driver (outputCompilation: Compilation) (diagnostics: Diagnostic list) =
        Assert.Equal(0, diagnostics.Length)
        let newTree = outputCompilation.SyntaxTrees.Skip(1).SingleOrDefault()
        Assert.NotNull newTree
        Approvals.Verify(newTree.ToString())
     

    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``Code outputs for one simple command``() =
        let code = String.Join("\n", MapData.NoMapping.HandlerCode)
        let compilation = CreateCompilation code
        VerifyTestResult RunCliModelGenerator compilation