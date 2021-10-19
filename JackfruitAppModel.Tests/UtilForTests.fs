module Generator.Tests.UtilForTests

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open System
open Generator.RoslynUtils
open Jackfruit.Models
open Xunit
open Jackfruit.ArchetypeMapping
open Generator.AppErrors

let testNamespace = "TestCode"


let ConcatErrors error = 
    match error with 
    | Roslyn diagnostics -> 
        String.concat "\n\r"
            [ for error in diagnostics do error.ToString()
            ]
    | _ -> error.ToString()


let CreateMethod name (statements: string list) = 
    let code = String.concat "\r" statements 
    @$"
    public void {name}()
    {{
        {code}
    }}
    "


let createClass name code = @$"
    public class {name}
    {{
        {code}
    }}"


let CreateNamespace (usings:string list) name code = 
    let usings = String.concat " " usings
    @$"using System;
    namespace {name}
    {{
        {code}
    }}"


let ReadCodeFromFile fileName =
    System.IO.File.ReadAllText fileName


//let CommandNamesFromArchetypeInfo (result: Result<ArchetypeInfo list, Diagnostic list>) = 
//    match result with 
//    | Ok archetypes -> [for arch in archetypes do
//                            arch.Archetype.CommandName ]
//    | Error errors-> invalidOp (concatErrors errors)


let AddMapStatementToTestCode (statements:string list) =
    let methods = 
        [ CreateMethod "MethodA" statements ]
        |> String.concat ""
    CreateNamespace [] testNamespace (createClass "ClassA" methods)


let HandlerSource = ReadCodeFromFile "..\..\..\TestHandlers.cs"
let OneMapping = AddMapStatementToTestCode ["""var x = new ConsoleSupport.BuilderInferredParser(); x.MapInferred("", Handlers.A);"""]


let GetSemanticModelFromFirstTree inputTrees =
    let trees = 
        match inputTrees with 
        | f, s -> [f; s]

    let compilation = 
        let core = typeof<obj>.Assembly.Location
        let runtime = 
            let dir = IO.Path.GetDirectoryName(core)
            IO.Path.Combine(dir, "System.Runtime.dll")
        CSharpCompilation.Create(
            "test", 
            syntaxTrees = trees,
            references = [
                    MetadataReference.CreateFromFile(core)
                    MetadataReference.CreateFromFile(runtime)
                    MetadataReference.CreateFromFile(@"ConsoleSupport.dll")],
            options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))

    let errors = 
        [ for diag in compilation.GetDiagnostics() do
            if diag.Severity = DiagnosticSeverity.Error then
                diag]
    if errors.IsEmpty then
        match trees with 
        | [] -> invalidOp "Model not found for tree"
        | firstTree::_ -> Ok (compilation.GetSemanticModel (firstTree, true))
    else
        Error (Roslyn errors)


let IsResultOk result = 
    match result with 
    | Ok _ -> true
    | Error e -> false


let IsModelOk modelResult =
    let errorMessages =
            match modelResult with
            | Error e -> [for x in e do x.ToString()]
            | _ -> List.empty
            |> String.concat " "  
    printfn "%s" errorMessages
    IsResultOk modelResult


let ModelFrom (source: Source) (handlerSource: Source) =
    let makeTuple tree handlerTree =
        (tree, handlerTree)

    let combineTrees hSource (tree: SyntaxTree) =
        SyntaxTreeResult hSource
        |> Result.map (makeTuple tree)

    SyntaxTreeResult source
    |> Result.bind (combineTrees handlerSource)
    |> Result.bind GetSemanticModelFromFirstTree 


let ShouldEqual (expected: 'a) (actual: 'a) =     
    try
        Assert.Equal<'a>(expected, actual)
    with
        | _ -> printf "Expected: %A\nActual: %A" expected actual 


let AddMapStatements includeBad (statements: string list) =
    let BadMappingStatement = "builder.MapInferredX(\"\", Handlers.X);"
    AddMapStatementToTestCode [ "var builder = new ConsoleSupport.BuilderInferredParser();";
                                if includeBad then BadMappingStatement
                                for s in statements do s ]


let archetypesAndModelFromSource source =
    let source = AddMapStatements false source
    let mutable model:SemanticModel option = None

    // KAD: Any better way to catch an interim value in a pipeline
    let updateModel newModel = 
        model <- Some newModel
        newModel

    let result1 = 
        ModelFrom (CSharpCode source) (CSharpCode HandlerSource)
        |> Result.map updateModel

    let result2 = 
        result1
        |> Result.bind (InvocationsFromModel "MapInferred")

    let result = 
        result2
        |> Result.bind ArchetypeInfoListFrom

        //InvocationsFromModel mapMethodName model
        //|> Result.bind ArchetypeInfoListFrom
        //|> Result.map ArchetypeInfoTreeFrom
        //|> Result.map (CommandDefFrom model appModel)

    match result with
    | Ok archetypeList -> (archetypeList, model.Value)
    | Error err -> invalidOp $"Test failed building archetypes from source {err}"



