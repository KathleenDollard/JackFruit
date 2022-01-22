module Generator.CliOutputTests

open Xunit
open Generator.Tests
open Generator
open Generator.LanguageRoslynOut
open Generator.BuildNewerCliCodeModel
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
        let writerResult = outputter.Output codeModel
        let code =
            match writerResult with 
            | Error e -> invalidOp "Unexpected error in test"
            | Ok writer -> 
                writer.Output

        Approvals.Verify(code)

