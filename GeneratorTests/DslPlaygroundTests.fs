module DslPlaygroundTests


open Xunit
open Generator.Language
open Common
open DslKeywords
open DslCodeBuilder
open FSharp.Linq.RuntimeHelpers.LeafExpressionConverter

let NameFromSimpleName (namedItem: NamedItem) =
    match namedItem with
    | SimpleNamedItem n -> n
    | _ -> invalidOp "Simple name not found"
  
let NameAndGenericsFromName (namedItem: NamedItem) =
    match namedItem with
    | GenericNamedItem (n, g) -> 
        n, 
        [for x in g do 
            match x with 
            | SimpleNamedItem gn -> gn
            | _ -> invalidOp "Generic was not a simple name" ]
    | _ -> invalidOp "Generic name not found"

type ``When creating a namespace``() =
    let namespaces: Namespace list = []

    // KAD-Don-Chet: It appears that we don't support the empty case.
    [<Fact(Skip="Can't create an empty thing yet. May drop requirement.")>]
    member _.``Can create a namespace``() =
        let nspace = "George"

        // KAD-Don-Chet: I thought when I created a Zero method, I'd be able to remove the body of these expressions if they were empty.
        let codeModel = 
             Namespace(nspace) {
                Using ""
                }
        Assert.Equal(nspace, codeModel.NamespaceName)
        Assert.Empty(codeModel.Usings)
        Assert.Empty(codeModel.Classes)

    [<Fact>]
    member _.``Can add using to namespace``() =
        let nspace = "George"
        let usingName = "Fred"
        let codeModel = 
             Namespace(nspace) {
                Using usingName
                }

        Assert.Equal(nspace, codeModel.NamespaceName)
        Assert.Equal(1, codeModel.Usings.Length)
        Assert.Equal(UsingModel.Create usingName, codeModel.Usings[0])
        Assert.Empty(codeModel.Classes)
 
    [<Fact>]
    member _.``Can add using with alias to namespace``() =
        let nspace = "George"
        let usingName = "Fred"
        let alias = "F"
        let expected = { Namespace = usingName; Alias = Some alias }
        let codeModel = 
             Namespace(nspace) {
                Using usingName Alias alias
                }

        Assert.Equal(nspace, codeModel.NamespaceName)
        Assert.Equal(1, codeModel.Usings.Length)
        Assert.Equal(expected, codeModel.Usings[0])
        Assert.Empty(codeModel.Classes)

    [<Fact>]
    member _.``Can add multiple usings to namespace``() =
        let nspace = "George"
        let usingName0 = "Fred"
        let usingName1 = "Sally"
        let usingAlias1 = "S"
        let usingName2 = "Sue"
        let expected0 = UsingModel.Create usingName0
        let expected1 = { Namespace = usingName1; Alias = Some usingAlias1 }
        let expected2 = UsingModel.Create usingName2
        let codeModel = 
            Namespace(nspace) 
                {
                    Using usingName0
                    Using usingName1 Alias usingAlias1
                    Using usingName2
                }

        Assert.Equal(nspace, codeModel.NamespaceName)
        Assert.Equal(3, codeModel.Usings.Length)
        Assert.Equal(expected0, codeModel.Usings[0])
        Assert.Equal(expected1, codeModel.Usings[1])
        Assert.Equal(expected2, codeModel.Usings[2])
        Assert.Empty(codeModel.Classes)
        
    [<Fact>]
    member _.``Can add class to namespace``() =
        let nspace = "George"
        let className = ["Fred"; "Bill"]
        let codeModel = 
            Namespace(nspace) {
                let x = 42
                Class(className[0]) {
                        Public }

                }

        Assert.Equal(nspace, codeModel.NamespaceName)
        Assert.Empty(codeModel.Usings)
        Assert.Equal(1, codeModel.Classes.Length)
        Assert.Equal(ClassModel.Create className[0], codeModel.Classes[0])


