module rec Generator.Language

open Generator
open Common


type ILanguage =

    // Language structure
    abstract member Using: UsingModel -> string list
    abstract member NamespaceOpen: NamespaceModel -> string list
    abstract member NamespaceClose: NamespaceModel -> string list
    abstract member ClassOpen: ClassModel -> string list
    abstract member ClassClose: ClassModel -> string list
    abstract member ConstructorOpen: ConstructorModel -> string list
    abstract member ConstructorClose: ConstructorModel -> string list
    abstract member MethodOpen: MethodModel -> string list
    abstract member MethodClose: MethodModel -> string list
    abstract member AutoProperty: PropertyModel -> string list
    abstract member PropertyOpen: PropertyModel -> string list
    abstract member PropertyClose: PropertyModel -> string list
    abstract member Field: FieldModel -> string list
    abstract member GetOpen: PropertyModel-> string list
    abstract member GetClose: PropertyModel -> string list
    abstract member SetOpen: PropertyModel -> string list
    abstract member SetClose: PropertyModel -> string list

    // Statements
    abstract member IfOpen: IfModel -> string list
    abstract member IfClose: IfModel -> string list
    abstract member ForEachOpen: ForEachModel -> string list
    abstract member ForEachClose: ForEachModel -> string list

    abstract member Assignment: AssignmentModel -> string list
    abstract member AssignWithDeclare: AssignWithDeclareModel -> string list
    abstract member Return: ExpressionModel -> string list
    abstract member SimpleCall: ExpressionModel -> string list
    abstract member Comment: ExpressionModel -> string list
    abstract member Pragma: ExpressionModel -> string list

    abstract member Invocation: InvocationModel -> string
    abstract member Comparison: ComparisonModel -> string

    // Other
    abstract member NamedItemOutput: NamedItem -> string

type Scope =
    | Public
    | Private
    | Internal
    | Protected

type StaticOrInstance =
    | Static
    | Instance

// Where a class may be used, use NamedType, even if it will generally be an instance
// KAD: Consider if all these overloads are needed
type GenericNamedItem = 
    { Name: string 
      GenericTypes: NamedItem list }
    static member Create name =
        { Name = name 
          GenericTypes = [] }
let As name =
    { Name = name
      GenericTypes = []}
let Of = As
let AsGeneric name genericTypes =
    { Name = name
      GenericTypes = genericTypes }
let OfGeneric = AsGeneric


type InvocationModel =
    { Instance: NamedItem // Named item for invoking static methods on generic types
      MethodName: NamedItem // For generic methods
      ShouldAwait: bool
      Arguments: ExpressionModel list}
let Invoke instanceName methodName arguments =
    ExpressionModel.Invocation
        { Instance = SimpleNamedItem instanceName
          MethodName = methodName
          ShouldAwait = false
          Arguments = arguments } 
let With (arguments: ExpressionModel list) = arguments 

type InstantiationModel =
    { TypeName: NamedItem
      Arguments: ExpressionModel list}
let New typeName arguments = 
    ExpressionModel.Instantiation 
        { TypeName = NamedItem.Create typeName []
          Arguments = arguments }
let NewGeneric typeName genericName arguments =
    ExpressionModel.Instantiation
        { TypeName = NamedItem.Create typeName [ NamedItem.Create genericName [] ]
          Arguments = arguments }

type Operator =
    | Equals
    | NotEquals
    | GreaterThan
    | LessThan
    | GreaterThanOrEqualTo
    | LessThanOrEqualTo

type ComparisonModel =
    { Left: ExpressionModel
      Right: ExpressionModel
      Operator: Operator}
    //static member Equals left right =
    //    { Left = left
    //      Right = right
    //      Operator = Operator.Equals }
    //static member NotEquals left right =
    //    { Left = left
    //      Right = right
    //      Operator = Operator.NotEquals }  

type ExpressionModel =
    | Invocation of InvocationModel
    | Comparison of ComparisonModel
    | Instantiation of InstantiationModel
    | StringLiteral of string
    | NonStringLiteral of string
    | Symbol of string
    | Comment of string
    | Pragma of string
    | Null
    static member Compare left operator right =
        Comparison
            { Left = left
              Right = right
              Operator = operator }
    static member NotEquals left right =
        { Left = left
          Right = right
          Operator = Operator.NotEquals }  

