namespace LanguageDslTests

open Xunit
open Generator
open Generator.LanguageModel
open Common
open DslForCode
open Generator.LanguageHelpers
open Generator.LanguageHelpers.Statements
open DslKeywords
open Generator.LanguageStatements


type ``Create namespaces with``() =
// Namespaces are with nothing specified is currently not possible

    [<Fact>]
    member _.``Simple using`` () =
        let usingName = "MyUsing"
        let namespaceName = "MyNamespace"
        let expected = { NamespaceName = namespaceName
                         Usings = [ UsingModel.Create usingName ]
                         Classes = [] }
        let actual =
            Namespace(namespaceName) { 
                Using usingName }
        Assert.Equal(expected, actual.Model)

    [<Fact>]
    member _.``Using with alias`` () =
        let namespaceName = "MyNamespace"
        let usingName = "MyUsing"
        let alias = "Mine"
        let expected = { NamespaceName = namespaceName
                         Usings = [ {UsingNamespace = usingName; Alias = Some alias} ]
                         Classes = [] }
        let actual =
            Namespace(namespaceName) { 
                Using usingName alias }
        Assert.Equal(expected, actual.Model)

    [<Fact>]
    member _.``Single public class`` () =
        let namespaceName = "MyNamespace"
        let className = "MyUsing"
        let expected = 
            { NamespaceName = namespaceName
              Usings = [ ]
              Classes = [ ClassModel.Create(className, Public) ] }
        let actual =
            Namespace(namespaceName) { 
                // KAD-Don: Are empty CEs just not practical
                Class(className) {
                    Public2 } }
        Assert.Equal(expected, actual.Model)