type ``When creating a class``() =

    
    [<Fact>]
    member _.``Can create class``() =
        let className = "George"
        let codeModel = 
            Class(className) { 
                Public
                }

        Assert.Equal(className, NameFromSimpleName codeModel.ClassName)
        Assert.Equal(Public, codeModel.Scope)
        Assert.Equal(Instance, codeModel.StaticOrInstance)
        Assert.Equal(false, codeModel.IsAsync)
        Assert.Equal(false, codeModel.IsPartial)
        Assert.Equal(None, codeModel.InheritedFrom)
        Assert.Empty(codeModel.ImplementedInterfaces)
        Assert.Empty(codeModel.Members)

    [<Fact>]
    member _.``Can create class with multiple modifiers``() =
        let className = "George"
        let codeModel = 
            Class(className) { 
                Public Static Async Partial
                }

        Assert.Equal(className, NameFromSimpleName codeModel.ClassName)
        Assert.Equal(Public, codeModel.Scope)
        Assert.Equal(StaticOrInstance.Static, codeModel.StaticOrInstance)
        Assert.True(codeModel.IsAsync)
        Assert.True(codeModel.IsPartial)
        Assert.Equal(None, codeModel.InheritedFrom)
        Assert.Empty(codeModel.ImplementedInterfaces)
        Assert.Empty(codeModel.Members)

    // using inline data hacks because VS is terrible at displaying other theory approaches in VS 2019
    [<Theory>]
    [<InlineData(0)>]
    [<InlineData(1)>]
    [<InlineData(2)>]
    [<InlineData(3)>]
    [<InlineData(4)>]
    [<InlineData(5)>]
    [<InlineData(6)>]
    [<InlineData(7)>]
    [<InlineData(8)>]
    [<InlineData(9)>]
    member _.``Can create different scopes and modifiers``(pos: int) =
        let tupleFromCodeModel (codeModel: ClassModel) = 
            codeModel.Scope, codeModel.StaticOrInstance, codeModel.IsAbstract, codeModel.IsAsync, codeModel.IsPartial, codeModel.IsSealed
        let data = 
            [ (Public, Instance, false, false, false, false), Class("A") { Public }
              (Public, Instance, false, true, false, false), Class("A") { Public Async }
              (Public, Instance, true, false, false, false), Class("A") { Public Abstract }
              (Public, StaticOrInstance.Static, true, false, true, false), Class("A") { Public Static Abstract Partial}
              (Public, Instance, true, true, true, true), Class("A") { Public Abstract Async Partial Sealed}
              (Private, Instance, false, false, false, false), Class("A") { Private }
              (Internal, Instance, false, true, false, false), Class("A") { Internal Async }
              (Protected, Instance, true, false, false, false), Class("A") { Protected Abstract }
              (Private, StaticOrInstance.Static, true, false, true, false), Class("A") { Private Static Abstract Partial}
              (Protected, Instance, true, true, true, true), Class("A") { Protected Abstract Async Partial Sealed}
            ]

        match data[pos] with 
        | expected, actual -> Assert.Equal(expected, tupleFromCodeModel actual)
                
    [<Fact>]
    member _.``Can create class with generic types``() =
        let className = "George"
        let genericNames = ["string"; "int"]
        let codeModel = 
            Class(className) {
                Generics [ SimpleNamedItem genericNames[0]; SimpleNamedItem genericNames[1] ]
                }

        let (actualName, actualGenerics) = NameAndGenericsFromName codeModel.ClassName        
        
        Assert.Equal(className, actualName)
        Assert.Equal(2, actualGenerics.Length)
        Assert.Equal(genericNames[0], actualGenerics[0])
        Assert.Equal(genericNames[1], actualGenerics[1])

    [<Fact>]
    member _.``Can create class with base class``() =
        let className = "George"
        let genericName = "Bart"
        let codeModel = 
            Class(className) {
                InheritedFrom (SimpleNamedItem genericName)
                }

        let actual = 
            match codeModel.InheritedFrom with
            | None -> invalidOp "No InheritedFrom was found"
            | Some namedItem -> 
                match namedItem with 
                | SimpleNamedItem n -> n
                | _ -> invalidOp "Simple name not found for InheritedFrom"
        Assert.Equal(genericName, actual)

    [<Fact>]
    member _.``Can create class with implemented interfaces``() =
        let className = "George"
        let interfaceNames = ["A"; "B"]
        let codeModel = 
            Class(className) {
                ImplementedInterfaces [for i in interfaceNames do NamedItem.Create i []]
                }

        Assert.Equal(2, codeModel.ImplementedInterfaces.Length)
        Assert.Equal(interfaceNames[0], NameFromSimpleName codeModel.ImplementedInterfaces[0])
        Assert.Equal(interfaceNames[1], NameFromSimpleName codeModel.ImplementedInterfaces[1])
    
    [<Fact>]
    member _.``Can add field to class``() =
        let className = "George"
        let fieldName = "A"
        let fieldType = SimpleNamedItem "int"
        // KAD-Chet: Can we do better here? We could do field with a method, but I do not 
        // think we can do other members that way
        let codeModel = 
            Class(className) {
                Field(fieldName, fieldType) {
                    Public
                    }
                }

        Assert.Equal(className, NameFromSimpleName codeModel.ClassName)
        Assert.Equal(1, codeModel.Members.Length)
        let actualField =
            match codeModel.Members.Head with 
            | :? FieldModel as f -> f
            | _ -> invalidOp "A field was not found"
        Assert.Equal(fieldName, actualField.FieldName)
        Assert.Equal(fieldType, actualField.FieldType)


type ``When creating a field``() =

    [<Fact>]
    member _.``Can create a field``() =
        let name = "A"
        let fieldType = SimpleNamedItem "int"
        let codeModel = 
            Field(name, fieldType) {
                Public
                }

        Assert.Equal(name, codeModel.FieldName)
        Assert.Equal(fieldType, codeModel.FieldType)

     
     
type ``When creating a property``() =

    [<Fact>]
    member _.``Can create a property``() =
        let name = "A"
        let propertyType = SimpleNamedItem "int"
        let codeModel = 
            Property(name, propertyType) {
                Public
                }

        Assert.Equal(name, codeModel.PropertyName)
        Assert.Equal(propertyType, codeModel.Type)


type ``When creating a constructor``() =

    [<Fact>]
    member _.``Can create a ctor``() =
        let className = "A"
        let codeModel = 
            Constructor(className) {
                Public
                }

        Assert.Equal(className, codeModel.ClassName)

        
//type ``When creating a method``() =
        
    //[<Fact>]
    //member _.``Can create a method``() =
    //    let name = "A"
    //    let returnType = Void
    //    let codeModelExpr = 
    //        Method(SimpleNamedItem name, returnType) {
    //            //Public
    //            }
        
    //    let codeModel = EvaluateQuotation codeModelExpr :?> MethodModel

    //    Assert.Equal(name, NameFromSimpleName codeModel.MethodName)
    //    Assert.Equal(returnType, codeModel.ReturnType)
        
        

type ``When creating Return statements``() =
    [<Fact>]
    member _.``Can create void return``() =
        let methodName = "A"
        let codeModelExpr = 
            Method(SimpleNamedItem methodName, Void) {
                Public
                Return
                }

        let codeModel = EvaluateQuotation codeModelExpr :?> MethodModel
        Assert.Equal(1, codeModel.Statements.Length)
        Assert.IsType<MethodModel>(codeModel)

