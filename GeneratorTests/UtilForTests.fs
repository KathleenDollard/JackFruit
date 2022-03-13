module Generator.Tests.UtilsForTests


open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open System
open Generator.RoslynUtils
open Generator.Models
open Xunit
open Generator.NewMapping
open Generator
open System.Threading
open System.Reflection
open Generator.RoslynCSharpUtils

let testNamespace = "TestCode"
let private seperator = "\r\n"

type Source =
    | CSharpTree of SyntaxTree
    | CSharpCode of string
    | VBTree of SyntaxTree
    | VBCode of string

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


let CreateClass name (code: string list) = @$"
    public class {name}
    {{
        { code |> String.concat seperator }
    }}"


let CreateNamespace (usings:string list) name (code: string) = 
    let usings = String.concat " " usings
    @$"using System;
    namespace {name}
    {{
        { code }
    }}"


let ReadCodeFromFile fileName =
    System.IO.File.ReadAllText fileName


let AddMethodsToClass (methods:string list) =
    CreateNamespace [] testNamespace (CreateClass "ClassA" methods)


let AddStatementsToMethod (statements:string list) =
    AddMethodsToClass [ CreateMethod "MethodA" statements ]

let GetCSharpCompilation trees =
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
                //MetadataReference.CreateFromFile(@"ConsoleSupport.dll")
                ],
        options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))

let GetSemanticModelFromFirstTree trees =
    let compilation = 
        GetCSharpCompilation trees

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

let SyntaxTreeResult (source: Source) =
    let tree =
        match source with
        | CSharpTree tree -> tree
        | CSharpCode code -> (Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText code)
        | VBTree tree -> tree
        | VBCode code -> (Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxTree.ParseText code)
    
    let errors =
        [ for diag in tree.GetDiagnostics() do
            if diag.Severity = DiagnosticSeverity.Error then diag ]
    
    if errors.IsEmpty then
        Ok tree
    else
        Error (Roslyn errors)

let SyntaxTreesFrom (sources: Source list) =
    let combineTrees (trees: SyntaxTree list) source =
        let newTreeResult = SyntaxTreeResult source
        match newTreeResult with 
        | Ok newTree -> Ok (List.concat [trees; [newTree]])
        | Error err -> Error err

    let mutable result: Result<SyntaxTree list, AppErrors> = 
        match SyntaxTreeResult sources.Head with 
        | Ok tree -> Ok [tree]
        | Error err -> Error err

    for source in sources.Tail do
        let newResult = 
            match result with 
            | Ok trees -> combineTrees trees source
            | Error _ -> result
        result <- newResult
    result

let CompilationFrom(sources: Source list) =
    let result = SyntaxTreesFrom sources
    match result with 
    | Ok trees -> Ok (GetCSharpCompilation trees)
    | Error err -> Error err


 
//let ModelFrom(sources: Source list) =
//    let combineTrees (trees: SyntaxTree list) source =
//        let newTreeResult = SyntaxTreeResult source
//        match newTreeResult with 
//        | Ok newTree -> Ok (List.concat [trees; [newTree]])
//        | Error err -> Error err

//    let mutable result: Result<SyntaxTree list, AppErrors> = 
//        match SyntaxTreeResult sources.Head with 
//        | Ok tree -> Ok [tree]
//        | Error err -> Error err

//    for source in sources.Tail do
//        let newResult = 
//            match result with 
//            | Ok trees -> combineTrees trees source
//            | Error _ -> result
//        result <- newResult
//    match result with 
//    | Ok trees -> GetSemanticModelFromFirstTree trees
//    | Error err -> Error err
 

let MethodDeclarationNodesFrom (syntaxTree: CSharpSyntaxTree) = 
    Ok 
        [ for node in syntaxTree.GetRoot().DescendantNodes() do
           match node with 
           | MethodDeclaration (_, _) -> node
           | _ -> () ] 
     
     
let MethodSymbolFromMethodDeclaration (model: SemanticModel) (expression: SyntaxNode) =
    let handler =
        model.GetDeclaredSymbol expression

    let symbol =
        match handler with
        | null -> invalidOp "Delegate not found"
        | _ -> handler

    match symbol with
    | :? IMethodSymbol as m -> Some m
    | _ -> None

