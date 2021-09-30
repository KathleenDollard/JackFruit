module Generator.Tests.UtilForTests

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open System
open Generator.RoslynUtils
open Generator.Models
open Xunit

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
    // KAD: Is there an easier way to do this?
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