let InvocationsAndModelFrom source =
    //let source = AddMapStatements false source
    let mutable model:SemanticModel = null

    // KAD: Any better way to catch an interim value in a pipeline
    let updateModel newModel = 
        model <- newModel
        newModel

    ModelFrom (CSharpCode source) (CSharpCode HandlerSource)
    |> Result.map updateModel
    |> Result.map (InvocationsFromModel "MapInferred")
    |> Result.map (fun invocations -> (invocations, model))

// KAD-Don Any shortcuts in this? I need a diff, not just that they do not match
//let MatchCommandDef expected actual =
//    let mutable errors = []
//    let addError err = errors <- err::errors // added backwards, then reverse
//    let checkForMatch valueName expectedValue actualValue =
//        if expectedValue <> actualValue then
//            addError $"Mismatch: {actualValue} not equal to {expectedValue} for {valueName}"
//        ()

//    let CheckArgDef path expected actual =
//        let matchesHandled = 
//            match (expected, actual) with
//            | Some _, None -> addError $"Mismatch: Arg expected, but was not found for {path}" ; true
//            | None, Some _ -> addError $"Mismatch: No Arg expected, but one found for {path}"; true
//            | None, None -> true
//            | _ -> false
//        if not matchesHandled then 
//            let expected = expected.Value
//            let actual = actual.Value
//            checkForMatch $"ArgDef.ArgId for {path}" expected.ArgId actual.ArgId
//            checkForMatch $"ArgDef.Name for {path}:{expected.ArgId}" expected.Name actual.Name
//            checkForMatch $"ArgDef.Description for {path}:{expected.ArgId}" expected.Description actual.Description
//            checkForMatch $"ArgDef.Required for {path}:{expected.ArgId}" expected.Required actual.Required
//            checkForMatch $"ArgDef.TypeName for {path}:{expected.ArgId}" expected.TypeName actual.TypeName
//        ()

//    let CheckOptionDef path expected actual =
//        checkForMatch $"OptionDef.OptionId for {path}" expected.OptionId actual.OptionId
//        checkForMatch $"OptionDef.Name for {path}:{expected.OptionId}" expected.Name actual.Name
//        checkForMatch $"OptionDef.Description for {path}:{expected.OptionId}" expected.Description actual.Description
//        checkForMatch $"OptionDef.Aliases for {path}:{expected.OptionId}" expected.Aliases actual.Aliases
//        checkForMatch $"OptionDef.Required for {path}:{expected.OptionId}" expected.Required actual.Required
//        checkForMatch $"OptionDef.TypeName for {path}:{expected.OptionId}" expected.TypeName actual.TypeName
//        ()

//    let CheckOptionDefs path (expectedOptions: OptionDef list) (actualOptions: OptionDef list) =
//        if expectedOptions.Length <> actualOptions.Length then 
//            let newErr = $"Mismacth in number of options for {path}"
//            errors <- newErr::errors
//            ()
//        else
//            let zipped =  List.zip expectedOptions actualOptions
//            for (expected, actual) in zipped do 
//                CheckOptionDef path expected actual
//            ()

//    let rec CheckCommandDef path expected actual =
//        let path = if path = "" then "<root>" else path
                    
//        checkForMatch $"CommandDef.CommandId for {path}" expected.CommandId actual.CommandId
//        checkForMatch $"CommandDef.Path for {path}" expected.Path actual.Path
//        checkForMatch $"CommandDef.Name for {path}" expected.Name actual.Name
//        checkForMatch $"CommandDef.Description for {path}" expected.Description actual.Description
//        checkForMatch $"CommandDef.Aliases for {path}" expected.Aliases actual.Aliases
//        CheckArgDef path expected.Arg actual.Arg
//        CheckOptionDefs path expected.Options actual.Options
//        CheckCommandDefs path expected.SubCommands actual.SubCommands
//        ()

//    and CheckCommandDefs path (expectedCommands: CommandDef list) (actualCommands: CommandDef list) =
//        if expectedCommands.Length <> actualCommands.Length then 
//            let newErr = $"Mismacth in number of options for {path}"
//            errors <- newErr::errors
//            ()
//        else
//            let zipped =  List.zip expectedCommands actualCommands
//            for (expected, actual) in zipped do 
//                CheckCommandDef path expected actual
//            ()

   
//    CheckCommandDef "" expected actual
//    errors |> String.Concat


