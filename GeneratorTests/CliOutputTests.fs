module Generator.CliOutputTests

open Xunit
open Generator.Tests
open Generator
open Generator.LanguageRoslynOut
open Generator.BuildCliCodeModel
open ApprovalTests
open ApprovalTests.Reporters

type ``When creating a code from model``() =

    let cSharp = LanguageCSharp() :> ILanguage
    let outputter = RoslynOut (cSharp, ArrayWriter(3))

    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``can create one simple mapping``() =
        let codeModelResult = OutputCommandWrapper MapData.OneSimpleMapping.CommandDef
        let codeModel = 
            match codeModelResult with 
            | Ok codeModel ->  codeModel
            | Error e -> invalidOp $"Failed creating code model {e}"
        let writer = outputter.Output codeModel
        let code = writer.Output

        Approvals.Verify(code)

