module DslForCode

open Dsl
open Generator.LanguageModel
open Common
open System
open DslKeywords
open Generator.LanguageStatements
open Generator.LanguageExpressions
open Generator.LanguageHelpers


[<AbstractClass>]
type StatementBuilderBase<'T>() =
    inherit DslBase<'T, IStatement>()

[< AbstractClass >]
type MethodBase<'T>() =
    inherit StatementBuilderBase<'T>()

    member _.NewParameter parameterName parameterType style =
        { ParameterName = parameterName
          Type = parameterType
          Style = 
            match style with 
            | Some s -> s
            | None -> Normal }

type Namespace(name: string) =
    inherit DslBase<NamespaceModel, ClassModel>()

    override _.Empty() : NamespaceModel =  NamespaceModel.Create name

    override this.NewMember item =
        { this.Empty() with Classes = [ item ] }

    override _.CombineModels nspace nspace2 : NamespaceModel =
        //let newName = 
        //    if String.IsNullOrWhiteSpace nspace.NamespaceName then nspace2.NamespaceName
        //    else nspace.NamespaceName
        { nspace with 
            NamespaceName = nspace.NamespaceName // always set to this value
            Usings =  List.append nspace.Usings nspace2.Usings
            Classes = List.append nspace.Classes nspace2.Classes }  

    //[<CustomOperation("Name", MaintainsVariableSpaceUsingBind = true)>]
    //member this.setName (varModel, [<ProjectionParameter>] name)   =
    //    this.SetModel varModel { varModel.Model with NamespaceName = name varModel.Variables }

    [<CustomOperation("Using", MaintainsVariableSpaceUsingBind = true)>]
    member this.setUsing (varModel, [<ProjectionParameter>] using)   =
        let value: string = using varModel.Variables
        let newUsing = UsingModel.Create(value)
        this.SetModel varModel { varModel.Model with Usings = varModel.Model.Usings @ [ newUsing ] }

    // KAD-Don: I had a problem when I had the projection attribute on both parameters. Does it need to be on 
    //          the first or does it need to be on a common parameter, and by implication, is a common parameter required
    [<CustomOperation("Using", MaintainsVariableSpaceUsingBind = true)>]
    member this.setUsing (varModel, [<ProjectionParameter>] using, alias)   =
        let usingName: string = using varModel.Variables
        //let aliasValue: string = alias varModel.Variables
        let aliasValue = alias
        let alias = 
            if String.IsNullOrWhiteSpace aliasValue then None
            else Some aliasValue
        let newUsing = { UsingNamespace = usingName; Alias = alias }
        this.SetModel varModel { varModel.Model with Usings = varModel.Model.Usings @ [ newUsing ] }


