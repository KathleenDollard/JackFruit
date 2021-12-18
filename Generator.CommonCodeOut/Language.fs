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
    abstract member ConstructorOpen: Constructor -> string list
    abstract member ConstructorClose: Constructor -> string list
    abstract member MethodOpen: Method -> string list
    abstract member MethodClose: Method -> string list
    abstract member AutoProperty: Property -> string list
    abstract member PropertyOpen: Property -> string list
    abstract member PropertyClose: Property -> string list
    abstract member Field: Field -> string list
    abstract member GetOpen: Property-> string list
    abstract member GetClose: Property -> string list
    abstract member SetOpen: Property -> string list
    abstract member SetClose: Property -> string list

    // Statements
    abstract member IfOpen: If -> string list
    abstract member IfClose: If -> string list
    abstract member ForEachOpen: ForEach -> string list
    abstract member ForEachClose: ForEach -> string list

    abstract member Assignment: Assignment -> string list
    abstract member AssignWithDeclare: AssignWithDeclare -> string list
    abstract member Return: Expression -> string list
    abstract member SimpleCall: Expression -> string list
    abstract member Comment: Expression -> string list
    abstract member Pragma: Expression -> string list

    abstract member Invocation: Invocation -> string
    abstract member Comparison: Comparison -> string

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


type Invocation =
    { Instance: NamedItem // Named item for invoking static methods on generic types
      MethodName: NamedItem // For generic methods
      ShouldAwait: bool
      Arguments: Expression list}
let Invoke instanceName methodName arguments =
    Expression.Invocation
        { Instance = SimpleNamedItem instanceName
          MethodName = methodName
          ShouldAwait = false
          Arguments = arguments } 
let With (arguments: Expression list) = arguments 

type Instantiation =
    { TypeName: NamedItem
      Arguments: Expression list}
let New typeName arguments = 
    Expression.Instantiation 
        { TypeName = NamedItem.Create typeName []
          Arguments = arguments }
let NewGeneric typeName genericName arguments =
    Expression.Instantiation
        { TypeName = NamedItem.Create typeName [ NamedItem.Create genericName [] ]
          Arguments = arguments }

type Operator =
    | Equals
    | NotEquals
    | GreaterThan
    | LessThan
    | GreaterThanOrEqualTo
    | LessThanOrEqualTo

type Comparison =
    { Left: Expression
      Right: Expression
      Operator: Operator}
    //static member Equals left right =
    //    { Left = left
    //      Right = right
    //      Operator = Operator.Equals }
    //static member NotEquals left right =
    //    { Left = left
    //      Right = right
    //      Operator = Operator.NotEquals }  

type Expression =
    | Invocation of Invocation
    | Comparison of Comparison
    | Instantiation of Instantiation
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

type If =
    { Condition: Expression
      Statements: Statement list
      Elses: If list}

type ForEach =
    { LoopVar: string
      LoopOver: string
      Statements: Statement list }

type Assignment = 
    { Item: string
      Value: Expression}

type AssignWithDeclare =
    { Variable: string
      TypeName: NamedItem option
      Value: Expression}
let AssignVar variable value =
    Statement.AssignWithDeclare 
        { Variable = variable
          Value = value 
          TypeName = None }

type Statement =
    | If of If
    | Assign of Assignment
    | AssignWithDeclare of AssignWithDeclare
    | ForEach of ForEach
    | Return of Expression
    | SimpleCall of Expression
    static member Invoke instanceName methodName arguments =
        SimpleCall 
            ( Invocation
                { Instance = SimpleNamedItem instanceName
                  MethodName = methodName
                  ShouldAwait = false
                  Arguments = arguments } )

type Parameter =
    { ParameterName: string
      Type: NamedItem
      Default: Expression option
      IsParams: bool}
    static member Create name paramType =
        { ParameterName = name
          Type = paramType
          Default = None
          IsParams = false }