let MethodSymbolFromMethodCall (model: SemanticModel) (expression: SyntaxNode) =
    let handler =
        model.GetSymbolInfo expression

    match handler.Symbol with 
    | null when handler.CandidateSymbols.IsEmpty -> None
    | null -> 
        match handler.CandidateSymbols.[0] with 
        | :? IMethodSymbol as m -> Some m
        | _ -> None
    | _ -> 
        match handler.Symbol with
        | :? IMethodSymbol as m -> Some m
        | _ -> None
         
//let MethodSymbolsFromSource source =
//    let code = AddMethodsToClass source
//    let modelResult = ModelFrom [ CSharpCode code ]
//    let model =
//        match modelResult with 
//        | Ok model -> model
//        | Error _ -> invalidOp "Test failed during SemanticModel creation"
//    let tree = 
//        match model.SyntaxTree with 
//        | :? CSharpSyntaxTree as t -> t
//        | _ -> invalidOp "Unexpected syntax tree type"
//    let declarationsResults = RoslynCSharpUtils.MethodDeclarationNodesFrom (tree)
//    let declarations =
//         match declarationsResults with 
//         | Ok d -> d
//         | Error _ -> invalidOp "Test failed during Method syntax lookup"
//    let methods =
//        [ for declaration in declarations do
//            let methodResult = MethodSymbolFromMethodDeclaration model declaration 
//            match methodResult with 
//            | Some method -> method 
//            | None -> invalidOp "Test failed during Method symbol lookup" ]
//    model, methods

     
     
//let MethodSymbolFromMethodDeclaration (model: SemanticModel) (expression: SyntaxNode) =
//    let handler =
//        model.GetDeclaredSymbol expression

//    let symbol =
//        match handler with
//        | null -> invalidOp "Delegate not found"
//        | _ -> handler

//    match symbol with
//    | :? IMethodSymbol as m -> Some m
//    | _ -> None

//let MethodSymbolFromMethodCall (model: SemanticModel) (expression: SyntaxNode) =
//    let handler =
//        model.GetSymbolInfo expression

//    match handler.Symbol with 
//    | null when handler.CandidateSymbols.IsEmpty -> None
//    | null -> 
//        match handler.CandidateSymbols.[0] with 
//        | :? IMethodSymbol as m -> Some m
//        | _ -> None
//    | _ -> 
//        match handler.Symbol with
//        | :? IMethodSymbol as m -> Some m
//        | _ -> None
         
let MethodSymbolsFromSource source =
    let code = AddMethodsToClass source
    let treesResult = SyntaxTreesFrom [ CSharpCode code ]
    let trees =
        match treesResult with
        | Ok t -> t
        | Error _ -> invalidOp "Failure building syntax tree"
    let compilation = GetCSharpCompilation trees
    let csharpTree = 
        match trees[0] with // only one
        | :? CSharpSyntaxTree as t -> t
        | _ -> invalidOp "Unexpected syntax tree language"
    let declarationsResults = MethodDeclarationNodesFrom csharpTree 
    let semanticModel = compilation.GetSemanticModel(csharpTree, true)

    let declarations =
         match declarationsResults with 
         | Ok d -> d
         | Error _ -> invalidOp "Test failed during Method syntax lookup"
    let methods =
        [ for declaration in declarations do
            let methodResult = MethodSymbolFromMethodDeclaration semanticModel declaration 
            match methodResult with 
            | Some method -> method 
            | None -> invalidOp "Test failed during Method symbol lookup" ]
    semanticModel, methods

let CommandDefFromHandlerSource source =
    let model, methods = MethodSymbolsFromSource source

    [ for method in methods do
        CommandDefFromMethod {InfoCommandId = None; Method = MethodSymbol method; Path = []; ForPocket = []; Namespace = ""} ]

let ShouldEqual (expected: 'a) (actual: 'a) =     
    try
        Assert.Equal<'a>(expected, actual)
    with
        | _ -> printf "Expected: %A\nActual: %A" expected actual 


