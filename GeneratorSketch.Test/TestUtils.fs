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

    let addMapStatementToTestCode (statements:string list) =
        let methods = [ createMethod "MethodA" statements ]
        // KAD: can this pipe? I did not make that work
        let x = (String.concat "" (List.toSeq methods))
        createNamespace [] testNamespace (createClass "ClassA" x)

    let handlerSource = readCodeFromFile "..\..\..\TestHandlers.cs"
    let oneMapping = addMapStatementToTestCode ["""var x = new ConsoleSupport.BuilderInferredParser(); x.MapInferred("", Handlers.A);"""]



    let getSemanticModelFromFirstTree (trees:SyntaxTree list) =
        let getCompilation (trees:SyntaxTree list) =
            // KAD: Is there a better way to do this
            let addReference r (compilation: CSharpCompilation) =
                compilation.AddReferences([r])
            let addSyntaxTrees trees (compilation: CSharpCompilation) =
                compilation.AddSyntaxTrees(trees)
            let core = typeof<obj>.Assembly.Location
            let dir = IO.Path.GetDirectoryName(core)
            let runtime = IO.Path.Combine(dir, "System.Runtime.dll")
            let compilation = CSharpCompilation.Create(
                                    "test", 
                                    options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                                    |> addReference (MetadataReference.CreateFromFile(core))
                                    |> addReference (MetadataReference.CreateFromFile(runtime))
                                    |> addReference (MetadataReference.CreateFromFile(@"ConsoleSupport.dll"))
                                    |> addSyntaxTrees (trees.ToArray())
            compilation  

        let compilation = getCompilation trees
        let errors = [ for diag in compilation.GetDiagnostics() do
                          if diag.Severity = DiagnosticSeverity.Error then
                              diag
                          else
                              ()]
        if errors.IsEmpty then
            match trees with 
                        | [] -> invalidOp "Model not found for tree"
                        | firstTree::_ -> Ok (compilation.GetSemanticModel (firstTree, true))
        else
            Error errors

    let getSemanticModelFromSource (code:string) (otherCode:string list) =
        let trees = (CSharpSyntaxTree.ParseText code)::
                                [ for source in otherCode do
                                    (CSharpSyntaxTree.ParseText source) ]
        getSemanticModelFromFirstTree trees

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

    let buildHandler mapping handlerSource =
        let tree = parseTreeThrowOnErrors mapping
        let handlerTree = parseTreeThrowOnErrors handlerSource

        let modelResult = getSemanticModelFromFirstTree [tree; handlerTree]
        let model = match modelResult with 
                        | Ok m -> m
                        | Error e -> invalidOp "Semantic model creation failed"
        let commandInfo = archetypeInfoFrom (SyntaxTree tree)
                         |> List.exactlyOne
        evaluateHandler model commandInfo.HandlerExpression

    let shouldEqual (expected: 'a) (actual: 'a) =     
        try
            Assert.Equal<'a>(expected, actual)
        with
            | _ -> printf "Expected: %A\nActual: %A" expected actual 