type IfModel =
    { Condition: ExpressionModel
      Statements: StatementModel list
      Elses: IfModel list}

type ForEachModel =
    { LoopVar: string
      LoopOver: string
      Statements: StatementModel list }

type AssignmentModel = 
    { Item: string
      Value: ExpressionModel}

type AssignWithDeclareModel =
    { Variable: string
      TypeName: NamedItem option
      Value: ExpressionModel}
let AssignVar variable value =
    StatementModel.AssignWithDeclare 
        { Variable = variable
          Value = value 
          TypeName = None }

type StatementModel =
    | If of IfModel
    | Assign of AssignmentModel
    | AssignWithDeclare of AssignWithDeclareModel
    | ForEach of ForEachModel
    | Return of ExpressionModel
    | SimpleCall of ExpressionModel
    static member Invoke instanceName methodName arguments =
        SimpleCall 
            ( Invocation
                { Instance = SimpleNamedItem instanceName
                  MethodName = methodName
                  ShouldAwait = false
                  Arguments = arguments } )

type ParameterModel =
    { ParameterName: string
      Type: NamedItem
      Default: ExpressionModel option
      IsParams: bool}
    static member Create name paramType =
        { ParameterName = name
          Type = paramType
          Default = None
          IsParams = false }


type IMember = interface end

type MethodModel =
    { MethodName: NamedItem
      ReturnType: Return
      StaticOrInstance: StaticOrInstance
      IsExtension: bool
      IsAsync: bool
      Scope: Scope
      Parameters: ParameterModel list
      Statements: StatementModel list}
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
 

type ConstructorModel =
    { ClassName: string
      StaticOrInstance: StaticOrInstance
      Scope: Scope
      Parameters: ParameterModel list
      Statements: StatementModel list}
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
      GetStatements: StatementModel list
      SetStatements: StatementModel list}
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
      InitialValue: ExpressionModel option}
    static member Create fieldName fieldType =
      { FieldName = fieldName
        FieldType = fieldType
        IsReadonly = false
        StaticOrInstance = Instance
        Scope = Public
        InitialValue = None }
    interface IMember
      

type Member =
    | Method of MethodModel
    | Property of PropertyModel
    | Field of FieldModel
    | Constructor of ConstructorModel
    | Class of ClassModel

type ClassModel = 
    { ClassName: NamedItem
      Scope: Scope
      StaticOrInstance: StaticOrInstance
      IsAsync: bool
      IsPartial: bool
      InheritedFrom: NamedItem option
      ImplementedInterfaces: NamedItem list
      Members: IMember list}
    static member Create(className: NamedItem, scope: Scope, members: IMember list) =
        { ClassName = className
          Scope = scope
          StaticOrInstance = Instance
          IsAsync = false
          IsPartial = false
          InheritedFrom = None
          ImplementedInterfaces = []
          Members = members }
    static member Create(className: string, scope: Scope, members: IMember list) =
        ClassModel.Create((SimpleNamedItem className), scope, members)
    static member Create(className: string) =
        ClassModel.Create((SimpleNamedItem className), Public, [])
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
          Classes = [] }    member this.AddUsings (usings: UsingModel list) =
        { this with Usings = List.append this.Usings usings }
    static member Default() = NamespaceModel.Create ""
    member this.AddClasses (classes: ClassModel list) =
        { this with Classes = List.append this.Classes classes }


type CodeBlock =
    | NamespaceBlock
    | MethodBlock
    | FunctionBlock
    | IfBlock
    | ForBlock


