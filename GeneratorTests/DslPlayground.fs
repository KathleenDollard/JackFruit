module DslPlayground

open DslCodeBuilder
open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open Generator.RoslynUtils
open Microsoft.CodeAnalysis
open Generator.Language
open Common

type ``When using DSL``() =

    [<Fact>]
    member _.``Can add using to namespace``() =
        let nspace = "George"
        let usingName = "Fred"
        let Namespace = NamespaceBuilder(nspace)
        let codeModel = 
            Namespace {
                Usings [ { Namespace = usingName; Alias = None } ]
                }

        Assert.Equal(nspace, codeModel.NamespaceName)
        Assert.Equal(1, codeModel.Usings.Length)
        Assert.Equal(codeModel.Usings[0], UsingModel.Create usingName)
        Assert.Empty(codeModel.Classes)

    
    [<Fact>]
    member _.``Can add multiple usings to namespace``() =
        let nspace = "George"
        let usingName0 = "Fred"
        let usingName1 = "Sally"
        let usingName2 = "Sue"
        let codeModel = 
            NamespaceBuilder(nspace) 
                {
                    Usings 
                        [ { Namespace = usingName0; Alias = None } 
                          { Namespace = usingName1; Alias = None } 
                          { Namespace = usingName2; Alias = None } ]

                    Using "Jill"
                    // KAD-Don: Why doesn't this work? (I have not gotten any overloads to work)
                    // Using "Jack" Alias "Hill"

                }

        Assert.Equal(nspace, codeModel.NamespaceName)
        Assert.Equal(4, codeModel.Usings.Length)
        Assert.Equal(UsingModel.Create usingName0, codeModel.Usings[0])
        Assert.Equal(UsingModel.Create usingName1, codeModel.Usings[1])
        Assert.Equal(UsingModel.Create usingName2, codeModel.Usings[2])
        Assert.Empty(codeModel.Classes)

    [<Fact>]
    member _.``Can add class to namespace``() =
        let nspace = "George"
        let className = ["Fred"; "Sally"; "Sue"]
        let Namespace = NamespaceBuilder("George")
        let codeModel = 
            Namespace {
                Classes 
                    [ for n in className do
                        ClassModel.Create(n, Public, []) ]
                }

        Assert.Equal(nspace, codeModel.NamespaceName)
        Assert.Equal(3, codeModel.Classes.Length)
        Assert.Equal(ClassModel.Create className[0], codeModel.Classes[0])
        Assert.Equal(ClassModel.Create className[1], codeModel.Classes[1])
        Assert.Equal(ClassModel.Create className[2], codeModel.Classes[2])
        Assert.Empty(codeModel.Usings)
    
    [<Fact>]
    member _.``Can create class``() =
        let className = "George"
        let Class = ClassBuilder(className)
        let codeModel = 
            Class { 
                Public
                }

        let actualName = 
            match codeModel.ClassName with
            | SimpleNamedItem n -> n
            | _ -> invalidOp "Simple name not found"
        Assert.Equal(className, actualName)
        Assert.Equal(Public, codeModel.Scope)
        Assert.Equal(Instance, codeModel.StaticOrInstance)
        Assert.Equal(false, codeModel.IsAsync)
        Assert.Equal(false, codeModel.IsPartial)
        Assert.Equal(None, codeModel.InheritedFrom)
        Assert.Empty(codeModel.ImplementedInterfaces)
        Assert.Empty(codeModel.Members)
     
    [<Fact>]
    member _.``Can create class with generic types``() =
        let className = "George"
        let genericNames = ["string"; "int"]
        let Class = ClassBuilder(className)
        let codeModel = 
            Class {
                Generics [ SimpleNamedItem genericNames[0]; SimpleNamedItem genericNames[1] ]
                }

        let (actualName, actualGenerics) = 
            match codeModel.ClassName with
            | GenericNamedItem (n, g) -> 
                n, 
                [for x in g do 
                    match x with 
                    | SimpleNamedItem gn -> gn
                    | _ -> invalidOp "Generic was not a simple name" ]
            | _ -> invalidOp "Generic name not found"
        Assert.Equal(className, actualName)
        Assert.Equal(2, actualGenerics.Length)
        Assert.Equal(actualGenerics[0], genericNames[0])
        Assert.Equal(actualGenerics[1], genericNames[1])

    [<Fact>]
    member _.``Can create class with base class``() =
        let className = "George"
        let genericName = "Bart"
        let Class = ClassBuilder(className)
        let codeModel = 
            Class {
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
        let Class = ClassBuilder(className)
        let codeModel = 
            Class {
                Public
                // KAD-Don: Why doesn't this work? (I have not gotten most overloads to work)
                //Public Static

                }

        Assert.True false

    [<Fact>]
    member _.``Can create class with members``() =
        let className = "George"
        let Class = ClassBuilder(className)
        let codeModel = 
            Class {
                Public
                // KAD-Don: Why doesn't this work? (I have not gotten most overloads to work)
                //Public Static

                }

        Assert.True false
