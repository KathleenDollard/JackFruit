module DslCodeBuilder

// KAD-Don: What is this syntax (from:https://putridparrot.com/blog/my-first-attempt-as-implementing-a-computational-expression-in-f/
//     let (Items items) = curves
//     Specifically, the part in parens. A deconstructing cast from context?

open System
open Generator.Language
open Common
open type Generator.Language.Statements
open System.Linq
open System.Collections.Generic

type AliasWord =
| Alias

type IModifierWord = interface end
type IClassModifierWord = interface inherit IModifierWord end
type IFieldModifierWord = interface inherit IModifierWord end
type IMethodModifierWord = interface inherit IModifierWord end

type StaticWord =
    | Static
    interface IClassModifierWord
    interface IFieldModifierWord
    interface IMethodModifierWord

type AsyncWord = 
    | Async
    interface IClassModifierWord
    interface IMethodModifierWord

type PartialWord =
    | Partial
    interface IClassModifierWord
    interface IMethodModifierWord

type AbstractWord = 
    | Abstract
    interface IClassModifierWord
    interface IMethodModifierWord

type SealedWord = 
    | Abstract
    interface IClassModifierWord
    interface IMethodModifierWord

type OfWord =
    | Of

let Modifiers (modifiers:IEnumerable<IModifierWord>) staticOrInstance isAsync isPartial =
    (  
        (if modifiers.Contains Static then StaticOrInstance.Static else staticOrInstance),
        (if modifiers.Contains Async then true else isAsync),
        (if modifiers.Contains Partial then true else isPartial)
    )


// I want to do something like the following (NamedItem exists). Punctuation minimized as I think I need to be flexible.
//
//   typeName: NamedItem = "typeName" Of "T1"  "T2"  (Of "T3" (Of "T4")) "T4"
//
// The only way I have thought to do this is to have a dummy C# class "GenericShim" that has an implicit 
// conversion from string to itself, then have a paramArray of the shim type. Can the types IntelliSense 
// avoid this being entirely non-discoverable.

