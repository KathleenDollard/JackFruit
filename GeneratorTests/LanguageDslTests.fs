namespace LanguageDslTests

open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open Generator.GeneralUtils
open Generator
open Generator.Tests.TestData
open Generator.Language
open Common
open Generator.LanguageExpressions
open Generator.LanguageStatements
open Generator.LanguageRoslynOut
open Generator.LanguageHelpers
open System.Collections.Generic
open DslForCode
open DslKeywords


type ``Create namespaces with``() =
// Namespaces are with nothing specified is currently not possible

    [<Fact>]
    member _.``Simple using`` () =
        let usingName = "MyUsing"
        let namespaceName = "MyNamespace"
        let expected = { NamespaceName = namespaceName
                         Usings = [ UsingModel.Create usingName ]
                         Classes = [] }
        // KAD-Don: Uncomment the empty string here when DSL can overload
        // KAD-Don: The return here is the wrapped model. While I understand why, it's problematic
        let actual =
            Namespace(namespaceName) { 
                Using usingName "" }
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
            // KAD-Don: Nesting builders doesn't seem to work here
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
    member _.``Public Async and Partial class`` () =
        let className = "MyClass"
        let expected =  
            { ClassModel.Create(className, Public) with
                Modifiers = [ Async; Partial ] }
        let actual =
            Class(className) {
                Public2 Async Partial
                }
        Assert.Equal(expected, actual.Model)