type Class(name: string) =
    inherit DslBase<ClassModel, IMember>()

    member private this.setScope varModel scope modifier1 modifier2 =
        this.SetModel varModel 
            { varModel.Model with 
                Scope = scope
                Modifiers = varModel.Model.Modifiers @ Modifier.CombineModifiers modifier1 modifier2 }

    override _.Empty() =  ClassModel.Create name

    override this.NewMember item =
        { this.Empty() with Members = [ item ] }

    override _.CombineModels cls1 cls2 =
        //let newName = 
        //    match cls1.ClassName with
        //    | SimpleNamedItem n when String.IsNullOrWhiteSpace(n) -> cls2.ClassName
        //    | _ -> cls1.ClassName
        let newScope = 
            match cls1.Scope with 
            | Unknown -> cls2.Scope
            | _ -> cls1.Scope  
        let newInheritedFrom =
            match (cls1.InheritedFrom, cls2.InheritedFrom) with
            | (NoBase, NoBase) -> NoBase
            | (SomeBase baseClass, NoBase) -> SomeBase baseClass
            | (NoBase, SomeBase baseClass) -> SomeBase baseClass
            | (_, SomeBase baseClass) -> SomeBase baseClass
        { cls1 with 
            ClassName = cls1.ClassName   // Always set to this
            Scope = newScope
            Modifiers = List.append cls1.Modifiers cls2.Modifiers
            InheritedFrom = newInheritedFrom
            ImplementedInterfaces = List.append cls1.ImplementedInterfaces cls2.ImplementedInterfaces
            Members =  List.append cls1.Members cls2.Members }  

    [<CustomOperation("Public2", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPublic(varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.setScope varModel Scope.Public modifier1 modifier2

    [<CustomOperation("Private", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPrivate (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.setScope varModel Scope.Private modifier1 modifier2

    [<CustomOperation("Internal", MaintainsVariableSpaceUsingBind = true)>]
    member this.setInternal (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.setScope varModel Scope.Internal modifier1 modifier2

    [<CustomOperation("Friend", MaintainsVariableSpaceUsingBind = true)>]
    member this.setFriend (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.setScope varModel Scope.Internal modifier1 modifier2

    [<CustomOperation("Protected", MaintainsVariableSpaceUsingBind = true)>]
    member this.setProtected (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.setScope varModel Scope.Protected modifier1 modifier2

    [<CustomOperation("ProtectedInternal", MaintainsVariableSpaceUsingBind = true)>]
    member this.setProtectedInternal (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.setScope varModel Scope.ProtectedInternal modifier1 modifier2

    [<CustomOperation("PrivateProtected", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPrivateProtected (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.setScope varModel Scope.PrivateProtected modifier1 modifier2

    [<CustomOperation("InheritedFrom", MaintainsVariableSpaceUsingBind = true)>]
    member this.inheritedFrom (varModel, [<ProjectionParameter>] inheritedFrom) =
        let inheritedFrom: string = inheritedFrom varModel.Variables
        this.SetModel varModel { varModel.Model with InheritedFrom = SomeBase inheritedFrom }

    [<CustomOperation("ImplementsInterface", MaintainsVariableSpaceUsingBind = true)>]
    member this.interfaces (varModel, [<ProjectionParameter>] interfaceToImplement) =
        let implement = interfaceToImplement varModel.Variables
        this.SetModel varModel { varModel.Model with ImplementedInterfaces = varModel.Model.ImplementedInterfaces @ [implement] }

    //[<CustomOperation("Members", MaintainsVariableSpaceUsingBind = true)>]
    //member this.addMember (varModel, memberModels: IMember list) =
    //    this.SetModel varModel { cls with Members =  List.append cls.Members memberModels }
 

type Field(name: string, typeName: NamedItem) =
    inherit DslBase<IMember, unit>()

     member private this.SetScopeAndModifiers 
            (varModel: M<IMember, 'Vars0>) 
            (scope: Scope) 
            (modifier1: Modifier option) 
            (modifier2: Modifier option) =
        let fld = varModel.Model :?> FieldModel 
        let newField: FieldModel = 
            { fld with 
                Scope = scope
                Modifiers = fld.Modifiers @ Modifier.CombineModifiers modifier1 modifier2 }
        this.SetModel varModel newField


    override _.Empty() =  FieldModel.Create name typeName

    override _.CombineModels mbr1 mbr2 =
        let fld1 = mbr1 :?> FieldModel
        let fld2 = mbr2 :?> FieldModel
        //let newName = 
        //    if String.IsNullOrWhiteSpace(fld1.FieldName) then fld1.FieldName
        //    else fld2.FieldName
        //let newType = 
        //    match fld1.FieldType with
        //    | SimpleNamedItem n when String.IsNullOrWhiteSpace(n) -> fld2.FieldType
        //    | _ -> fld1.FieldType
        let newInitialValue =
            match fld1.InitialValue with
            | None -> fld2.InitialValue
            | _ -> fld1.InitialValue
        let newScope = 
            match fld1.Scope with 
            | Unknown -> fld2.Scope
            | _ -> fld1.Scope  
        { fld1 with 
            FieldName = fld1.FieldName //Always set to this
            FieldType = fld1.FieldType //Always set to thi
            Scope = newScope
            Modifiers = List.append fld1.Modifiers fld2.Modifiers
            InitialValue = newInitialValue }  
 
    override _.NewMember item = invalidOp "What called this?"

    [<CustomOperation("Public2", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPublic(varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.Public modifier1 modifier2
    
    [<CustomOperation("Private", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPrivate (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.Private modifier1 modifier2
    
    [<CustomOperation("Internal", MaintainsVariableSpaceUsingBind = true)>]
    member this.setInternal (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.Internal modifier1 modifier2
    
    [<CustomOperation("Friend", MaintainsVariableSpaceUsingBind = true)>]
    member this.setFriend (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.Internal modifier1 modifier2
    
    [<CustomOperation("Protected", MaintainsVariableSpaceUsingBind = true)>]
    member this.setProtected (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.Protected modifier1 modifier2
    
    [<CustomOperation("ProtectedInternal", MaintainsVariableSpaceUsingBind = true)>]
    member this.setProtectedInternal (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.ProtectedInternal modifier1 modifier2
    
    [<CustomOperation("PrivateProtected", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPrivateProtected (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.PrivateProtected modifier1 modifier2

    [<CustomOperation("InitialValue", MaintainsVariableSpace = true)>]
    member this.initialValue ((varModel: M<IMember, 'Vars0>), [<ProjectionParameter>] initialValue) =
        let initialValue: IExpression = initialValue varModel.Variables
        let fld = varModel.Model :?> FieldModel 
        this.SetModel varModel { fld with InitialValue = Some initialValue }
 
type Property(name: string, typeName: NamedItem) =
    inherit DslBase<IMember, PropertyAccessorModel>()

    member private this.SetScopeAndModifiers 
            (varModel: M<IMember, 'Vars0>) 
            (scope: Scope) 
            (modifier1: Modifier option) 
            (modifier2: Modifier option) =
        let prop = varModel.Model :?> PropertyModel 
        let newProperty: PropertyModel = 
            { prop with 
                Scope = scope
                Modifiers = prop.Modifiers @ Modifier.CombineModifiers modifier1 modifier2 }
        this.SetModel varModel newProperty

    override _.Empty() =  PropertyModel.Create name typeName

    override _.CombineModels mbr1 mbr2 = 
        let combineAccessors (acc1: PropertyAccessorModel option) (acc2: PropertyAccessorModel option) = 
            match acc1, acc2 with
            | None, None -> None
            | Some x , None -> Some x
            | None, Some x -> Some x
            | Some a1, Some a2 -> 
                let scope =
                    match a1.Scope, a2.Scope with 
                    | Unknown, Unknown -> Unknown
                    | Unknown, s -> s
                    | s, Unknown -> s
                    | s, _ -> s
                Some 
                    { PropertyAccessorModel.Scope = scope
                      AccessorType = a1.AccessorType
                      Statements = a1.Statements @ a2.Statements }

        let prop1 = mbr1 :?> PropertyModel
        let prop2 = mbr2 :?> PropertyModel
        let newScope = 
            match prop1.Scope with 
            | Unknown -> prop2.Scope
            | _ -> prop1.Scope  
        { prop1 with 
            PropertyName = prop1.PropertyName // Always set via constructor
            Type = prop1.Type // Always set via constructor
            Scope = newScope
            Modifiers = List.append prop1.Modifiers prop2.Modifiers
            Getter = combineAccessors prop1.Getter prop2.Getter
            Setter = combineAccessors prop1.Setter prop2.Setter }

    // Need work an Set and Get and initialize before this works
    override this.NewMember item = 
        let property = this.Empty() :?> PropertyModel
        if item.AccessorType = Getter then  
            { property with Getter = Some item }
        else
            { property with Setter = Some item }

    [<CustomOperation("Public2", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPublic(varModel: M<IMember, 'Vars0>, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.Public modifier1 modifier2

    [<CustomOperation("Private", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPrivate (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
         this.SetScopeAndModifiers varModel Scope.Private modifier1 modifier2
  
    [<CustomOperation("Internal", MaintainsVariableSpaceUsingBind = true)>]
    member this.setInternal (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
         this.SetScopeAndModifiers varModel Scope.Internal modifier1 modifier2
  
    [<CustomOperation("Friend", MaintainsVariableSpaceUsingBind = true)>]
    member this.setFriend (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
         this.SetScopeAndModifiers varModel Scope.Internal modifier1 modifier2
  
    [<CustomOperation("Protected", MaintainsVariableSpaceUsingBind = true)>]
    member this.setProtected (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
         this.SetScopeAndModifiers varModel Scope.Protected modifier1 modifier2
  
    [<CustomOperation("ProtectedInternal", MaintainsVariableSpaceUsingBind = true)>]
    member this.setProtectedInternal (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
         this.SetScopeAndModifiers varModel Scope.ProtectedInternal modifier1 modifier2
  
    [<CustomOperation("PrivateProtected", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPrivateProtected (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
         this.SetScopeAndModifiers varModel Scope.PrivateProtected modifier1 modifier2

type Get() =
    inherit StatementBuilderBase<PropertyAccessorModel>()

    member private this.SetScope 
           (varModel: M<PropertyAccessorModel, 'Vars0>) 
           (scope: Scope) =
       let getter = varModel.Model
       let newGetter: PropertyAccessorModel = 
           { getter with Scope = scope }
       this.SetModel varModel newGetter

    override _.Empty() =  PropertyAccessorModel.Create(Getter)

    override _.CombineModels acc1 acc2 = 
        let newScope = 
            match acc1.Scope with 
            | Unknown -> acc2.Scope
            | _ -> acc1.Scope  
        { acc1 with 
            Scope = newScope
            Statements = acc1.Statements @ acc2.Statements}

    override this.NewMember item =
        let method = this.Empty() 
        { method with Statements = [ item ] }

    [<CustomOperation("Public2", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPublic(varModel: M<PropertyAccessorModel, 'Vars0>, ?modifier1: Modifier, ?modifier2: Modifier) =
       this.SetScope varModel Scope.Public

    [<CustomOperation("Private", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPrivate (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScope varModel Scope.Private
 
    [<CustomOperation("Internal", MaintainsVariableSpaceUsingBind = true)>]
    member this.setInternal (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScope varModel Scope.Internal
 
    [<CustomOperation("Friend", MaintainsVariableSpaceUsingBind = true)>]
    member this.setFriend (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScope varModel Scope.Internal
 
    [<CustomOperation("Protected", MaintainsVariableSpaceUsingBind = true)>]
    member this.setProtected (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScope varModel Scope.Protected
 
    [<CustomOperation("ProtectedInternal", MaintainsVariableSpaceUsingBind = true)>]
    member this.setProtectedInternal (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScope varModel Scope.ProtectedInternal
 
    [<CustomOperation("PrivateProtected", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPrivateProtected (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScope varModel Scope.PrivateProtected

type Method(name: string) =
    inherit MethodBase<IMember>()

    member private this.SetScopeAndModifiers 
           (varModel: M<IMember, 'Vars0>) 
           (scope: Scope) 
           (modifier1: Modifier option) 
           (modifier2: Modifier option) =
       let method = varModel.Model :?> MethodModel 
       let newMethod: MethodModel = 
           { method with 
               Scope = scope
               Modifiers = method.Modifiers @ Modifier.CombineModifiers modifier1 modifier2 }
       this.SetModel varModel newMethod

    override _.Empty() =  MethodModel.Create name

    override _.CombineModels mbr1 mbr2 = 
        let method1 = mbr1 :?> MethodModel
        let method2 = mbr2 :?> MethodModel
        //let newName = 
        //    match method1.MethodName with
        //    | SimpleNamedItem n when String.IsNullOrWhiteSpace(n) -> method2.MethodName
        //    | _ -> method1.MethodName
        let newReturnType = 
            match method1.ReturnType with
            | ReturnTypeUnknown -> method2.ReturnType
            | _ -> method1.ReturnType
        let newScope = 
            match method1.Scope with 
            | Unknown -> method2.Scope
            | _ -> method1.Scope  
        { method1 with 
            MethodName = method1.MethodName // Always set in constructor
            ReturnType = newReturnType
            Scope = newScope
            Modifiers = method1.Modifiers@  method2.Modifiers
            Parameters = method1.Parameters @ method2.Parameters
            Statements = method1.Statements @ method2.Statements}  

    override this.NewMember item =
        let method = this.Empty() :?> MethodModel
        { method with Statements = [ item ] }

    [<CustomOperation("Public2", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPublic(varModel: M<IMember, 'Vars0>, ?modifier1: Modifier, ?modifier2: Modifier) =
       this.SetScopeAndModifiers varModel Scope.Public modifier1 modifier2

    [<CustomOperation("Private", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPrivate (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.Private modifier1 modifier2
 
    [<CustomOperation("Internal", MaintainsVariableSpaceUsingBind = true)>]
    member this.setInternal (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.Internal modifier1 modifier2
 
    [<CustomOperation("Friend", MaintainsVariableSpaceUsingBind = true)>]
    member this.setFriend (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.Internal modifier1 modifier2
 
    [<CustomOperation("Protected", MaintainsVariableSpaceUsingBind = true)>]
    member this.setProtected (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.Protected modifier1 modifier2
 
    [<CustomOperation("ProtectedInternal", MaintainsVariableSpaceUsingBind = true)>]
    member this.setProtectedInternal (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.ProtectedInternal modifier1 modifier2
 
    [<CustomOperation("PrivateProtected", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPrivateProtected (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.PrivateProtected modifier1 modifier2

    [<CustomOperation("ReturnType", MaintainsVariableSpaceUsingBind = true)>]
    member this.setReturnType (varModel: M<IMember, 'Vars0>, [<ProjectionParameter>] returnType) =
        let returnType: ReturnType = returnType varModel.Variables
        let method = varModel.Model :?> MethodModel 
        //let returnType = ReturnType.Create returnType
        this.SetModel varModel { method with ReturnType = returnType }

    [<CustomOperation("Parameter", MaintainsVariableSpaceUsingBind = true)>]
    member this.setParameter (varModel: M<IMember, 'Vars0>, [<ProjectionParameter>] parameterName, parameterType, (?style: ParameterStyle))   =
        let parameterName: string = parameterName varModel.Variables
        let method = varModel.Model :?> MethodModel 
        let newParameter = this.NewParameter parameterName parameterType style
        this.SetModel varModel (method.AddParameter newParameter)
       
        
type Constructor() =
    inherit MethodBase<IMember>()

    member private this.SetScopeAndModifiers 
           (varModel: M<IMember, 'Vars0>) 
           (scope: Scope) 
           (modifier1: Modifier option) 
           (modifier2: Modifier option) =
       let ctor = varModel.Model :?> ConstructorModel 
       let newConstructor: ConstructorModel = 
           { ctor with 
               Scope = scope
               Modifiers = ctor.Modifiers @ Modifier.CombineModifiers modifier1 modifier2 }
       this.SetModel varModel newConstructor


    override _.Empty() =  ConstructorModel.Create()
    
    override _.CombineModels mbr1 mbr2 = 
        let method1 = mbr1 :?> ConstructorModel
        let method2 = mbr2 :?> ConstructorModel
        let newScope = 
            match method1.Scope with 
            | Unknown -> method2.Scope
            | _ -> method1.Scope  
        { method1 with 
            Scope = newScope
            Modifiers = method1.Modifiers@  method2.Modifiers
            Parameters = method1.Parameters @ method2.Parameters
            Statements = method1.Statements @ method2.Statements }  

    override this.NewMember item =
        let method = this.Empty() :?> ConstructorModel
        { method with Statements = [ item ] }

    [<CustomOperation("Public2", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPublic(varModel: M<IMember, 'Vars0>, ?modifier1: Modifier, ?modifier2: Modifier) =
       this.SetScopeAndModifiers varModel Scope.Public modifier1 modifier2

    [<CustomOperation("Private", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPrivate (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.Private modifier1 modifier2
 
    [<CustomOperation("Internal", MaintainsVariableSpaceUsingBind = true)>]
    member this.setInternal (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.Internal modifier1 modifier2
 
    [<CustomOperation("Friend", MaintainsVariableSpaceUsingBind = true)>]
    member this.setFriend (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.Internal modifier1 modifier2
 
    [<CustomOperation("Protected", MaintainsVariableSpaceUsingBind = true)>]
    member this.setProtected (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.Protected modifier1 modifier2
 
    [<CustomOperation("ProtectedInternal", MaintainsVariableSpaceUsingBind = true)>]
    member this.setProtectedInternal (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.ProtectedInternal modifier1 modifier2
 
    [<CustomOperation("PrivateProtected", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPrivateProtected (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetScopeAndModifiers varModel Scope.PrivateProtected modifier1 modifier2

    [<CustomOperation("Parameter", MaintainsVariableSpaceUsingBind = true)>]
    member this.setParameter ((varModel: M<IMember, 'Vars0>), [<ProjectionParameter>] parameterName, parameterType, (?style: ParameterStyle))   =
        let parameterName: string = parameterName varModel.Variables
        let method = varModel.Model :?> ConstructorModel 
        let newParameter = this.NewParameter parameterName parameterType style
        this.SetModel varModel (method.AddParameter newParameter)
  
    [<CustomOperation("Base", MaintainsVariableSpaceUsingBind = true)>]
    member this.setBase ((varModel: M<IMember, 'Vars0>), (arguments: IExpression list))   =
        let ctor = varModel.Model :?> ConstructorModel 
        this.SetModel varModel (ctor.AddBaseOrThis Base arguments)
  
    [<CustomOperation("This", MaintainsVariableSpaceUsingBind = true)>]
    member this.seThis ((varModel: M<IMember, 'Vars0>), (arguments: IExpression list))   =
        let ctor = varModel.Model :?> ConstructorModel 
        this.SetModel varModel (ctor.AddBaseOrThis This arguments)


type If(condition: ICompareExpression) =
    inherit StatementBuilderBase<IStatement>()

    override _.Empty() =  IfModel.Create condition []

    override _.CombineModels mbr1 mbr2 =
        let if1 = mbr1 :?> IfModel
        let if2 = mbr2 :?> IfModel
        if1.AddStatements if2.Statements

    override this.NewMember item =
        let if1 = this.Empty() :?> IfModel
        { if1 with Statements = [ item ] }

type ElseIf(condition: ICompareExpression) =
    inherit StatementBuilderBase<IStatement>()

    override _.Empty() =  ElseIfModel.Create condition []

    override _.CombineModels mbr1 mbr2 =
        let elseif1 = mbr1 :?> ElseIfModel
        let elseif2 = mbr2 :?> ElseIfModel
        elseif1.AddStatements elseif2.Statements

    override this.NewMember item =
        let method = this.Empty() :?> ElseIfModel
        { method with Statements = [ item ] }


type Else() =
    inherit StatementBuilderBase<IStatement>()

    override _.Empty() =  ElseModel.Create []

    override _.CombineModels mbr1 mbr2 =
        let else1 = mbr1 :?> ElseModel
        let else2= mbr2 :?> ElseModel
        else1.AddStatements else2.ElseStatements

    override this.NewMember item =
        let else1 = this.Empty() :?> ElseModel
        { else1 with ElseStatements = [ item ] }

