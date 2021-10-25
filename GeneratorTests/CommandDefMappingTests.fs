module Generator.Tests.CommandDefMappingTests

open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open Generator.RoslynUtils
open Generator.GeneralUtils
open Generator.Models
open Generator.Tests.UtilsForTests
open Microsoft.CodeAnalysis
open Generator.Tests
open Generator.NewMapping
open Generator


// I'm not sure what we should be testing first
//  * Creating CommandDef from random method (per most APpModels) : ``When building CommandDefs``
//  * Creating CommandDef from SetHandler (Kevin's model)
//  * Creating a CommandDef by hand and testing providers
//  * Creating a CommandDef by hand and generating Kevin's code
//  * Something else 

type ``When building CommandDefs``() =

    let MethodSymbolsFromSource source =
        let code = AddMethodsToClass source
        let modelResult = ModelFrom [ CSharpCode code ]
        let model =
            match modelResult with 
            | Ok model -> model
            | Error _ -> invalidOp "Test failed during SemanticModel creation"
        let declarationsResults = MethodDeclarationNodesFrom model.SyntaxTree
        let declarations =
             match declarationsResults with 
             | Ok d -> d
             | Error _ -> invalidOp "Test failed during Method syntax lookup"
        let methods =
            [ for declaration in declarations do
                let methodResult = MethodSymbolFromMethodDeclaration model declaration 
                match methodResult with 
                | Some method -> method 
                | None -> invalidOp "Test failed during Method symbol lookup" ]
        model, methods

    let TestCommandDefFromSource map =
        let model, methods = MethodSymbolsFromSource map.HandlerCode
        let expected = map.CommandDef

        let actual = 
            [ for method in methods do
                CommandDefFromMethod model {InfoCommandId = None; Method = Some method; Path = []; ForPocket = []} ]
        let differences = (CommandDefDifferences expected actual)

        match differences with 
        | None -> () // All is great!
        | Some issues -> 
            // KAD-Don: Why the second (from left) set of parens?
            raise (MatchException (expected.ToString(), actual.ToString(), (String.concat "\r\n" issues)))
        


    [<Fact>]
    member _.``One simple comand built``() =
        TestCommandDefFromSource MapData.OneSimpleMapping

    [<Fact>]
    member _.``One complex comand built``() =
        TestCommandDefFromSource MapData.OneComplexMapping

    [<Fact>]
    member _.``Three simple commands built``() =
        TestCommandDefFromSource MapData.ThreeMappings

    [<Fact>]
    member _.``No command does noto throw``() =
        TestCommandDefFromSource MapData.NoMapping

                