type ``Create classes with``() =
    // Classes are with nothing specified is currently not possible

    [<Fact>]
    member _.``Simple Public class`` () =
        let className = "MyClass"
        let expected =  
            ClassModel.Create(className, Public)
        let actual =
            Class(className) {
                Public2
                }
        Assert.Equal(expected, actual.Model)

    [<Fact>]
    member _.``Simple Internal class`` () =
        let className = "MyClass"
        let expected =  
            ClassModel.Create(className, Internal)
        let actual =
            Class(className) {
                Internal
                }
        Assert.Equal(expected, actual.Model)

    [<Fact>]
    member _.``Simple Friend class`` () =
        let className = "MyClass"
        let expected =  
            ClassModel.Create(className, Internal)
        let actual =
            Class(className) {
                Friend
                }
        Assert.Equal(expected, actual.Model)
    
    [<Fact>]
    member _.``Simple Protected class`` () =
        let className = "MyClass"
        let expected =  
            ClassModel.Create(className, Protected)
        let actual =
            Class(className) {
                Protected
                }
        Assert.Equal(expected, actual.Model)
    
    [<Fact>]
    member _.``Simple Private class`` () =
        let className = "MyClass"
        let expected =  
            ClassModel.Create(className, Private)
        let actual =
            Class(className) {
                Private
                }
        Assert.Equal(expected, actual.Model)
  
    [<Fact>]
    member _.``Simple ProtectedInternal class`` () =
        let className = "MyClass"
        let expected =  
            ClassModel.Create(className, ProtectedInternal)
        let actual =
            Class(className) {
                ProtectedInternal
                }
        Assert.Equal(expected, actual.Model)
    
    [<Fact>]
    member _.``Simple PrivateProtected class`` () =
        let className = "MyClass"
        let expected =  
            ClassModel.Create(className, PrivateProtected)
        let actual =
            Class(className) {
                PrivateProtected
                }
        Assert.Equal(expected, actual.Model)
    
    [<Fact>]
    member _.``Public Static class`` () =
        let className = "MyClass"
        let expected =  
            { ClassModel.Create(className, Public) with
                Modifiers = [ Static ] }
        let actual =
            Class(className) {
                Public2 Static
                }
        Assert.Equal(expected, actual.Model)

    [<Fact>]
    member _.``Public Abstract class`` () =
        let className = "MyClass"
        let expected =  
            { ClassModel.Create(className, Public) with
                Modifiers = [ Abstract ] }
        let actual =
            Class(className) {
                Public2 Abstract
                }
        Assert.Equal(expected, actual.Model)

    [<Fact>]
    member _.``Public Sealed class`` () =
        let className = "MyClass"
        let expected =  
            { ClassModel.Create(className, Public) with
                Modifiers = [ Sealed ] }
        let actual =
            Class(className) {
                Public2 Sealed
                }
        Assert.Equal(expected, actual.Model)

    [<Fact>]
    member _.``Public Async class`` () =
        let className = "MyClass"
        let expected =  
            { ClassModel.Create(className, Public) with
                Modifiers = [ Async ] }
        let actual =
            Class(className) {
                Public2 Async
                }
        Assert.Equal(expected, actual.Model)

    [<Fact>]
    member _.``Public Partial class`` () =
        let className = "MyClass"
        let expected =  
            { ClassModel.Create(className, Public) with
                Modifiers = [ Partial ] }
        let actual =
            Class(className) {
                Public2 Partial
                }
        Assert.Equal(expected, actual.Model)

    [<Fact>]
    member _.``Private Async and Partial class`` () =
        let className = "MyClass"
        let expected =  
            { ClassModel.Create(className, Private) with
                Modifiers = [ Async; Partial ] }
        let actual =
            Class(className) {
                Private Async Partial
                }
        Assert.Equal(expected, actual.Model)

    
    [<Fact>]
    member _.``Internal Async and Partial class`` () =
        let className = "MyClass"
        let expected =  
            { ClassModel.Create(className, Internal) with
                Modifiers = [ Async; Partial ] }
        let actual =
            Class(className) {
                Internal Async Partial
                }
        Assert.Equal(expected, actual.Model)

    
    [<Fact>]
    member _.``Friend Async and Partial class`` () =
        let className = "MyClass"
        let expected =  
            { ClassModel.Create(className, Internal) with
                Modifiers = [ Async; Partial ] }
        let actual =
            Class(className) {
                Friend Async Partial
                }
        Assert.Equal(expected, actual.Model)
    
    [<Fact>]
    member _.``Protected Async and Partial class`` () =
        let className = "MyClass"
        let expected =  
            { ClassModel.Create(className, Protected) with
                Modifiers = [ Async; Partial ] }
        let actual =
            Class(className) {
                Protected Async Partial
                }
        Assert.Equal(expected, actual.Model)
    
    [<Fact>]
    member _.``ProtectedInternal Async and Partial class`` () =
        let className = "MyClass"
        let expected =  
            { ClassModel.Create(className, ProtectedInternal) with
                Modifiers = [ Async; Partial ] }
        let actual =
            Class(className) {
                ProtectedInternal Async Partial
                }
        Assert.Equal(expected, actual.Model)
    
    [<Fact>]
    member _.``PrivateProtected Async and Partial class`` () =
        let className = "MyClass"
        let expected =  
            { ClassModel.Create(className, PrivateProtected) with
                Modifiers = [ Async; Partial ] }
        let actual =
            Class(className) {
                PrivateProtected Async Partial
                }
        Assert.Equal(expected, actual.Model)
  
    [<Fact>]
    member _.``InheritedFrom`` () =
        let className = "MyClass"
        let baseName = "MyBaseClass"
        let expected =  
            { ClassModel.Create(className, Public) with
                InheritedFrom = SomeBase (NamedItem.Create baseName) }
        let actual =
            Class(className) {
                Public2
                InheritedFrom baseName
                }
        Assert.Equal(expected, actual.Model)
    
    [<Fact>]
    member _.``Add member`` () =
        let className = "MyClass"
        let methodName = "MyField"
        let typeName = "string"
        let expectedField =  
            { FieldModel.Create methodName  typeName with Scope = Public }
        let expected =  
            { ClassModel.Create(className, Public) with
                Members = [ expectedField ] }
        let actual =
               Field(methodName, typeName) {
                   Public2
                   }
        let actual =
            Class(className) {
                Public2
                Field(methodName, typeName) {
                                Public2
                                }
                }
        Assert.Equal(expected, actual.Model)