/// This custom comparer accomplishes two things: it ignores the Pocket and 
/// it gives the object on which an issue occurs. 
let CommandDefDifferences (expected: CommandDef list) (actual: CommandDef list) =
    // MemberDefUsage and CommandDefUsage are not tested because it contains data that we can't easily replicate in tests
    let CompareMember commandId (exp: MemberDef) (act: MemberDef) =
        [ let id = commandId + if String.IsNullOrEmpty(exp.MemberId) then act.MemberId else exp.MemberId
          if exp.MemberId <> act.MemberId then $"MemberId {exp.MemberId} does not match {act.MemberId}"
          if exp.TypeName <> act.TypeName then $"{id}: TypeName {exp.TypeName} does not match {act.TypeName}"
          if exp.GenerateSymbol <> act.GenerateSymbol then $"{id}: GenerateSymbol {exp.GenerateSymbol} does not match {act.GenerateSymbol}"
          if exp.MemberKind <> act.MemberKind then $"{id}: MemberKind {exp.MemberKind} does not match {act.MemberKind}"
          if exp.Aliases <> act.Aliases then $"{id}: Aliases {exp.Aliases} does not match {act.Aliases}"
          if exp.ArgDisplayName <> act.ArgDisplayName then $"{id}: ArgDisplayName {exp.ArgDisplayName} does not match {act.ArgDisplayName}"
          if exp.Description <> act.Description then $"{id}: Description {exp.Description} does not match {act.Description}"
          if exp.RequiredOverride <> act.RequiredOverride then $"{id}: RequiredOverride {exp.RequiredOverride} does not match {act.RequiredOverride}"
        ]

    let rec CompareCommand parentId (exp: CommandDef) (act: CommandDef) =
        [ let id = parentId + if String.IsNullOrEmpty(exp.CommandId) then act.CommandId else exp.CommandId
          if exp.CommandId <> act.CommandId then $"{id}: CommandId '{exp.CommandId}' does not match '{act.CommandId}'"
          if exp.ReturnType <> act.ReturnType then $"{id}: ReturnType {exp.ReturnType} does not match {act.ReturnType}"
          if exp.Path <> act.Path then $"{id}: Path {exp.Path} does not match {act.Path}"
          if exp.Aliases <> act.Aliases then $"{id}: Aliases {exp.Aliases} does not match {act.Aliases}"
          if exp.Description <> act.Description then $"{id}: Description {exp.Description} does not match {act.Description}" 
          
          if exp.Members.Length <> act.Members.Length then 
            $"{id}: Members length of expected ({exp.Members.Length}) is different than the length of actual ({act.Members.Length})"
          else
            let members = List.zip exp.Members act.Members
            for expMember, actMember in members do
                // KAD: Easier way to flatten into this list comprehension? Can be an interaction with implicit yields. May need to yield everything
                yield! CompareMember id expMember actMember

          if exp.SubCommands.Length <> act.SubCommands.Length then 
            $"{id}: SubCommands length of expected ({exp.SubCommands.Length}) is different than the length of actual ({act.SubCommands.Length})"
          else
            let subCommands = List.zip exp.SubCommands act.SubCommands
            for expSubCommand, actSubCommand in subCommands do
                let issues = CompareCommand id expSubCommand actSubCommand
                for issue in issues do issue
        ]


    if expected.Length <> actual.Length then 
        Some [ $"CommandDef length of expected ({expected.Length}) is different than the length of actual ({actual.Length})"]
    else 
        let data = List.zip expected actual
        let errors =
            [ for (exp, act) in data do
                let issues = CompareCommand "" exp act
                for issue in issues do issue
            ]
        match errors with 
        | [] -> None
        | _ -> Some errors

let RunGenerator (generator: ISourceGenerator) (inputCompilation: Compilation)=
    let driver = CSharpGeneratorDriver.Create(generator)
    let c = CancellationToken.None
    driver.RunGeneratorsAndUpdateCompilation (inputCompilation, cancellationToken = c)

let CreateCompilation (source: string) =
    CSharpCompilation.Create("compilation",
        seq {CSharpSyntaxTree.ParseText(source)},
        seq {MetadataReference.CreateFromFile(typeof<Binder>.Assembly.Location)},
        CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))       









