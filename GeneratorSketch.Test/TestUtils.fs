module TestUtils

    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open System
    open System.Linq
    open GeneratorSketch.Generator
    open CSharpTestCode
    open Xunit

    let testNamespace = "TestCode"

    let concatErrors errors = String.concat "\n\r" [ for error in errors do
                                                            error.ToString() ]

    let CommandNamesFromArchetypeInfo (result: Result<ArchetypeInfo list, Diagnostic list>) = 
        match result with 
        | Ok archetypes -> [for arch in archetypes do
                                arch.Archetype.CommandName ]
        | Error errors-> invalidOp (concatErrors errors)

    let addMapStatementToTestCode (statements:string list) =
        let methods = 
            [ createMethod "MethodA" statements ]
            |> String.concat ""
        createNamespace [] testNamespace (createClass "ClassA" methods)

    let handlerSource = readCodeFromFile "..\..\..\TestHandlers.cs"
    let oneMapping = addMapStatementToTestCode ["""var x = new ConsoleSupport.BuilderInferredParser(); x.MapInferred("", Handlers.A);"""]



    let getSemanticModelFromFirstTree (trees:SyntaxTree list) =
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


    let getSemanticModelFromSource (source: Source) (otherSources:Source list) =
        let treeResults = 
            syntaxTreeResult source::
            [ for src in otherSources do
                syntaxTreeResult src]

        // KAD: Is there an easier way to tease these apart than traversing the list twice
        let errors = 
            [ for result in treeResults do 
                match result with 
                | Error resultErrors -> 
                    for error in resultErrors do
                        error
                | Ok _ -> ()]

        if errors.IsEmpty then
            getSemanticModelFromFirstTree 
                [ for result in treeResults do 
                    match result with 
                    | Ok tree -> tree
                    | Error _ -> ()]
        else
            Error errors


    let isResultOk result = 
        match result with 
        | Ok _ -> true
        | Error e -> false


    let isModelOk modelResult =
        let errorMessages =
             match modelResult with
               | Error e -> [for x in e do x.ToString()]
               | _ -> List.empty
             |> String.concat " "  
        printfn "%s" errorMessages
        isResultOk modelResult


    let parseTreeThrowOnErrors (source: string) =
        let tree = CSharpSyntaxTree.ParseText(source)     
        if tree.GetDiagnostics().Count() > 0 then 
            invalidOp "Compilation failed during Arrange"
        tree


    let modelFrom (source: Source) (handlerSource: Source) =
        let tree = 
            match syntaxTreeResult source with
            | Ok tr -> tr
            | Error errors -> invalidOp (concatErrors errors)
        let handlerTree = 
            match syntaxTreeResult handlerSource with
            | Ok tr -> tr
            | Error errors -> invalidOp (concatErrors errors)

        let modelResult = getSemanticModelFromFirstTree [tree; handlerTree]
        match modelResult with 
        | Ok model -> model
        | Error e -> invalidOp "Semantic model creation failed"


    let shouldEqual (expected: 'a) (actual: 'a) =     
        try
            Assert.Equal<'a>(expected, actual)
        with
            | _ -> printf "Expected: %A\nActual: %A" expected actual 


    let shouldBeSome (expected: 'a) (actual: Option<'a>) = 
        match actual with 
        | Some v -> Assert.Equal<'a>(expected, v)
        | None -> Assert.Equal("Some", "None") //hack


    let shouldBeNone (actual: Option<'a>) = 
        match actual with 
        | Some _ -> Assert.Equal("None", "Some") //hack
        | None -> ()


    