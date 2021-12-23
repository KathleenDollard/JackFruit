module DslCodeBuilder


open System
open DslKeywords
open Generator.Language
open Generator.LanguageStatements
open Generator.LanguageExpressions
open Common


[<AbstractClass>]
type BuilderBase<'T>() =
 
    abstract member EmptyItem: unit -> 'T
    abstract member InternalCombine: 'T -> 'T -> 'T

    member this.Yield (()) : 'T = this.EmptyItem()
    member this.Zero() : 'T = this.Yield(())
    member this.Combine (method: 'T, method2: 'T) : 'T=
        this.InternalCombine method method2
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


[<AbstractClass>]
type StatementBuilderBase<'T when 'T :> IStatementContainer<'T>>() =
    inherit BuilderBase<'T>()

    // KAD-Chet: I do notunderstand how adding the statements here and in combine don't double them
    [<CustomOperation("Return", MaintainsVariableSpace = true)>]
    member _.addReturn (method: MethodModel) =
        method.AddStatements [ {ReturnModel.Expression = None} ] 

    [<CustomOperation("Return", MaintainsVariableSpace = true)>]
    member _.addReturn (method: MethodModel, expression: obj) =
        let expr: IExpression = 
            match expression with 
            | :? IExpression as x -> x
            | :? string as x -> StringLiteralModel.Create x
            | _ -> NonStringLiteralModel.Create (expression.ToString())

        method.AddStatements [ {ReturnModel.Expression = Some expr} ] 

    [<CustomOperation("SimpleCall", MaintainsVariableSpace = true)>]
    member _.addSimpleCall (method: MethodModel, expression: obj) =
        let expr: IExpression = 
            match expression with 
            | :? IExpression as x -> x
            | :? string as x -> StringLiteralModel.Create x
            | _ -> NonStringLiteralModel.Create (expression.ToString())

        method.AddStatements [ {SimpleCallModel.Expression = expr} ] 
 

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

    let updateModifiers (cls: ClassModel) scope (modifiers: Modifiers) =
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
        let modifiers = Modifiers.Evaluate modifiers
        updateModifiers cls Public modifiers

    [<CustomOperation("Private", MaintainsVariableSpace = true)>]
    member _.privateWithModifiers (cls: ClassModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        let modifiers = Modifiers.Evaluate modifiers
        updateModifiers cls Private modifiers

    [<CustomOperation("Internal", MaintainsVariableSpace = true)>]
    member _.internalWithModifiers (cls: ClassModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        let modifiers = Modifiers.Evaluate modifiers
        updateModifiers cls Internal modifiers

    [<CustomOperation("Protected", MaintainsVariableSpace = true)>]
    member _.protectedWithModifiers (cls: ClassModel, [<ParamArray>] modifiers: IClassModifierWord[]) =
        let modifiers = Modifiers.Evaluate modifiers
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

    [<CustomOperation("Members", MaintainsVariableSpace = true)>]
    member _.addMember (cls: ClassModel, memberModels: IMember list) =
        { cls with Members =  List.append cls.Members memberModels }
 

// KAD-Don: I have not been able to create a CE that supports an empty body. Is it possible?
//          I want to allow: 
//            Field(n, t) { }
//            Field(n, t) { Public Static }
type Field(name: string, typeName: NamedItem) =
    inherit BuilderBase<FieldModel>()

    let updateModifiers (field: FieldModel) scope (modifiers: Modifiers)  =
        { field with 
            Scope = scope
            StaticOrInstance = modifiers.StaticOrInstance }
        
    override _.EmptyItem() =  FieldModel.Create name typeName
    // KAD-Chet: This is goofy
    override _.InternalCombine cls cls2 = cls

    [<CustomOperation("Public", MaintainsVariableSpace = true)>]
    member _.publicWithModifiers (field: FieldModel, [<ParamArray>] modifiers: IFieldModifierWord[]) =
        let modifiers = Modifiers.Evaluate modifiers
        updateModifiers field Public modifiers

    [<CustomOperation("Private", MaintainsVariableSpace = true)>]
    member _.privateWithModifiers (field: FieldModel, [<ParamArray>] modifiers: IFieldModifierWord[]) =
        let modifiers = Modifiers.Evaluate modifiers
        updateModifiers field Private modifiers

    [<CustomOperation("Internal", MaintainsVariableSpace = true)>]
    member _.internalWithModifiers (field: FieldModel, [<ParamArray>] modifiers: IFieldModifierWord[]) =
        let modifiers = Modifiers.Evaluate modifiers
        updateModifiers field Internal modifiers

    [<CustomOperation("Protected", MaintainsVariableSpace = true)>]
    member _.protectedWithModifiers (field: FieldModel, [<ParamArray>] modifiers: IFieldModifierWord[]) =
        let modifiers = Modifiers.Evaluate modifiers
        updateModifiers field Protected modifiers


type Method(name: NamedItem, returnType: ReturnType) =
    inherit StatementBuilderBase<MethodModel>()

    let updateModifiers (method: MethodModel) scope (modifiers: Modifiers) =
        { method with 
            Scope = scope; 
            StaticOrInstance = modifiers.StaticOrInstance
            IsAsync = modifiers.IsAsync }
 
    override _.EmptyItem() : MethodModel =  MethodModel.Create name returnType
    override _.InternalCombine (method: MethodModel) (method2: MethodModel) =
        { method with Statements =  List.append method.Statements method2.Statements }
    member this.Yield (statement: IStatement) : MethodModel = 
        { this.Zero() with Statements = [ statement ] }

    [<CustomOperation("Public", MaintainsVariableSpace = true)>]
    member _.publicWithModifiers (method: MethodModel, [<ParamArray>] modifiers: IMethodModifierWord[]) =
        let modifiers = Modifiers.Evaluate modifiers
        updateModifiers method Public modifiers

    [<CustomOperation("Private", MaintainsVariableSpace = true)>]
    member _.privateWithModifiers (method: MethodModel, [<ParamArray>] modifiers: IMethodModifierWord[]) =
        let modifiers = Modifiers.Evaluate modifiers
        updateModifiers method Private modifiers

    [<CustomOperation("Internal", MaintainsVariableSpace = true)>]
    member _.internalWithModifiers (method: MethodModel, [<ParamArray>] modifiers: IMethodModifierWord[]) =
        let modifiers = Modifiers.Evaluate modifiers
        updateModifiers method Internal modifiers

    [<CustomOperation("Protected", MaintainsVariableSpace = true)>]
    member _.protectedWithModifiers (method: MethodModel, [<ParamArray>] modifiers: IMethodModifierWord[]) =
        let modifiers = Modifiers.Evaluate modifiers
        updateModifiers method Protected modifiers


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

