module rec Generator.Language

open Generator
open Common


type IMember = interface end
type IStatement = interface end
type IExpression = interface end
type ICompareExpression = 
    interface 
    inherit IExpression
    end
type IStatementContainer<'T> = 
    abstract member AddStatements: IStatement list -> 'T


type ParameterModel =
    { ParameterName: string
      Type: NamedItem
      Default: IExpression option
      IsParams: bool}
    static member Create name paramType =
        { ParameterName = name
          Type = paramType
          Default = None
          IsParams = false }


type MethodModel =
    { MethodName: NamedItem
      ReturnType: ReturnType
      Scope: Scope
      StaticOrInstance: StaticOrInstance
      IsAsync: bool
      IsExtension: bool
      Parameters: ParameterModel list
      Statements: IStatement list}
    static member Create methodName returnType =
        { MethodName = methodName
          ReturnType = returnType
          StaticOrInstance = Instance
          IsExtension = false
          IsAsync = false
          Scope = Public
          Parameters = []
          Statements = [] }
    interface IMember
    member this.AddStatements statements =
        { this with Statements = statements }
    interface IStatementContainer<MethodModel> with
        member this.AddStatements statements = this.AddStatements statements
     
 
type ConstructorModel =
    { ClassName: string
      StaticOrInstance: StaticOrInstance
      Scope: Scope
      Parameters: ParameterModel list
      Statements: IStatement list}
    static member Create className =
        { ClassName =  className
          StaticOrInstance = Instance
          Scope = Public
          Parameters = []
          Statements = [] }
    interface IMember


type PropertyModel =
    { PropertyName: string
      Type: NamedItem
      StaticOrInstance: StaticOrInstance
      Scope: Scope
      GetStatements: IStatement list
      SetStatements: IStatement list}
    static member Create propertyName propertyType =
      { PropertyName = propertyName
        Type = propertyType
        StaticOrInstance = Instance
        Scope = Public
        GetStatements = []
        SetStatements = [] }
    interface IMember


type FieldModel =
    { FieldName: string
      FieldType: NamedItem
      IsReadonly: bool
      StaticOrInstance: StaticOrInstance
      Scope: Scope
      InitialValue: IExpression option}
    static member Create fieldName fieldType =
      { FieldName = fieldName
        FieldType = fieldType
        IsReadonly = false
        StaticOrInstance = Instance
        Scope = Private
        InitialValue = None }
    interface IMember
      

//type Member =
//    | Method of MethodModel
//    | Property of PropertyModel
//    | Field of FieldModel
//    | Constructor of ConstructorModel
//    | Class of ClassModel


type ClassModel = 
    { ClassName: NamedItem
      Scope: Scope
      StaticOrInstance: StaticOrInstance
      IsAbstract: bool
      IsAsync: bool
      IsPartial: bool
      IsSealed: bool
      InheritedFrom: NamedItem option
      ImplementedInterfaces: NamedItem list
      Members: IMember list}
    static member Create(className, scope) =
        { ClassName = className
          Scope = scope
          StaticOrInstance = Instance
          IsAbstract = false
          IsAsync = false
          IsPartial = false
          IsSealed = false
          InheritedFrom = None
          ImplementedInterfaces = []
          Members = [] }
    //static member Create(className: string, scope: Scope, members: IMember list) =
    //    ClassModel.Create((SimpleNamedItem className), scope, members)
    static member Create(className) =
        ClassModel.Create((SimpleNamedItem className), Public)
    interface IMember


type UsingModel = 
    { Namespace: string
      Alias: string option }
    static member Create nspace =
        { Namespace = nspace
          Alias = None }


type NamespaceModel = 
    { NamespaceName: string
      Usings: UsingModel list
      Classes: ClassModel list}
    static member Create(name: string) =
        { NamespaceName = name
          Usings = []
          Classes = [] }    
    member this.AddUsings (usings: UsingModel list) =
        { this with Usings = List.append this.Usings usings }
    member this.AddClasses (classes: ClassModel list) =
        { this with Classes = List.append this.Classes classes }