type ``Create Fields with a ``() =
    [<Fact>]
    member _.``Public string`` () =
        let fieldName = "MyField"
        let typeName = "string"
        let expected =  
            { FieldModel.Create fieldName  typeName with Scope = Public }
        let actual =
            Field(fieldName, typeName) {
                Public2
                }
        Assert.Equal(expected, actual.Model :?> FieldModel)

    [<Fact>]
    member _.``Private string`` () =
        let fieldName = "MyField"
        let typeName = "string"
        let expected =  
            { FieldModel.Create fieldName  typeName with Scope = Public }
        let actual =
            Field(fieldName, typeName) {
                Public2
                }
        Assert.Equal(expected, actual.Model :?> FieldModel)

    [<Fact>]
    member _.``Internal string`` () =
        let fieldName = "MyField"
        let typeName = "string"
        let expected =  
            { FieldModel.Create fieldName  typeName with Scope = Internal }
        let actual =
            Field(fieldName, typeName) {
                Internal
                }
        Assert.Equal(expected, actual.Model :?> FieldModel)

    [<Fact>]
    member _.``Friend string`` () =
        let fieldName = "MyField"
        let typeName = "string"
        let expected =  
            { FieldModel.Create fieldName  typeName with Scope = Internal }
        let actual =
            Field(fieldName, typeName) {
                Friend
                }
        Assert.Equal(expected, actual.Model :?> FieldModel)

    [<Fact>]
    member _.``Protected string`` () =
        let fieldName = "MyField"
        let typeName = "string"
        let expected =  
            { FieldModel.Create fieldName  typeName with Scope = Protected }
        let actual =
            Field(fieldName, typeName) {
                Protected
                }
        Assert.Equal(expected, actual.Model :?> FieldModel)

    [<Fact>]
    member _.``ProtectedInternal string`` () =
        let fieldName = "MyField"
        let typeName = "string"
        let expected =  
            { FieldModel.Create fieldName  typeName with Scope = ProtectedInternal }
        let actual =
            Field(fieldName, typeName) {
                ProtectedInternal
                }
        Assert.Equal(expected, actual.Model :?> FieldModel)

    [<Fact>]
    member _.``PrivateProtected string`` () =
        let fieldName = "MyField"
        let typeName = "string"
        let expected =  
            { FieldModel.Create fieldName  typeName with Scope = PrivateProtected }
        let actual =
            Field(fieldName, typeName) {
                PrivateProtected
                }
        Assert.Equal(expected, actual.Model :?> FieldModel)

    [<Fact>]
    member _.``PrivateProtected int`` () =
        let fieldName = "MyField"
        let typeName = "int"
        let expected =  
            { FieldModel.Create fieldName  typeName with Scope = PrivateProtected }
        let actual =
            Field(fieldName, typeName) {
                PrivateProtected
                }
        Assert.Equal(expected, actual.Model :?> FieldModel)
   
    [<Fact>]
    member _.``Add field to class`` () =
        let className = "MyClass"
        let fieldName = "MyField"
        let typeName = "string"
        let expectedField =  
            { FieldModel.Create fieldName  typeName with Scope = Public }
        let expected =  
            { ClassModel.Create(className, Public) with
                Members = [ expectedField ] }
        let actual =
            Class(className) {
                Public2
                Field(fieldName, typeName) {
                                Public2
                                }
                }
        Assert.Equal(expected, actual.Model)