type RoslynOut(language: ILanguage, writer: IWriter) =

    //let addLines (newLines: string list) oldLines =
    //    let mutable retLines = oldLines
    //    for line in newLines do
    //        retLines <- line::retLines
    //    retLines

    member _.BlankLine() =
        writer.AddLine ""

    member this.OutputStatement (statement: StatementModel) =
        match statement with 
        | If x -> this.OutputIf x
        | Assign x -> this.OutputAssignment x
        | AssignWithDeclare x -> this.OutputAssignWithDeclare x
        | Return x -> this.OutputReturn x
        | ForEach x -> this.OutputForEach x
        | SimpleCall x -> this.OutputSimpleCall x

    member this.OutputStatements (statements: StatementModel list) =
        for statement in statements do 
            this.OutputStatement statement

    member this.OutputIf (ifInfo: IfModel) = 
        writer.AddLines (language.IfOpen ifInfo)
        writer.IncreaseIndent()
        this.OutputStatements ifInfo.Statements
        writer.DecreaseIndent()
        writer.AddLines (language.IfClose ifInfo)

    member _.OutputAssignWithDeclare assign =
        writer.AddLines (language.AssignWithDeclare assign)

    member _.OutputAssignment assignment =
        writer.AddLines (language.Assignment assignment)

    member _.OutputReturn ret =
        writer.AddLines (language.Return ret)

    member this.OutputForEach foreach :unit =
        writer.AddLines (language.ForEachOpen foreach) 
        writer.IncreaseIndent()
        this.OutputStatements foreach.Statements
        writer.DecreaseIndent()
        writer.AddLines (language.ForEachClose foreach)

    member _.OutputSimpleCall simple =
        writer.AddLines (language.SimpleCall simple)

    member _.OutputComment comment =
        writer.AddLines (language.Comment comment)

    member _.OutputPragma pragma =
        writer.AddLines (language.Pragma pragma)

    member this.OutputMethod (method: MethodModel) =
        writer.AddLines (language.MethodOpen method) 
        writer.IncreaseIndent()
        this.OutputStatements method.Statements
        writer.DecreaseIndent()
        writer.AddLines (language.MethodClose method)

    member this.OutputConstructor (ctor: ConstructorModel) =
       writer.AddLines (language.ConstructorOpen ctor) 
       writer.IncreaseIndent()
       this.OutputStatements ctor.Statements
       writer.DecreaseIndent()
       writer.AddLines (language.ConstructorClose ctor)

    member this.OutputProperty (prop : PropertyModel) =
        let isAutoProp = 
            prop.SetStatements.IsEmpty && prop.GetStatements.IsEmpty
        if isAutoProp then
            writer.AddLines (language.AutoProperty prop)
        else
            writer.AddLines (language.PropertyOpen prop)
            writer.IncreaseIndent()
            writer.AddLines (language.GetOpen prop)
            writer.IncreaseIndent()
            this.OutputStatements prop.GetStatements
            writer.DecreaseIndent()
            writer.AddLines (language.GetClose prop)
            writer.AddLines (language.SetOpen prop)
            writer.IncreaseIndent()
            this.OutputStatements prop.SetStatements
            writer.DecreaseIndent()
            writer.AddLines (language.SetClose prop)
            writer.DecreaseIndent()
            writer.AddLines (language.PropertyClose prop)

    member this.OutputField (field: FieldModel) =
        writer.AddLines (language.Field field)

     member this.OutputMember mbr =
        match mbr with 
        | Method m -> this.OutputMethod m
        | Property p -> this.OutputProperty p
        | Class c -> this.OutputClass c
        | Field f -> this.OutputField f
        | Constructor c -> this.OutputConstructor c

    member this.OutputMembers (members: IMember list) =
        for mbr in members do 
            ()
            //this.OutputMember mbr

    member this.OutputClass cls =
        writer.AddLines (language.ClassOpen cls)
        writer.IncreaseIndent()
        this.OutputMembers cls.Members
        writer.DecreaseIndent()
        writer.AddLines (language.ClassClose cls)

    member this.OutputClasses (classes: ClassModel list) =
        for cls in classes do 
            this.OutputClass cls

    member _.OutputUsings (usings: UsingModel list) =
        for using in usings do 
            writer.AddLines (language.Using using)

    member this.Output (nspace: NamespaceModel) = 
        this.OutputUsings nspace.Usings
        writer.AddLines (language.NamespaceOpen nspace)
        writer.IncreaseIndent()
        this.OutputClasses nspace.Classes
        writer.DecreaseIndent()
        writer.AddLines (language.NamespaceClose nspace)
        writer


