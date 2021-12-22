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
open DslKeywords


[<AbstractClass>]
type BuilderBase<'T>() =
 
    abstract member EmptyItem: unit -> 'T
    abstract member InternalCombine: 'T -> 'T -> 'T

    member this.Yield (()) : 'T = this.EmptyItem()
    member this.Zero() : 'T = this.Yield(())
    // KAD-Chet: I had to comment out the following to get Namespace to work. I do not know 
    //           what is different. And the value of quote isn't yet clear to me.
    //member _.Quote() = ()
    member this.Combine (item: 'T, item2: 'T) : 'T = this.InternalCombine item item2
    member _.Delay(f: unit -> 'T) : 'T = f()
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
    inherit BuilderBase<NamespaceModel>()

    override _.EmptyItem() =  NamespaceModel.Create name
    override _.InternalCombine nspace nspace2 =
        { nspace with 
            Usings =  List.append nspace.Usings nspace2.Usings
            Classes = List.append nspace.Classes nspace2.Classes }  

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
    inherit BuilderBase<ClassModel>()

    let updateModifiers (cls: ClassModel) scope modifiers =
        { cls with 
            Scope = scope; 
            StaticOrInstance = modifiers.StaticOrInstance
            IsAbstract = modifiers.IsAbstract
            IsAsync = modifiers.IsAsync
            IsPartial = modifiers.IsPartial
            IsSealed = modifiers.IsSealed}

    override _.EmptyItem() =  ClassModel.Create name
    override _.InternalCombine cls cls2 =
        { cls with Members =  List.append cls.Members cls2.Members }  

    [<CustomOperation("Generics", MaintainsVariableSpace = true)>]
    member _.generics (cls: ClassModel, generics: NamedItem list) =
        let currentName =
            match cls.ClassName with 
            | SimpleNamedItem n -> n
            | GenericNamedItem (n, _ ) -> n
        { cls with ClassName = NamedItem.Create currentName generics }

    [<CustomOperation("Public", MaintainsVariableSpace = true)>]
    member _.publicWithModifiers (cls: ClassModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        // KAD-Don: Is there a better way to upcast the members of an array or list?
        let modifiers = Modifiers.Evaluate (modifiers.OfType<IModifierWord>())
        updateModifiers cls Public modifiers

    [<CustomOperation("Private", MaintainsVariableSpace = true)>]
    member _.privateWithModifiers (cls: ClassModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        let modifiers = Modifiers.Evaluate (modifiers.OfType<IModifierWord>())
        updateModifiers cls Private modifiers

    [<CustomOperation("Internal", MaintainsVariableSpace = true)>]
    member _.internalWithModifiers (cls: ClassModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        let modifiers = Modifiers.Evaluate (modifiers.OfType<IModifierWord>())
        updateModifiers cls Internal modifiers

    [<CustomOperation("Protected", MaintainsVariableSpace = true)>]
    member _.protectedWithModifiers (cls: ClassModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        let modifiers = Modifiers.Evaluate (modifiers.OfType<IModifierWord>())
        updateModifiers cls Protected modifiers

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

    // KAD-Chet: I do notunderstand how adding the statements here and in combine don't double them
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