[<AbstractClass>]
type BuilderBase<'T when 'T :> IStatementContainer<'T>>() =
 
    abstract member EmptyItem: unit -> 'T
    abstract member InternalCombine: 'T -> 'T -> 'T

    member this.Yield (()) : 'T = this.EmptyItem()
    member this.Zero() : 'T = this.Yield(())
    member _.Quote() = ()
    member _.Delay(f: unit -> 'T) : 'T = f()
    member this.Combine (item: 'T, item2: 'T) : 'T = this.InternalCombine item item2
    member this.For(items, f) : 'T = 
        let itemList = Seq.toList items
        match itemList with 
        | [] -> this.Zero()
        | [x] -> f(x)
        | head::tail ->
            let mutable headResult = f(head)
            for x in tail do 
                headResult <- this.Combine(headResult, f(x))
            headResult

[<AbstractClass>]
type StatementBuilderBase<'T when 'T :> IStatementContainer<'T>>() =
    inherit BuilderBase<'T>()

    [<CustomOperation("Return", MaintainsVariableSpace = true)>]
    member _.addReturn (method: MethodModel) =
        { method with Statements =  List.append method.Statements [ {ReturnModel.Expression = None} ] }
 

type Namespace(name: string) =

    member _.Yield (_) =  NamespaceModel.Create name
    member this.Zero(_) = this.Yield()
    //member _.Quote() = ()

    [<CustomOperation("Usings", MaintainsVariableSpace = true)>]
    member _.addUsings (nspace: NamespaceModel, usings): NamespaceModel =
        nspace.AddUsings usings

    [<CustomOperation("Using", MaintainsVariableSpace = true)>]
    member _.addUsing (nspace: NamespaceModel, name: string, _:AliasWord, alias: string): NamespaceModel =
        let newUsing =
            if String.IsNullOrWhiteSpace alias then
                { Namespace = name; Alias = None } 
            else
                { Namespace = name; Alias = Some alias } 
        nspace.AddUsings [ newUsing ]
        
    [<CustomOperation("Using", MaintainsVariableSpace = true)>]
    member this.addUsing (nspace: NamespaceModel, name: string): NamespaceModel =
        this.addUsing(nspace, name, Alias, "")
  
    [<CustomOperation("Classes", MaintainsVariableSpace = true)>]
    member _.addClasses (nspace: NamespaceModel, classes): NamespaceModel =
        nspace.AddClasses classes


type Class(name: string) =
      
    let updateModifiers (cls: ClassModel) scope staticOrInstance isAsync isPartial =
        { cls with Scope = scope; StaticOrInstance = staticOrInstance; IsAsync = isAsync; IsPartial = isPartial}
        
    member _.Yield (_) = ClassModel.Create(name, Public, [])
    member this.Zero() = this.Yield()

    //// TODO: Passing a named item will be quite messy. This problemw will recur. Harder if we can't get overloads
    //[<CustomOperation("Name", MaintainsVariableSpace = true)>]
    //member _.name (cls: ClassModel, name: string, _: OfWord, generics: NamedItem list) =
    //    let generics =
    //        match generics with 
    //        | [] -> SimpleNamedItem name
    //        | GenericNamedItem (n, _ ) -> n
    //    { cls with ClassName = NamedItem.Create currentName generics }

    //// TODO: Passing a named item will be quite messy. This problemw will recur. Harder if we can't get overloads
    //[<CustomOperation("Name", MaintainsVariableSpace = true)>]
    //member _.name (cls: ClassModel, name: string) =
    //    SimpleNamedItem name

    // TODO: Passing a named item will be quite messy. This problemw will recur. Harder if we can't get overloads
    [<CustomOperation("Generics", MaintainsVariableSpace = true)>]
    member _.generics (cls: ClassModel, generics: NamedItem list) =
        let currentName =
            match cls.ClassName with 
            | SimpleNamedItem n -> n
            | GenericNamedItem (n, _ ) -> n
        { cls with ClassName = NamedItem.Create currentName generics }

    // TODO: Add seale and abstract to class model and here
    [<CustomOperation("Public", MaintainsVariableSpace = true)>]
    member _.publicWithModifiers (cls: ClassModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        let (staticOrInstance, isAsync, isPartial) = 
            Modifiers (modifiers.OfType<IModifierWord>()) cls.StaticOrInstance cls.IsAsync cls.IsPartial
        updateModifiers cls Public staticOrInstance isAsync isPartial

    [<CustomOperation("Private", MaintainsVariableSpace = true)>]
    member _.privateWithModifiers (cls: ClassModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        let (staticOrInstance, isAsync, isPartial) = 
            Modifiers (modifiers.OfType<IModifierWord>()) cls.StaticOrInstance cls.IsAsync cls.IsPartial
        updateModifiers cls Private staticOrInstance isAsync isPartial

    [<CustomOperation("Internal", MaintainsVariableSpace = true)>]
    member _.internalWithModifiers (cls: ClassModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        let (staticOrInstance, isAsync, isPartial) = 
            Modifiers (modifiers.OfType<IModifierWord>()) cls.StaticOrInstance cls.IsAsync cls.IsPartial
        updateModifiers cls Internal staticOrInstance isAsync isPartial

    [<CustomOperation("Protected", MaintainsVariableSpace = true)>]
    member _.protectedWithModifiers (cls: ClassModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        let (staticOrInstance, isAsync, isPartial) = 
            Modifiers (modifiers.OfType<IModifierWord>()) cls.StaticOrInstance cls.IsAsync cls.IsPartial
        updateModifiers cls Protected staticOrInstance isAsync isPartial

    // TODO: Passing a named item will be quite messy. This problemw will recur. Harder if we can't get overloads
    [<CustomOperation("InheritedFrom", MaintainsVariableSpace = true)>]
    member _.inheritedFrom (cls: ClassModel, inheritedFrom: NamedItem) =
        // Consider whether resetting this to None is a valid scenario
        { cls with InheritedFrom = Some inheritedFrom }

    // TODO: Passing a named item will be quite messy. This problemw will recur. Harder if we can't get overloads
    [<CustomOperation("ImplementedInterfaces", MaintainsVariableSpace = true)>]
    member _.interfaces (cls: ClassModel, [<ParamArray>]interfaces: NamedItem list) =
        { cls with ImplementedInterfaces = interfaces }

    [<CustomOperation("Members", MaintainsVariableSpace = true)>]
    member _.members (cls: ClassModel, members: IMember list) =
        { cls with Members = members }


// KAD-Don: I have not been able to create a CE that supports an empty body. Is it possible?
//          I want to allow: 
//            Field(n, t) { }
//            Field(n, t) { Public Static }
type Field(name: string, typeName: NamedItem) =
    let updateModifiers (field: FieldModel) scope staticOrInstance  =
        { field with Scope = scope; StaticOrInstance = staticOrInstance }
        
    member _.Yield (_) = FieldModel.Create name typeName
    member this.Zero() = this.Yield()

    // TODO: Add async and partial to class model and here
    [<CustomOperation("Public", MaintainsVariableSpace = true)>]
    member _.modifiers (cls: FieldModel) =
        updateModifiers cls Public Instance


type Method(name: NamedItem, returnType: ReturnType) =
    //inherit StatementContainerBuilder<MethodModel>()

    let updateModifiers (method: MethodModel) scope staticOrInstance  =
        { method with Scope = scope; StaticOrInstance = staticOrInstance }
        
    member _.Yield (()) : MethodModel = MethodModel.Create name returnType
    member this.Yield (statement: IStatement) : MethodModel = 
        { this.Zero() with Statements = [statement] }
    member _.Combine (method: MethodModel, method2: MethodModel) : MethodModel=
        { method with Statements =  List.append method.Statements method2.Statements }
    member this.Zero() : MethodModel = this.Yield(())
    member _.Quote() = ()
    member _.Delay(f: unit -> MethodModel) : MethodModel = f()
    member this.For(methods, f) :MethodModel = 
        let methodList = Seq.toList methods
        match methodList with 
        | [] -> this.Zero()
        | [x] -> f(x)
        | head::tail ->
            let mutable headResult = f(head)
            for x in tail do 
                headResult <- this.Combine(headResult, f(x))
            headResult

    // TODO: Add async and partial to class model and here
    [<CustomOperation("Public", MaintainsVariableSpace = true)>]
    member _.modifiers (method: MethodModel) =
        updateModifiers method Public Instance

    [<CustomOperation("Return", MaintainsVariableSpace = true)>]
    member _.addReturn (method: MethodModel) =
        { method with Statements =  List.append method.Statements [ {ReturnModel.Expression = None} ] }


type Property(name: string, typeName: NamedItem) =
    let updateModifiers (property: PropertyModel) scope staticOrInstance  =
        { property with Scope = scope; StaticOrInstance = staticOrInstance }
        
    member _.Yield (_) = PropertyModel.Create name typeName
    member this.Zero() = this.Yield()

    [<CustomOperation("Public", MaintainsVariableSpace = true)>]
    member _.modifiers (property: PropertyModel) =
        updateModifiers property Public Instance

type Constructor(className: string) =
    let updateModifiers (ctor: ConstructorModel) scope staticOrInstance  =
        { ctor with Scope = scope; StaticOrInstance = staticOrInstance }
        
    member _.Yield (_) = ConstructorModel.Create className
    member this.Zero() = this.Yield()

    [<CustomOperation("Public", MaintainsVariableSpace = true)>]
    member _.modifiers (ctor: ConstructorModel) =
        updateModifiers ctor Public Instance

//type Return() =
//    member _.Yield (_) = { ReturnModel.Expression = None }
//    member this.Zero() = this.Yield()
   


//type ClassModel = unit
//type FieldModel = unit
//type ClassBuilder () =
//    let mutable count = 0
//    member x.Zero (): unit  = () // this is our state for the CE run
//    member x.Yield (v) = v
    
//    // Combine allows you to integrate some kind of external state 
//    // into your 'base' state
    
//    member x.Combine(state: ClassModel, field: FieldModel) = 
//        state.AddField field
    
//    member x.Combine(state: ClassModel, method: MethodModel) = 
//        state.AddMethod method
        
//    [<CustomOperation("Field")>]
//    member x.Field(state: unit, scope: string, name: string, ty: System.Type) = 
//        // add these field members to some backing state
//        ()
    
    
//type ModelClassBuilder() = 
//    inherit ClassBuilder ()
    
//    member x.Run(state: unit) = state
    
    
//let Class = ModelClassBuilder()

//let Field = //FieldBuilder()

//let thingClass = 
//    Class {
//      Field {
//          scope "public"
//          name "Thing"
//          typ typeof<int>
//      }
//      Method {
          
//      }
//    }
