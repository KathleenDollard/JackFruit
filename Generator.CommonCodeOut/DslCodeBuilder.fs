module DslCodeBuilder

// KAD-Chet: The previous approach of individual type specific yields had a problem with 
//           multiple entries. Only the first entry appeared, which I didn't initially 
//           discover because I was just aassigning a scope/modifiers - a single line - 
//           onlyv when working with Zero(). The old Combine() ignored the data on the class
//
//           So, I removed those and added a much more complex Combine(). This works great 
//           whenever it is actually called, which does not happen on zero().

open System
open DslKeywords
open Generator.Language
open Generator.LanguageStatements
open Generator.LanguageExpressions
open Common
open Generator.LanguageExpressions.ExpressionHelpers
open Generator.LanguageHelpers
open FSharp.Core
open System.Linq

type methodLikeData =
    { Scope: Scope
      Modifiers: Modifier list
      ReturnType: ReturnType
      Parameters: ParameterModel list
      Statements: IStatement list }


// KAD-Chet: What's sane way to do tihs.
let partitionOfTypeAndRemaining<'T when 'T :> IStatement>(list: IStatement list) =
    // KAD-Chet: I could not get this to work, even with bizarre code, with list
    let sequence = List.ofSeq list
    let matches = sequence.Where(fun x -> x :? 'T)
    let remaining = sequence.Except(matches)
    (matches.OfType<'T>(), List.ofSeq remaining)
let getMethodLikeData 
    (existingData1: methodLikeData) 
    (existingData2: methodLikeData) =
    // I do not think we care which list returnType, etc came from
    let list = List.append existingData1.Statements existingData2.Statements

    let (returnTypes, remaining) = partitionOfTypeAndRemaining<ReturnType> list
    let returnType =
        let candidates = List.ofSeq (returnTypes.Where(fun x -> x <> ReturnTypeUnknown))
        match candidates with 
        // New one overwrites old, if no new one, take right, unless right is unknown
        | [] -> if existingData2.ReturnType = ReturnTypeUnknown then existingData1.ReturnType else existingData2.ReturnType
        | _ -> candidates[0] 

    let (scopeAndModifiers, remaining) = partitionOfTypeAndRemaining<ScopeAndModifiers> remaining
    let scope =
        let candidates = List.ofSeq (scopeAndModifiers.Where(fun x -> x.Scope <> Unknown))
        match candidates with 
        // New one overwrites old, if no new one, take right, unless right is unknown
        | [] -> if existingData2.Scope = Unknown then existingData1.Scope else existingData2.Scope
        | _ -> candidates[0].Scope
    let modifiers = 
        let modifierList = 
            List.ofSeq (scopeAndModifiers.Select(fun x -> x.Modifiers))
            |> List.collect (fun x -> x)
        List.append existingData1.Modifiers existingData2.Modifiers
            |> List.append modifierList
            |> List.distinct

    // Parameters should, perhaps, be their own CE. I'm worried about retaining order correctly this way
    let (newParameters, statements) = partitionOfTypeAndRemaining<ParameterModel> remaining
    let parameters =
        List.append existingData1.Parameters existingData2.Parameters
            |> List.append (List.ofSeq newParameters)

    { Scope = scope
      Modifiers = modifiers
      ReturnType = returnType
      Parameters = parameters
      Statements = statements }

[<AbstractClass>]
type BuilderBase<'T>() =
 
    abstract member EmptyItem: unit -> 'T
    abstract member InternalCombine: 'T -> 'T -> 'T

    member this.Yield (()) : 'T = this.EmptyItem()
    member this.Zero() : 'T = this.Yield(())
    member this.Combine (item: 'T, item2: 'T) : 'T=
        this.InternalCombine item item2
    member _.Delay(f: unit -> 'T) : 'T = f()
    member this.For(items, f) :'T = 
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
type BuilderBase2<'M, 'T>() =
             
    abstract member EmptyItem: unit -> 'M
    abstract member InternalCombine: 'M -> 'M -> 'M
            
    member this.Zero() : 'M = this.EmptyItem()
    member this.Combine (item: 'M, item2: 'M) : 'M=
        this.InternalCombine item item2
    member _.Delay(f) : 'M = f()
    member this.For(items, f) :'M = 
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

    member this.Yield (statement: IStatement) : 'T = 
        this.Zero().AddStatements [ statement ]


type If(condition: ICompareExpression) =
    inherit StatementBuilderBase<IfModel>()

    override _.EmptyItem() : IfModel =  IfModel.Create condition []
    override _.InternalCombine if1 if2 : IfModel =
        if1.AddStatements if2.Statements

type ElseIf(condition: ICompareExpression) =
    inherit StatementBuilderBase<ElseIfModel>()

    override _.EmptyItem() : ElseIfModel =  ElseIfModel.Create condition []
    override _.InternalCombine if1 if2 : ElseIfModel =
        if1.AddStatements if2.Statements

type Else() =
    inherit StatementBuilderBase<ElseModel>()

    override _.EmptyItem() : ElseModel =  ElseModel.Create []
    override _.InternalCombine if1 if2 : ElseModel =
        if1.AddStatements if2.ElseStatements



type Namespace(name: string) =
    inherit BuilderBase2<NamespaceModel, ClassModel>()

    override _.EmptyItem() : NamespaceModel =  NamespaceModel.Create name
    override _.InternalCombine nspace nspace2 : NamespaceModel =
        { nspace with 
            Usings =  List.append nspace.Usings nspace2.Usings
            Classes = List.append nspace.Classes nspace2.Classes }  

    member this.Yield () : NamespaceModel = this.Zero()
    member this.Yield (_: unit) : NamespaceModel = this.Zero()
    member this.Yield (usingModel: UsingModel) : NamespaceModel = 
        this.Zero().AddUsings [usingModel]
    member this.Yield (classModel: ClassModel) : NamespaceModel = 
        this.Zero().AddClasses [classModel]
    member this.Return(classModel) = this.Zero().AddUsings classModel
  

type Class(name: string) =
    inherit BuilderBase<ClassModel>()

    let updateModifiers (cls: ClassModel) scope (modifiers: Modifier[]) =
        { cls with 
            Scope = scope; 
            Modifiers = List.ofArray modifiers
       }

    override _.EmptyItem() =  ClassModel.Create name
    override _.InternalCombine cls1 cls2 =
        let newScope = 
            match (cls1.Scope, cls2.Scope) with
            | (Unknown, Unknown) -> Unknown
            | (scope, Unknown) -> scope
            | (Unknown, scope) -> scope
            | (_, scope2) -> scope2
        let newInheritedFrom =
            match (cls1.InheritedFrom, cls2.InheritedFrom) with
            | (NoBase, NoBase) -> NoBase
            | (SomeBase baseClass, NoBase) -> SomeBase baseClass
            | (NoBase, SomeBase baseClass) -> SomeBase baseClass
            | (_, SomeBase baseClass) -> SomeBase baseClass

        { cls1 with 
            Scope = newScope
            Modifiers = List.append cls1.Modifiers cls2.Modifiers
            InheritedFrom = newInheritedFrom
            ImplementedInterfaces = List.append cls1.ImplementedInterfaces cls2.ImplementedInterfaces
            Members =  List.append cls1.Members cls2.Members }  
    member this.Yield (memberModel: IMember) : ClassModel = 
        { this.Zero() with Members = [ memberModel ] }
    member this.Yield (modifiers: ScopeAndModifiers) : ClassModel = 
        { this.Zero() with Scope = modifiers.Scope; Modifiers = modifiers.Modifiers }
    member this.Yield (inheritedFrom: InheritedFrom) : ClassModel = 
        { this.Zero() with InheritedFrom = inheritedFrom }
    member this.Yield (implementedInterfaces: ImplementedInterface list) : ClassModel = 
        { this.Zero() with ImplementedInterfaces = implementedInterfaces }

    //[<CustomOperation("Public", MaintainsVariableSpace = true)>]
    //member _.publicWithModifiers (cls: ClassModel, [<ParamArray>] modifiers: Modifier[]) =
    //    updateModifiers cls Public modifiers

    //[<CustomOperation("Private", MaintainsVariableSpace = true)>]
    //member _.privateWithModifiers (cls: ClassModel, [<ParamArray>] modifiers: Modifier[]) =
    //    updateModifiers cls Private modifiers

    //[<CustomOperation("Internal", MaintainsVariableSpace = true)>]
    //member _.internalWithModifiers (cls: ClassModel, [<ParamArray>] modifiers: Modifier[]) =
    //    updateModifiers cls Internal modifiers

    //[<CustomOperation("Protected", MaintainsVariableSpace = true)>]
    //member _.protectedWithModifiers (cls: ClassModel, [<ParamArray>] modifiers: Modifier[]) =
    //    updateModifiers cls Protected modifiers

    //// TODO: Passing a named item will be quite messy. This problemw will recur. Harder if we can't get overloads
    //[<CustomOperation("InheritedFrom", MaintainsVariableSpace = true)>]
    //member _.inheritedFrom (cls: ClassModel, inheritedFrom: NamedItem) =
    //    // Consider whether resetting this to None is a valid scenario
    //    { cls with InheritedFrom = SomeBase inheritedFrom }

    //// TODO: Passing a named item will be quite messy. This problemw will recur. Harder if we can't get overloads
    //[<CustomOperation("ImplementedInterfaces", MaintainsVariableSpace = true)>]
    //member _.interfaces (cls: ClassModel, [<ParamArray>]interfaces: ImplementedInterface[]) =
    //    { cls with ImplementedInterfaces = List.ofArray interfaces }

    //[<CustomOperation("Generics", MaintainsVariableSpace = true)>]
    //member _.generics (cls: ClassModel, generics: NamedItem list) =
    //    let currentName =
    //        match cls.ClassName with 
    //        | SimpleNamedItem n -> n
    //        | GenericNamedItem (n, _ ) -> n
    //    { cls with ClassName = NamedItem.Create currentName generics }

    //[<CustomOperation("Members", MaintainsVariableSpace = true)>]
    //member _.addMember (cls: ClassModel, memberModels: IMember list) =
    //    { cls with Members =  List.append cls.Members memberModels }
 

// Don: I have not been able to create a CE that supports an empty body. Is it possible?
//          I want to allow: 
//            Field(n, t) { }
//            Field(n, t) { Public Static }
type Field(name: string, typeName: NamedItem) =
    inherit BuilderBase<FieldModel>()

    let updateModifiers (field: FieldModel) scope (modifiers: Modifier[])  =
        { field with 
            Scope = scope
            Modifiers = List.ofArray modifiers }
        
    override _.EmptyItem() =  FieldModel.Create name typeName
    // KAD: Clean this up. Either put a default in the base or combine the modifier comparison as elsewhere
    override _.InternalCombine cls cls2 = cls

    [<CustomOperation("Public", MaintainsVariableSpace = true)>]
    member _.publicWithModifiers (field: FieldModel, [<ParamArray>] modifiers: Modifier[]) =
        updateModifiers field Public modifiers

    [<CustomOperation("Private", MaintainsVariableSpace = true)>]
    member _.privateWithModifiers (field: FieldModel, [<ParamArray>] modifiers: Modifier[]) =
        updateModifiers field Private modifiers

    [<CustomOperation("Internal", MaintainsVariableSpace = true)>]
    member _.internalWithModifiers (field: FieldModel, [<ParamArray>] modifiers: Modifier[]) =
        updateModifiers field Internal modifiers

    [<CustomOperation("Protected", MaintainsVariableSpace = true)>]
    member _.protectedWithModifiers (field: FieldModel, [<ParamArray>] modifiers: Modifier[]) =
        updateModifiers field Protected modifiers

[< AbstractClass >]
type MethodBase<'T when 'T :> IMethodLike<'T>>() =
    inherit StatementBuilderBase<'T>()

    //member this.Yield (modifiers: ScopeAndModifiers) : 'T = 
    //    this.Zero().AddScopeAndModifiers modifiers
    //member this.Yield (parameter: ParameterModel) : 'T = 
    //    this.Zero().AddParameter parameter


    //[<CustomOperation("Public", MaintainsVariableSpace = true)>]
    //member _.publicWithModifiers (item: 'T, [<ParamArray>] modifiers: Modifier[]) =
    //    (item :> IMethodLike<'T>).AddScopeAndModifiers Public (List.ofArray modifiers)

    //[<CustomOperation("Private", MaintainsVariableSpace = true)>]
    //member _.privateWithModifiers (item: 'T, [<ParamArray>] modifiers: Modifier[]) =
    //     (item :> IMethodLike<'T>).AddScopeAndModifiers Private (List.ofArray modifiers)

    //[<CustomOperation("Internal", MaintainsVariableSpace = true)>]
    //member _.internalWithModifiers (item: 'T, [<ParamArray>] modifiers: Modifier[]) =
    //     (item :> IMethodLike<'T>).AddScopeAndModifiers Internal (List.ofArray modifiers)

    //[<CustomOperation("Protected", MaintainsVariableSpace = true)>]
    //member _.protectedWithModifiers (item: 'T, [<ParamArray>] modifiers: Modifier[]) =
    //     (item :> IMethodLike<'T>).AddScopeAndModifiers Protected (List.ofArray modifiers)

    //[<CustomOperation("Parameter", MaintainsVariableSpace = true)>]
    //member _.addParmaeter (item: 'T, parameterName: string, parameterType: NamedItem) =
    //    (item :> IMethodLike<'T>).AddParameter parameterName parameterType Normal


type Method (name: NamedItem) =
    inherit MethodBase<MethodModel>()

    override _.EmptyItem (): MethodModel =  MethodModel.Create (name, Void)
    override _.InternalCombine (method1: MethodModel) (method2: MethodModel) =
        let data =
            getMethodLikeData 
                { Scope = method1.Scope 
                  Modifiers = method1.Modifiers
                  ReturnType = method1.ReturnType
                  Parameters = method1.Parameters
                  Statements = method1.Statements }
                { Scope = method2.Scope 
                  Modifiers = method2.Modifiers
                  ReturnType = method2.ReturnType
                  Parameters = method2.Parameters
                  Statements = method2.Statements }

        { method1 with 
            Scope = data.Scope
            Modifiers = data.Modifiers
            ReturnType = data.ReturnType
            Parameters = data.Parameters
            Statements = data.Statements }
  


type Property(name: string, typeName: NamedItem) =

    let updateModifiers (property: PropertyModel) scope (modifiers: Modifier[]) =
        { property with 
            Scope = scope
            Modifiers = List.ofArray modifiers }        
        
    member _.Yield (_) = PropertyModel.Create name typeName
    member this.Zero() = this.Yield()

    [<CustomOperation("Public", MaintainsVariableSpace = true)>]
    member _.modifiers (property: PropertyModel, [<ParamArray>] modifiers: Modifier[]) =
        updateModifiers property Public modifiers


    [<CustomOperation("Private", MaintainsVariableSpace = true)>]
    member _.privateWithModifiers (property: PropertyModel, [<ParamArray>] modifiers: Modifier[]) =
        updateModifiers property Public modifiers

    [<CustomOperation("Internal", MaintainsVariableSpace = true)>]
    member _.internalWithModifiers (property: PropertyModel, [<ParamArray>] modifiers: Modifier[]) =
        updateModifiers property Public modifiers

    [<CustomOperation("Protected", MaintainsVariableSpace = true)>]
    member _.protectedWithModifiers (property: PropertyModel, [<ParamArray>] modifiers: Modifier[]) =
        updateModifiers property Public modifiers


type Constructor() =
    inherit MethodBase<ConstructorModel>()
        
    override _.EmptyItem (): ConstructorModel =  ConstructorModel.Create()
    override _.InternalCombine (ctor1: ConstructorModel) (ctor2: ConstructorModel) =
        let data =
            getMethodLikeData 
                { Scope = ctor1.Scope 
                  Modifiers = ctor1.Modifiers
                  ReturnType = ReturnTypeUnknown
                  Parameters = ctor1.Parameters
                  Statements = ctor1.Statements }
                { Scope = ctor2.Scope 
                  Modifiers = ctor2.Modifiers
                  ReturnType = ReturnTypeUnknown
                  Parameters = ctor2.Parameters
                  Statements = ctor2.Statements }

        { ctor1 with 
            Scope = data.Scope
            Modifiers = data.Modifiers
            Parameters = data.Parameters
            Statements = data.Statements }


