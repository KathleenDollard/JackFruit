module rec Generator.LanguageModel

open Generator
open Common
open DslKeywords
open Generator.GeneralUtils
open System

type Modifier =
    | Static
    | Async
    | Extension
    | Partial
    | Abstract
    | Sealed
    | Readonly
    static member Contains modifier list =
        List.exists (fun x -> x = modifier) list
    static member CombineModifiers (modifier1: Modifier option) (modifier2: Modifier option) =
        [ match modifier1 with | Some m -> m | None -> ()
          match modifier2 with | Some m -> m | None -> () ]        



        
type ParameterStyle =
    | Normal
    | DefaultValue of IExpression
    | IsParamArray



type IMember = interface end
type IStatementLike = interface end
type IStatement = interface end
type IExpression = interface end
type IScoped = 
    abstract member Scope: Scope

type ICompareExpression = 
    inherit IExpression

type IStatementContainer<'T> = 
    abstract member AddStatements: IStatement list -> 'T


type IMethodLike<'T when 'T:> IMethodLike<'T>> =
    inherit IStatementContainer<'T>
    abstract member AddScopeAndModifiers: ScopeAndModifiers -> 'T
    abstract member AddParameter: ParameterModel-> 'T
    abstract member AddReturnType: ReturnType -> 'T // no op for constructors and setters
   
type ParameterModel =
    { ParameterName: string
      Type: NamedItem
      Style: ParameterStyle }
    //interface IStatement
    static member Create name paramType =
        { ParameterName = name
          Type = paramType
          Style = Normal }


type ReturnType =
    | ReturnTypeVoid
    | ReturnTypeUnknown
    | ReturnType of t: NamedItem
    //interface IStatement
    static member Create typeName =
        match typeName with 
            | "void" -> ReturnTypeVoid
            | _ -> ReturnType (NamedItem.Create typeName)
    static member op_Implicit(typeName: string) : ReturnType = 
        ReturnType.Create typeName


type MethodModel =
    { MethodName: NamedItem
      ReturnType: ReturnType
      Scope: Scope
      Modifiers: Modifier list
      Parameters: ParameterModel list
      Statements: IStatement list}
    static member Create (methodName, returnType) =
        { MethodName = methodName
          ReturnType = returnType
          Scope = Public
          Modifiers = []
          Parameters = []
          Statements = [] }
    static member Create methodName  =
        MethodModel.Create (methodName, ReturnTypeUnknown)
    interface IMember
    member this.AddStatements statements =
        { this with Statements = statements }
    member this.AddScopeAndModifiers (scopeAndModifiers: ScopeAndModifiers) = 
        { this with Scope = scopeAndModifiers.Scope; Modifiers = scopeAndModifiers.Modifiers }
    member this.AddParameter parameterModel = 
        { this with Parameters = List.append this.Parameters [parameterModel] }
    member this.AddReturnType (returnType: ReturnType) = 
        { this with ReturnType = returnType }
    interface IMethodLike<MethodModel> with
        member this.AddStatements statements = this.AddStatements statements
        member this.AddScopeAndModifiers scopeAndModifiers = this.AddScopeAndModifiers scopeAndModifiers
        member this.AddParameter parameterModel = 
            this.AddParameter parameterModel
        member this.AddReturnType returnType = 
            this.AddReturnType returnType
     
 
type ConstructorModel =
    { Scope: Scope
      Modifiers: Modifier list
      Parameters: ParameterModel list
      Statements: IStatement list}
    static member Create() =
        { Scope = Public
          Modifiers = []
          Parameters = []
          Statements = [] }
    // TODO: Remove the following overload when it won't break too much
    static member Create (className) =
        { Scope = Public
          Modifiers = []
          Parameters = []
          Statements = [] }
    interface IMember
    member this.AddStatements statements =
        { this with Statements = statements }
    member this.AddScopeAndModifiers (scopeAndModifiers: ScopeAndModifiers) = 
        { this with Scope = scopeAndModifiers.Scope; Modifiers = scopeAndModifiers.Modifiers }
    member this.AddParameter parameterModel = 
        { this with Parameters = List.append this.Parameters [parameterModel] }
    interface IMethodLike<ConstructorModel> with
        member this.AddStatements statements = this.AddStatements statements
        member this.AddScopeAndModifiers scopeAndModifiers = this.AddScopeAndModifiers scopeAndModifiers
        member this.AddParameter parameterModel = 
            this.AddParameter parameterModel
        member this.AddReturnType returnType = 
            this  // no op


type PropertyModel =
    { PropertyName: string
      Type: NamedItem
      Scope: Scope
      Modifiers: Modifier list
      GetStatements: IStatement list
      SetStatements: IStatement list}
    static member Create propertyName propertyType =
        { PropertyName = propertyName
          Type = propertyType
          Scope = Public
          Modifiers = []
          GetStatements = []
          SetStatements = [] }
    interface IMember
    member this.AddScopeAndModifiers (scopeAndModifiers: ScopeAndModifiers) = 
        { this with Scope = scopeAndModifiers.Scope; Modifiers = scopeAndModifiers.Modifiers }


type FieldModel =
    { FieldName: string
      FieldType: NamedItem
      Scope: Scope
      Modifiers: Modifier list
      InitialValue: IExpression option}
    static member Create fieldName fieldType =
        { FieldName = fieldName
          FieldType = fieldType
          Scope = Private
          Modifiers = []
          InitialValue = None }
    interface IMember


type InheritedFrom =
    | SomeBase of BaseClass: NamedItem
    | NoBase
    //interface IMember

//type ImplementedInterface =
//    | ImplementedInterface of Name: NamedItem
//    //interface IMember
        
 
type ClassModel = 
    { ClassName: NamedItem
      Scope: Scope
      Modifiers: Modifier list
      InheritedFrom: InheritedFrom
      ImplementedInterfaces: NamedItem list
      Members: IMember list}
    static member Create(className, scope) =
        { ClassName = className
          Scope = scope
          Modifiers = []
          InheritedFrom = NoBase
          ImplementedInterfaces = []
          Members = [] }
    static member Create(className) =
        ClassModel.Create((NamedItem.Create className), Unknown)
    interface IMember


type UsingModel = 
    { UsingNamespace: string
      Alias: string option }
    static member Create nspace =
        { UsingNamespace = nspace
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
        // Ignore usings with empty strings. They would be an error later
        let newUsings =
            [ for u in usings do
                if not (String.IsNullOrWhiteSpace u.UsingNamespace) then u ]
        { this with Usings = List.append this.Usings newUsings }
    member this.AddClasses (classes: ClassModel list) =
        { this with Classes = List.append this.Classes classes }


type ScopeAndModifiers =
    { Scope: Scope
      Modifiers: Modifier list }
    //interface IMember
    //interface IStatement

type System.String with 
    member this.AsFieldName() =
        ToCamel(this)
    member this.AsParamName() =
        ToCamel(this)
        