type Method =
    { MethodName: NamedItem
      ReturnType: Return
      StaticOrInstance: StaticOrInstance
      IsExtension: bool
      IsAsync: bool
      Scope: Scope
      Parameters: Parameter list
      Statements: Statement list}
    static member Create name returnType =
        { MethodName = NamedItem.Create name []
          ReturnType = returnType
          StaticOrInstance = Instance
          IsExtension = false
          IsAsync = false
          Scope = Public
          Parameters = []
          Statements = [] }


type Constructor =
    { ClassName: NamedItem
      StaticOrInstance: StaticOrInstance
      Scope: Scope
      Parameters: Parameter list
      Statements: Statement list}
    static member Create className =
        { ClassName = SimpleNamedItem className
          StaticOrInstance = Instance
          Scope = Public
          Parameters = []
          Statements = [] }

type Property =
    { PropertyName: string
      Type: NamedItem
      StaticOrInstance: StaticOrInstance
      Scope: Scope
      GetStatements: Statement list
      SetStatements: Statement list}

type Field =
    { FieldName: string
      FieldType: NamedItem
      IsReadonly: bool
      StaticOrInstance: StaticOrInstance
      Scope: Scope
      InitialValue: Expression option}
      

type Member =
    | Method of Method
    | Property of Property
    | Field of Field
    | Constructor of Constructor
    | Class of ClassModel

type ClassModel = 
    { ClassName: NamedItem
      Scope: Scope
      StaticOrInstance: StaticOrInstance
      IsAsync: bool
      IsPartial: bool
      InheritedFrom: NamedItem option
      ImplementedInterfaces: NamedItem list
      Members: Member list}
    static member Create(className: NamedItem, scope: Scope, members: Member list) =
        { ClassName = className
          Scope = scope
          StaticOrInstance = Instance
          IsAsync = false
          IsPartial = false
          InheritedFrom = None
          ImplementedInterfaces = []
          Members = members }
    static member Create(className: string, scope: Scope, members: Member list) =
        ClassModel.Create((SimpleNamedItem className), scope, members)


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
    static member Default() =
        { NamespaceName = ""
          Usings = []
          Classes = [] }
    member this.AddUsings (usings: UsingModel list) =
        { this with Usings = List.append this.Usings usings }
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

    member this.OutputStatement (statement: Statement) =
        match statement with 
        | If x -> this.OutputIf x
        | Assign x -> this.OutputAssignment x
        | AssignWithDeclare x -> this.OutputAssignWithDeclare x
        | Return x -> this.OutputReturn x
        | ForEach x -> this.OutputForEach x
        | SimpleCall x -> this.OutputSimpleCall x

    member this.OutputStatements (statements: Statement list) =
        for statement in statements do 
            this.OutputStatement statement

    member this.OutputIf (ifInfo: If) = 
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

    member this.OutputMethod (method: Method) =
        writer.AddLines (language.MethodOpen method) 
        writer.IncreaseIndent()
        this.OutputStatements method.Statements
        writer.DecreaseIndent()
        writer.AddLines (language.MethodClose method)

    member this.OutputConstructor (ctor: Constructor) =
       writer.AddLines (language.ConstructorOpen ctor) 
       writer.IncreaseIndent()
       this.OutputStatements ctor.Statements
       writer.DecreaseIndent()
       writer.AddLines (language.ConstructorClose ctor)

    member this.OutputProperty (prop : Property) =
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

    member this.OutputField (field: Field) =
        writer.AddLines (language.Field field)

     member this.OutputMember mbr =
        match mbr with 
        | Method m -> this.OutputMethod m
        | Property p -> this.OutputProperty p
        | Class c -> this.OutputClass c
        | Field f -> this.OutputField f
        | Constructor c -> this.OutputConstructor c

    member this.OutputMembers (members: Member list) =
        for mbr in members do 
            this.OutputMember mbr

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


