module rec Generator.Language

open System.Text
open GeneralUtils
open System
open Generator


type ILanguage =
    abstract member PublicKeyword: string with get
    abstract member PrivateKeyword: string with get
    abstract member InternalKeyword: string with get
    abstract member StaticKeyword: string with get

    // Language structure
    abstract member Using: Using -> string list
    abstract member NamespaceOpen: Namespace -> string list
    abstract member NamespaceClose: Namespace -> string list
    abstract member ClassOpen: Class -> string list
    abstract member ClassClose: Class -> string list
    abstract member MethodOpen: Method -> string list
    abstract member MethodClose: Method -> string list
    abstract member AutoProperty: Property -> string list
    abstract member PropertyOpen: Property -> string list
    abstract member PropertyClose: Property -> string list
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

    // TODO: Consider abstract
    member private this.makeMethod staticOrInstance returnType name generics isExtension parameters statements =
       Member.Method
            { MethodName = NamedItem.Create name generics
              Scope = this
              StaticOrInstance = staticOrInstance
              IsExtension = isExtension
              ReturnType = returnType
              Parameters = parameters
              Statements = statements
            }

    member private this.makeClass staticOrInstance name generics members =
       Member.Class
            { ClassName = NamedItem.Create name generics
              Scope = this
              StaticOrInstance = staticOrInstance
              Members = members
            }

    member private this.makeProperty staticOrInstance propertyType name generics getStatements setStatements =
       Member.Property
            { PropertyName =  name
              Scope = this
              StaticOrInstance = staticOrInstance
              Type = propertyType
              GetStatements = getStatements
              SetStatements = setStatements
            }

    member this.MethodOf returnType name (genericsAsStrings: string list) parameters statements = 
        let generics = NamedItem.GenericsFromStrings name genericsAsStrings
        this.makeMethod Instance returnType name generics false parameters statements

    member this.StaticMethodOf returnType name (genericsAsStrings: string list) parameters statements = 
        let generics = NamedItem.GenericsFromStrings name genericsAsStrings
        this.makeMethod Static returnType name generics false parameters statements

    member this.Method returnType name parameters statements = 
        this.makeMethod Instance returnType name [] false parameters statements

    member this.StaticMethod returnType name parameters statements = 
        this.makeMethod Static returnType name [] false parameters statements

    member this.ExtensionMethodOf returnType name (genericsAsStrings: string list) parameters statements = 
        let generics = NamedItem.GenericsFromStrings name genericsAsStrings
        this.makeMethod Static returnType name generics true parameters statements

    member this.ExtensionMethod returnType name parameters statements = 
        this.makeMethod Static returnType name [] true parameters statements

    member this.Property propertyType (name:string) getStatements setStatements = 
        this.makeProperty Instance propertyType name getStatements setStatements


    member this.ClassOf name (genericsAsStrings: string list) members = 
        let generics = NamedItem.GenericsFromStrings name genericsAsStrings
        this.makeClass Instance name generics members

    member this.StaticClassOf name (genericsAsStrings: string list) members = 
        let generics = NamedItem.GenericsFromStrings name genericsAsStrings
        this.makeClass Static name generics members 

    member this.Class name members = 
        this.makeClass Instance name [] members

    member this.StaticClass name members = 
        this.makeClass Static name [] members


type StaticOrInstance =
    | Static
    | Instance


// Where a class may be used, use NamedType, even if it will generally be an instance
type GenericNamedItem = 
    { Name: string 
      GenericTypes: GenericNamedItem list }
    static member Create name =
        { Name = name
          GenericTypes = [] }
    static member CreateOf name  (generics: string list) =
        let createGeneric name = { Name = name; GenericTypes = [] }
        { Name = name
          GenericTypes = [ for g in generics do createGeneric g ] }
        
let As name =
    { Name = name
      GenericTypes = []}
let Of = As
let AsGeneric name genericTypes =
    { Name = name
      GenericTypes = genericTypes }
