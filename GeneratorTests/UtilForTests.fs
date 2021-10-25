module Generator.Tests.UtilsForTests


open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open System
open Generator.RoslynUtils
open Generator.Models
open Xunit
open Generator

let testNamespace = "TestCode"
let private seperator = "\r\n"


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


let GetSemanticModelFromFirstTree trees =
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




let ModelFrom(sources: Source list) =
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
    match result with 
    | Ok trees -> GetSemanticModelFromFirstTree trees
    | Error err -> Error err
        

let ShouldEqual (expected: 'a) (actual: 'a) =     
    try
        Assert.Equal<'a>(expected, actual)
    with
        | _ -> printf "Expected: %A\nActual: %A" expected actual 

   

// KAD-Don Any shortcuts in this? I need a diff, not just that they do not match

/// This custom comparer accomplishes two things: it ignores the Pocket and 
/// it gives the object on which an issue occurs. 
let CommandDefDifferences (expected: CommandDef list) (actual: CommandDef list) =
    let CompareMember commandId exp act =
        [ let id = commandId = if String.IsNullOrEmpty(exp.MemberId) then act.MemberId else exp.MemberId
          if exp.MemberId <> act.MemberId then $"MemberId {exp.MemberId} does not match {act.MemberId}"
          if exp.TypeName <> act.TypeName then $"{id}: TypeName {exp.TypeName} does not match {act.TypeName}"
          if exp.MemberUsage <> act.MemberUsage then $"{id}: MemberUsage {exp.MemberUsage} does not match {act.MemberUsage}"
          if exp.GenerateSymbol <> act.GenerateSymbol then $"{id}: GenerateSymbol {exp.GenerateSymbol} does not match {act.GenerateSymbol}"
          if exp.MemberKind <> act.MemberKind then $"{id}: MemberKind {exp.MemberKind} does not match {act.MemberKind}"
          if exp.Aliases <> act.Aliases then $"{id}: Aliases {exp.Aliases} does not match {act.Aliases}"
          if exp.ArgDisplayName <> act.ArgDisplayName then $"{id}: ArgDisplayName {exp.ArgDisplayName} does not match {act.ArgDisplayName}"
          if exp.Description <> act.Description then $"{id}: Description {exp.Description} does not match {act.Description}"
          if exp.RequiredOverride <> act.RequiredOverride then $"{id}: RequiredOverride {exp.RequiredOverride} does not match {act.RequiredOverride}"
     
        
        ]

    let rec CompareCommand parentId exp act =
        [ let id = parentId + if String.IsNullOrEmpty(exp.CommandId) then act.CommandId else exp.CommandId
          if exp.CommandId <> act.CommandId then $"CommandId {exp.CommandId} does not match {act.CommandId}"
          if exp.GenerateSetHandler <> act.GenerateSetHandler then $"{id}: GenerateSetHandler {exp.GenerateSetHandler} does not match {act.GenerateSetHandler}"
          if exp.Path <> act.Path then $"{id}: Path {exp.Path} does not match {act.Path}"
          if exp.Aliases <> act.Aliases then $"{id}: Aliases {exp.Aliases} does not match {act.Aliases}"
          if exp.Description <> act.Description then $"{id}: Description {exp.Description} does not match {act.Description}" 
          
          if exp.Members.Length > act.Members.Length then 
            $"{id}: Length of expected ({exp.Members.Length}) is different than the length of actual ({act.Members.Length})"
          else
            let members = List.zip exp.Members act.Members
            for expMember, actMember in members do
                // KAD-Don: Is there an easier way to flatten into this list comprehension?
                let issues = CompareMember id expMember actMember
                for issue in issues do issue

          if exp.SubCommands.Length > act.SubCommands.Length then 
            $"{id}: Length of expected ({exp.SubCommands.Length}) is different than the length of actual ({act.SubCommands.Length})"
          else
            let subCommands = List.zip exp.SubCommands act.SubCommands
            for expSubCommand, actSubCommand in subCommands do
                let issues = CompareCommand id expSubCommand actSubCommand
                for issue in issues do issue
        ]


    // if an error occurs in the following, update it _and update the comparison_
    // ** This should remain, although it is unused, and should not switch to Create method call **
    let test = { CommandId = ""
                 ReturnType = None
                 GenerateSetHandler = true
                 Path = []
                 Aliases = []
                 Description = None
                 Members = []
                 SubCommands = []
                 Pocket = [] }
    if expected.Length > actual.Length then 
        Some [ $"Length of expected ({expected.Length}) is different than the length of actual ({actual.Length})"]
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

     




    //let mutable errors = []
    //let addError err = errors <- err::errors // added backwards, then reverse
    //let checkForMatch valueName expectedValue actualValue =
    //    if expectedValue <> actualValue then
    //        addError $"Mismatch: {actualValue} not equal to {expectedValue} for {valueName}"
    //    ()

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