type ``Create Property with a ``() =
    [<Fact>]
    member _.``Public string`` () =
        let propertyName = "MyProperty"
        let typeName = "string"
        let expected =  
            { PropertyModel.Create propertyName  typeName with Scope = Public }
        let actual =
            Property(propertyName, typeName) {
                Public2
                }
        Assert.Equal(expected, actual.Model :?> PropertyModel)

    [<Fact>]
    member _.``Private string`` () =
        let propertyName = "MyProperty"
        let typeName = "string"
        let expected =  
            { PropertyModel.Create propertyName  typeName with Scope = Public }
        let actual =
            Property(propertyName, typeName) {
                Public2
                }
        Assert.Equal(expected, actual.Model :?> PropertyModel)

    [<Fact>]
    member _.``Internal string`` () =
        let propertyName = "MyProperty"
        let typeName = "string"
        let expected =  
            { PropertyModel.Create propertyName  typeName with Scope = Internal }
        let actual =
            Property(propertyName, typeName) {
                Internal
                }
        Assert.Equal(expected, actual.Model :?> PropertyModel)

    [<Fact>]
    member _.``Friend string`` () =
        let propertyName = "MyProperty"
        let typeName = "string"
        let expected =  
            { PropertyModel.Create propertyName  typeName with Scope = Internal }
        let actual =
            Property(propertyName, typeName) {
                Friend
                }
        Assert.Equal(expected, actual.Model :?> PropertyModel)

    [<Fact>]
    member _.``Protected string`` () =
        let propertyName = "MyProperty"
        let typeName = "string"
        let expected =  
            { PropertyModel.Create propertyName  typeName with Scope = Protected }
        let actual =
            Property(propertyName, typeName) {
                Protected
                }
        Assert.Equal(expected, actual.Model :?> PropertyModel)

    [<Fact>]
    member _.``ProtectedInternal string`` () =
        let propertyName = "MyProperty"
        let typeName = "string"
        let expected =  
            { PropertyModel.Create propertyName  typeName with Scope = ProtectedInternal }
        let actual =
            Property(propertyName, typeName) {
                ProtectedInternal
                }
        Assert.Equal(expected, actual.Model :?> PropertyModel)

    [<Fact>]
    member _.``PrivateProtected string`` () =
        let propertyName = "MyProperty"
        let typeName = "string"
        let expected =  
            { PropertyModel.Create propertyName  typeName with Scope = PrivateProtected }
        let actual =
            Property(propertyName, typeName) {
                PrivateProtected
                }
        Assert.Equal(expected, actual.Model :?> PropertyModel)

    [<Fact>]
    member _.``PrivateProtected int`` () =
        let propertyName = "MyProperty"
        let typeName = "int"
        let expected =  
            { PropertyModel.Create propertyName  typeName with Scope = PrivateProtected }
        let actual =
            Property(propertyName, typeName) {
                PrivateProtected
                }
        Assert.Equal(expected, actual.Model :?> PropertyModel)

    [<Fact>]
    member _.``Add property to class`` () =
        let className = "MyClass"
        let propertyName = "MyProperty"
        let typeName = "string"
        let expectedProperty =  
            { PropertyModel.Create propertyName  typeName with Scope = Public }
        let expected =  
            { ClassModel.Create(className, Public) with
                Members = [ expectedProperty ] }
        let actual =
            Class(className) {
                Public2
                Property(propertyName, typeName) {
                                Public2
                                }
                }
        Assert.Equal(expected, actual.Model)
        
type ``Create Methods with a ``() =
    [<Fact>]
    member _.``Public string`` () =
        let methodName = "MyMethod"
        let typeName = "string"
        let expected =  
            { MethodModel.Create methodName with Scope = Public }
        let actual =
            Method(methodName) {
                Public2
                }
        Assert.Equal(expected, actual.Model :?> MethodModel)

    [<Fact>]
    member _.``Private string`` () =
        let methodName = "MyMethod"
        let expected =  
            { MethodModel.Create methodName with Scope = Public }
        let actual =
            Method(methodName) {
                Public2
                }
        Assert.Equal(expected, actual.Model :?> MethodModel)

    [<Fact>]
    member _.``Internal string`` () =
        let methodName = "MyMethod"
        let expected =  
            { MethodModel.Create methodName with Scope = Internal }
        let actual =
            Method(methodName) {
                Internal
                }
        Assert.Equal(expected, actual.Model :?> MethodModel)

    [<Fact>]
    member _.``Friend string`` () =
        let methodName = "MyMethod"
        let expected =  
            { MethodModel.Create methodName with Scope = Internal }
        let actual =
            Method(methodName) {
                Friend
                }
        Assert.Equal(expected, actual.Model :?> MethodModel)

    [<Fact>]
    member _.``Protected string`` () =
        let methodName = "MyMethod"
        let expected =  
            { MethodModel.Create methodName with Scope = Protected }
        let actual =
            Method(methodName) {
                Protected
                }
        Assert.Equal(expected, actual.Model :?> MethodModel)

    [<Fact>]
    member _.``ProtectedInternal string`` () =
        let methodName = "MyMethod"
        let expected =  
            { MethodModel.Create methodName with Scope = ProtectedInternal }
        let actual =
            Method(methodName) {
                ProtectedInternal
                }
        Assert.Equal(expected, actual.Model :?> MethodModel)

    [<Fact>]
    member _.``PrivateProtected string`` () =
        let methodName = "MyMethod"
        let expected =  
            { MethodModel.Create methodName with Scope = PrivateProtected }
        let actual =
            Method(methodName) {
                PrivateProtected
                }
        Assert.Equal(expected, actual.Model :?> MethodModel)

    [<Fact>]
    member _.``Add method to clsss`` () =
        let className = "MyClass"
        let methodName = "MyMethod"
        let expectedMethod =  
             { MethodModel.Create methodName with Scope = Public }
        let expected =  
             { ClassModel.Create(className, Public) with
                 Members = [ expectedMethod ] }
        let actual =
             Class(className) {
                 Public2
                 Method(methodName) {
                                 Public2
                                 }
                 }
        Assert.Equal(expected, actual.Model)
 
    [<Fact>]
    member _.``Add statement to method`` () =
        let methodName = "MyMethod"
        let varName = "myVar"
        let expected =  
             { MethodModel.Create methodName with
                Scope = Public 
                Statements = [ AssignmentModel.Create varName (Literal 42) ] }
        let actual =
            Method(methodName) {
                Public2
                Assign varName To 42
                }
        Assert.Equal(expected, actual.Model :?> MethodModel)


