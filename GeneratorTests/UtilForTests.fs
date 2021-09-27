module Generator.Tests.UtilForTests

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open System
open Generator.RoslynUtils
open Xunit

let testNamespace = "TestCode"


let ConcatErrors errors = String.concat "\n\r" [ for error in errors do
                                                        error.ToString() ]


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


let GetSemanticModelFromFirstTree (trees:SyntaxTree list) =
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
        Error errors


let GetSemanticModelFromSource (source: Source) (otherSources:Source list) =
    let treeResults = 
        SyntaxTreeResult source::
        [ for src in otherSources do
            SyntaxTreeResult src]

    // KAD: Is there an easier way to tease these apart than traversing the list twice
    let errors = 
        [ for result in treeResults do 
            match result with 
            | Error resultErrors -> 
                for error in resultErrors do
                    error
            | Ok _ -> ()]

    if errors.IsEmpty then
        GetSemanticModelFromFirstTree 
            [ for result in treeResults do 
                match result with 
                | Ok tree -> tree
                | Error _ -> ()]
    else
        Error errors


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
    let tree = 
        match SyntaxTreeResult source with
        | Ok tr -> tr
        | Error errors -> invalidOp (ConcatErrors errors)
    let handlerTree = 
        match SyntaxTreeResult handlerSource with
        | Ok tr -> tr
        | Error errors -> invalidOp (ConcatErrors errors)

    let modelResult = GetSemanticModelFromFirstTree [tree; handlerTree]
    match modelResult with 
    | Ok model -> model
    | Error e -> invalidOp "Semantic model creation failed"


let ShouldEqual (expected: 'a) (actual: 'a) =     
    try
        Assert.Equal<'a>(expected, actual)
    with
        | _ -> printf "Expected: %A\nActual: %A" expected actual 
