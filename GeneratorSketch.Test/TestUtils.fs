module TestUtils

    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.CSharp
    open System
    open System.Linq
    open GeneratorSketch.Generator
    open CSharpTestCode
    open Xunit

    let testNamespace = "TestCode"
    let handlerMethod = "public static void MapInferred(string archetype, Delegate handler) {}"

    let addMapStatementToTestCode (statement:string) =
        let methods = [ createMethod "MethodA" statement; handlerMethod ]
        // KAD: can this pipe? I did not make that work
        let x = (String.concat "" (List.toSeq methods))
        createNamespace [] testNamespace (createClass "ClassA" x)

    let handlerSource = readCodeFromFile "..\..\..\TestHandlers.cs"
    let oneMapping = addMapStatementToTestCode """MapInferred("", Handlers.A);"""

    let getCompilation (trees:SyntaxTree list) =
        let baseCompilation = CSharpCompilation.Create("foo", options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
        // KAD: Is there a way to combine these? 
        let compilation2 = baseCompilation.AddSyntaxTrees (List.toArray trees)
        compilation2.AddReferences(MetadataReference.CreateFromFile(typeof<Object>.Assembly.Location))

    let getSemanticModelFromFirstTree (trees:SyntaxTree list) =
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
        let commandInfo = commandInfo tree
                         |> List.exactlyOne
        evaluateHandler model commandInfo.HandlerExpression

    let shouldEqual (expected: 'a) (actual: 'a) =     
        try
            Assert.Equal<'a>(expected, actual)
        with
            | _ -> printf "Expected: %A\nActual: %A" expected actual 