type ``Create Constructors with a ``() =
    [<Fact>]
    member _.``Public string`` () =
        let expected =  
            { ConstructorModel.Create() with Scope = Public }
        let actual =
            Constructor() {
                Public2
                }
        Assert.Equal(expected, actual.Model :?> ConstructorModel)

    [<Fact>]
    member _.``Private string`` () =
        let expected =  
            { ConstructorModel.Create() with Scope = Public }
        let actual =
            Constructor() {
                Public2
                }
        Assert.Equal(expected, actual.Model :?> ConstructorModel)

    [<Fact>]
    member _.``Internal string`` () =
        let expected =  
            { ConstructorModel.Create() with Scope = Internal }
        let actual =
            Constructor() {
                Internal
                }
        Assert.Equal(expected, actual.Model :?> ConstructorModel)

    [<Fact>]
    member _.``Friend string`` () =
        let expected =  
            { ConstructorModel.Create() with Scope = Internal }
        let actual =
            Constructor() {
                Friend
                }
        Assert.Equal(expected, actual.Model :?> ConstructorModel)

    [<Fact>]
    member _.``Protected string`` () =
        let expected =  
            { ConstructorModel.Create() with Scope = Protected }
        let actual =
            Constructor() {
                Protected
                }
        Assert.Equal(expected, actual.Model :?> ConstructorModel)

    [<Fact>]
    member _.``ProtectedInternal string`` () =
        let expected =  
            { ConstructorModel.Create() with Scope = ProtectedInternal }
        let actual =
            Constructor() {
                ProtectedInternal
                }
        Assert.Equal(expected, actual.Model :?> ConstructorModel)

    [<Fact>]
    member _.``PrivateProtected string`` () =
        let expected =  
            { ConstructorModel.Create() with Scope = PrivateProtected }
        let actual =
            Constructor() {
                PrivateProtected
                }
        Assert.Equal(expected, actual.Model :?> ConstructorModel)

    [<Fact>]
    member _.``Add method to clsss`` () =
        let className = "MyClass"
        let expectedConstructor =  
            { ConstructorModel.Create() with Scope = Public }
        let expected =  
            { ClassModel.Create(className, Public) with
                Members = [ expectedConstructor ] }
        let actual =
            Class(className) {
                Public2
                Constructor() {
                                Public2
                                }
                }
        Assert.Equal(expected, actual.Model)
    
    [<Fact>]
    member _.``Add statement to constructor`` () =
        let varName = "myVar"
        let expected =  
             { ConstructorModel.Create() with
                Scope = Public 
                Statements = [ AssignmentModel.Create varName (Literal 42) ] }
        let actual =
            Constructor() {
                Public2
                Assign varName To 42
                }
        Assert.Equal(expected, actual.Model :?> ConstructorModel)


type ``Everything``() =
    [<Fact>]
    member _.``Everything so far`` () =
        let usingName = "MyUsing"
        let namespaceName = "MyNamespace"
        let className = "MyClass"
        let expectedClass =  
            { ClassModel.Create(className, Public) with
                Modifiers = [ ] }
        let expected = { NamespaceName = namespaceName
                         Usings = [ UsingModel.Create usingName ]
                         Classes = [expectedClass] }
        let actual =
            Namespace(namespaceName) { 
                Using usingName "" 
                Class(className) {
                    Public2 
                    } }
        Assert.Equal(expected, actual.Model)