let OfGeneric = AsGeneric


type NamedItem =
    | GenericNamedItem of Name: string * GenericTypes: NamedItem list
    | SimpleNamedItem of Name: string
    static member GenericsFromStrings (name: string) genericsAsStrings =
        genericsAsStrings |> List.map (fun x -> SimpleNamedItem x)
    static member Create (name: string) generics =
        match generics with 
        | [] -> SimpleNamedItem name
        | _ -> GenericNamedItem (name, generics)


type Return =
    | Void
    | Type of t: NamedItem


type Invocation =
    { Instance: NamedItem
      MethodName: string
      Arguments: Expression list}
let Invoke instanceName methodName arguments =
    Expression.Invocation
        { Instance = NamedItem.Create instanceName [] 
          MethodName = methodName
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
let Equals left right =
    { Left = left
      Right = right
      Operator = Operator.Equals }
let NotEquals left right =
    { Left = left
      Right = right
      Operator = Operator.NotEquals }    

type Expression =
    | Invocation of Invocation
    | Comparison of Comparison
    | Instantiation of Instantiation
    | StringLiteral of string
    | NonStringLiteral of string
    | Symbol of string
    | Comment of string
    | Pragma of string

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
let Assign item value =
    Statement.Assign 
        { Item = item 
          Value = value}


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
      Scope: Scope
      Parameters: Parameter list
      Statements: Statement list}
    static member CreateOfT name genericTypeNames returnType parameters statements=
        let genericTypes = genericTypeNames |> List.map (fun x -> SimpleNamedItem x)
        { MethodName = NamedItem.Create name genericTypes
          ReturnType = returnType
          StaticOrInstance = Instance
          IsExtension = false
          Scope = Public
          Parameters = parameters
          Statements = statements }
    static member Create name returnType parameters statements=
        Method.CreateOfT name [] returnType parameters statements

type Property =
    { PropertyName: string
      Type: NamedItem
      StaticOrInstance: StaticOrInstance
      Scope: Scope
      GetStatements: Statement list
      SetStatements: Statement list}

type Member =
    | Method of Method
    | Property of Property
    | Class of Class
    static member publicMethod methodName returnType parameters statements =
        Method (Method.Create methodName returnType parameters statements)
    static member publicMethodOfT methodName genericTypeNames returnType parameters statements =
        Method (Method.CreateOfT methodName genericTypeNames returnType parameters statements)

type Class = 
    { ClassName: NamedItem
      StaticOrInstance: StaticOrInstance
      Scope: Scope
      Members: Member list }

type Using = 
    { Namespace: string
      Alias: string option }
    static member Create nspace =
        { Namespace = nspace
          Alias = None }

type Namespace = 
    { NamespaceName: string
      Usings: Using list
      Classes: Class list}

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

    member this.OutputProperty prop =
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

    member this.OutputMember mbr =
        match mbr with 
        | Method m -> this.OutputMethod m
        | Property p -> this.OutputProperty p
        | Class c -> this.OutputClass c

    member this.OutputMembers (members: Member list) =
        for mbr in members do 
            this.OutputMember mbr

    member this.OutputClass cls =
        writer.AddLines (language.ClassOpen cls)
        writer.IncreaseIndent()
        this.OutputMembers cls.Members
        writer.DecreaseIndent()
        writer.AddLines (language.ClassClose cls)

    member this.OutputClasses (classes: Class list) =
        for cls in classes do 
            this.OutputClass cls

    member _.OutputUsings (usings: Using list) =
        for using in usings do 
            writer.AddLines (language.Using using)

    member this.Output (nspace: Namespace) = 
        writer.AddLines (language.NamespaceOpen nspace)
        this.OutputUsings nspace.Usings
        writer.IncreaseIndent()
        this.OutputClasses nspace.Classes
        writer.DecreaseIndent()
        writer.AddLines (language.NamespaceClose nspace)



