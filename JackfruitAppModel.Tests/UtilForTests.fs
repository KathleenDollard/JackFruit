module Jackfruit.UtilsForTests

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open System
open Generator.RoslynUtils
open Jackfruit.Models
open Generator.Tests.UtilsForTests
open Xunit
open Jackfruit.ArchetypeMapping
open Generator
open Generator.RoslynUtils

let testNamespace = "TestCode"


let HandlerSource = ReadCodeFromFile "..\..\..\TestHandlers.cs"
//let OneMapping = AddMapStatementToTestCode ["""var x = new ConsoleSupport.BuilderInferredParser(); x.MapInferred("", Handlers.A);"""]

let eval = EvalCSharp()

let AddMapStatementToTestCode (statements:string list) =
    let methods = 
        [ CreateMethod "MethodA" statements ]
    CreateNamespace [] testNamespace (CreateClass "ClassA" methods)

let AddMapStatements includeBad (statements: string list) =
    let BadMappingStatement = "builder.MapInferredX(\"\", Handlers.X);"
    AddMapStatementToTestCode 
        [ "var builder = new ConsoleSupport.BuilderInferredParser();";
          if includeBad then BadMappingStatement
          for s in statements do s ]

let AddMethodsToClassWithBuilder (source: string list) =
    let source = source |> String.concat "\r\n"
    @$"
    using System;

    namespace TestCode
    {{
        public class Builder
        {{
            public void MapInferred(string archetype, Delegate handler) {{}}
        }}

        public class ClassA
        {{
            public void RandMethod()
            {{
                var builder = new Builder();
                {source}
            }}
        }}
 
    }}"

//let ArchetypesAndModelFromSource source =
//    let source = AddMapStatements false source
//    AddStatementToTestCodeModel option = None

//    // KAD: Any better way to catch an interim value in a pipeline
//    let updateModel newModel = 
//        model <- Some newModel
//        newModel

//    let result1 = 
//        ModelFrom [CSharpCode source; CSharpCode HandlerSource]
//        |> Result.map updateModel

//    let result2 = 
//        result1
//        |> Result.bind (InvocationsFromModel "MapInferred")

//    let result = 
//        result2
//        |> Result.bind ArchetypeInfoListFrom

//        //InvocationsFromModel mapMethodName model
//        //|> Result.bind ArchetypeInfoListFrom
//        //|> Result.map ArchetypeInfoTreeFrom
//        //|> Result.map (CommandDefFrom model appModel)

//    match result with
//    | Ok archetypeList -> (archetypeList, model.Value)
//    | Error err -> invalidOp $"Test failed building archetypes from source {err}"

let InvocationsAndModelFrom source =
    //let source = AddMapStatements false source
    let mutable model:SemanticModel = null

    // KAD: Any better way to catch an interim value in a pipeline
    let updateModel newModel = 
        model <- newModel
        newModel

    ModelFrom [CSharpCode source; CSharpCode HandlerSource]
    |> Result.map updateModel
    |> Result.map (eval.InvocationsFromModel "MapInferred")
    |> Result.map (fun invocations -> (invocations, model))


//let MethodSyntaxAndModel handlerName : Result<SyntaxNode * SemanticModel, AppErrors> = 
//    let modelResult = ModelFrom [(CSharpCode HandlerSource)]
//    match modelResult with 
//    | Ok  model -> 
//         let method = 
//            MethodDeclarationsFrom model.SyntaxTree
//            |> Result.map (List.filter (fun x -> x. = handlerName))
//            |> List.exactlyOne
//         Ok method, model

//    | Error err -> Error err



