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
    member this.Combine (method: 'T, method2: 'T) : 'T=
        this.InternalCombine method method2
    member this.Zero() : 'T = this.Yield(())
    member _.Delay(f: unit -> 'T) : 'T = f()
    member this.For(methods, f) :'T = 
        let methodList = Seq.toList methods
        match methodList with 
        | [] -> this.Zero()
        | [x] -> f(x)
        | head::tail ->
            let mutable headResult = f(head)
            for x in tail do 
                headResult <- this.Combine(headResult, f(x))
            headResult

    //member this.Yield (()) : 'T = this.EmptyItem()
    //member this.Zero() : 'T = this.Yield(())
    //// KAD-Chet: I had to comment out the following to get Namespace to work. I do not know 
    ////           what is different. And the value of quote isn't yet clear to me.
    ////member _.Quote() = ()
    //member this.Combine (item: 'T, item2: 'T) : 'T = this.InternalCombine item item2
    //member _.Delay(f: unit -> 'T) : 'T = f()
    //member this.For(items, f) : 'T = 
    //    let itemList = Seq.toList items
    //    match itemList with 
    //    | [] -> this.Zero()
    //    | [x] -> f(x)
    //    | head::tail ->
    //        let mutable headResult = f(head)
    //        for x in tail do 
    //            headResult <- this.Combine(headResult, f(x))
    //        headResult

[<AbstractClass>]
type StatementBuilderBase<'T when 'T :> IStatementContainer<'T>>() =
    inherit BuilderBase<'T>()

    [<CustomOperation("Return", MaintainsVariableSpace = true)>]
    member _.addReturn (container: 'T) =
        container.AddStatements [ {ReturnModel.Expression = None} ]
 

type Namespace(name: string) =
    inherit BuilderBase<NamespaceModel>()

    override _.EmptyItem() : NamespaceModel =  NamespaceModel.Create name
    override _.InternalCombine nspace nspace2 : NamespaceModel =
        { nspace with 
            Usings =  List.append nspace.Usings nspace2.Usings
            Classes = List.append nspace.Classes nspace2.Classes }  
    member this.Yield (classModel: ClassModel) : NamespaceModel = 
        { this.Zero() with Classes = [ classModel ] }
 
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
    member this.Yield (memberModel: IMember) : ClassModel = 
        { this.Zero() with Members = [ memberModel ] }

    //member _.Quote() = ()

    [<CustomOperation("Public", MaintainsVariableSpace = true)>]
    member _.publicWithModifiers (cls: ClassModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        // KAD-Don: Is there a better way to upcast the members of an array or list?
        // Try making Evaluate generic with 'T when IModifierWord and array and then pass array
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

    [<CustomOperation("Generics", MaintainsVariableSpace = true)>]
    member _.generics (cls: ClassModel, generics: NamedItem list) =
        let currentName =
            match cls.ClassName with 
            | SimpleNamedItem n -> n
            | GenericNamedItem (n, _ ) -> n
        { cls with ClassName = NamedItem.Create currentName generics }

    [<CustomOperation("AddMember", MaintainsVariableSpace = true)>]
    member _.addMember (cls: ClassModel, memberModel: IMember) =
        { cls with Members =  List.append cls.Members [ memberModel ] }
 


// KAD-Don: I have not been able to create a CE that supports an empty body. Is it possible?
//          I want to allow: 
//            Field(n, t) { }
//            Field(n, t) { Public Static }
type Field(name: string, typeName: NamedItem) =
    inherit BuilderBase<FieldModel>()

    let updateModifiers (field: FieldModel) scope modifiers  =
        { field with 
            Scope = scope
            StaticOrInstance = modifiers.StaticOrInstance }
        
    override _.EmptyItem() =  FieldModel.Create name typeName
    // KAD-Chet: This is goofy
    override _.InternalCombine cls cls2 = cls

    [<CustomOperation("Public", MaintainsVariableSpace = true)>]
    member _.publicWithModifiers (field: FieldModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        // KAD-Don: Is there a better way to upcast the members of an array or list?
        let modifiers = Modifiers.Evaluate (modifiers.OfType<IModifierWord>())
        updateModifiers field Public modifiers

    [<CustomOperation("Private", MaintainsVariableSpace = true)>]
    member _.privateWithModifiers (field: FieldModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        let modifiers = Modifiers.Evaluate (modifiers.OfType<IModifierWord>())
        updateModifiers field Private modifiers

    [<CustomOperation("Internal", MaintainsVariableSpace = true)>]
    member _.internalWithModifiers (field: FieldModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        let modifiers = Modifiers.Evaluate (modifiers.OfType<IModifierWord>())
        updateModifiers field Internal modifiers

    [<CustomOperation("Protected", MaintainsVariableSpace = true)>]
    member _.protectedWithModifiers (field: FieldModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        let modifiers = Modifiers.Evaluate (modifiers.OfType<IModifierWord>())
        updateModifiers field Protected modifiers


type Method(name: NamedItem, returnType: ReturnType) =
    inherit StatementBuilderBase<MethodModel>()

    let updateModifiers (method: MethodModel) scope modifiers =
        { method with 
            Scope = scope; 
            StaticOrInstance = modifiers.StaticOrInstance
            IsAsync = modifiers.IsAsync }
 
    override _.EmptyItem() : MethodModel =  MethodModel.Create name returnType
    override _.InternalCombine (method: MethodModel) (method2: MethodModel) =
        { method with Statements =  List.append method.Statements method2.Statements }
    member this.Yield (statement: IStatement) : MethodModel = 
        { this.Zero() with Statements = [ statement ] }

    // KAD-Don: When Quote is in the base class, expressions instead of result classes are returned. And when it is in Class
    member _.Quote() = ()

    //member this.Yield (()) : MethodModel = this.EmptyItem()
    //member this.Yield (statement: IStatement) : MethodModel = 
    //    { this.Zero() with Statements = [statement] }
    //member this.Combine (method: MethodModel, method2: MethodModel) : MethodModel=
    //    this.InternalCombine method method2
    //member this.Zero() : MethodModel = this.Yield(())
    //member _.Quote() = ()
    //member _.Delay(f: unit -> MethodModel) : MethodModel = f()
    //member this.For(methods, f) :MethodModel = 
    //    let methodList = Seq.toList methods
    //    match methodList with 
    //    | [] -> this.Zero()
    //    | [x] -> f(x)
    //    | head::tail ->
    //        let mutable headResult = f(head)
    //        for x in tail do 
    //            headResult <- this.Combine(headResult, f(x))
    //        headResult

    [<CustomOperation("Public", MaintainsVariableSpace = true)>]
    member _.publicWithModifiers (method: MethodModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        // KAD-Don: Is there a better way to upcast the members of an array or list?
        // Try making Evaluate generic with 'T when IModifierWord and array and then pass array
        let modifiers = Modifiers.Evaluate (modifiers.OfType<IModifierWord>())
        updateModifiers method Public modifiers

    [<CustomOperation("Private", MaintainsVariableSpace = true)>]
    member _.privateWithModifiers (method: MethodModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        let modifiers = Modifiers.Evaluate (modifiers.OfType<IModifierWord>())
        updateModifiers method Private modifiers

    [<CustomOperation("Internal", MaintainsVariableSpace = true)>]
    member _.internalWithModifiers (method: MethodModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        let modifiers = Modifiers.Evaluate (modifiers.OfType<IModifierWord>())
        updateModifiers method Internal modifiers

    [<CustomOperation("Protected", MaintainsVariableSpace = true)>]
    member _.protectedWithModifiers (method: MethodModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        let modifiers = Modifiers.Evaluate (modifiers.OfType<IModifierWord>())
        updateModifiers method Protected modifiers

    // KAD-Chet: I do notunderstand how adding the statements here and in combine don't double them
    [<CustomOperation("Return", MaintainsVariableSpace = true)>]
    member _.addReturn (method: MethodModel) =
        { method with Statements =  List.append method.Statements [ {ReturnModel.Expression = None} ] }

    [<CustomOperation("Return", MaintainsVariableSpace = true)>]
    member _.addReturn (method: MethodModel, expression: IExpression) =
        { method with Statements =  List.append method.Statements [ {ReturnModel.Expression = Some expression} ] }


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
