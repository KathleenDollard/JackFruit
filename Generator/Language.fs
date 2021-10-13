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

type Scope =
    | Public
    | Private
    | Internal

// Where a class may be used, use NamedType, even if it will generally be an instance
type GenericNamedItem = 
    { Name: string 
      GenericTypes: GenericNamedItem list }
    static member Create name =
        { Name = name 
          GenericTypes = [] }


type Invocation =
    { Instance: GenericNamedItem
      MethodName: string
      Arguments: Expression list}

type Instantiation =
    { TypeName: GenericNamedItem
      Arguments: Expression list}

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

type AssignWithDeclare =
    { VariableName: string
      TypeName: GenericNamedItem option
      Value: Expression}

type Statement =
    | If of If
    | Assignment of Assignment
    | AssignWithDeclare of AssignWithDeclare
    | ForEach of ForEach
    | Return of Expression
    | SimpleCall of Expression

type Parameter =
    { Name: string
      Type: GenericNamedItem
      Default: Expression option
      IsParams: bool}

type Method =
    { Name: GenericNamedItem
      ReturnType: GenericNamedItem option
      IsStatic: bool
      IsExtension: bool
      Scope: Scope
      Parameters: Parameter list
      Statements: Statement list}
    with static member Create name returnType =
        { Name = { Name = name; GenericTypes = [] }
          ReturnType = returnType
          IsStatic = false
          IsExtension = false
          Scope = Public
          Parameters = []
          Statements = [] }

type Property =
    { Name: string
      Type: GenericNamedItem
      IsStatic: bool
      Scope: Scope
      GetStatements: Statement list
      SetStatements: Statement list}

type Member =
    | Method of Method
    | Property of Property
    | Class of Class

type Class = 
    { Name: GenericNamedItem
      IsStatic: bool
      Scope: Scope
      Members: Member list}

type Using = 
    { Namespace: string
      Alias: string option }
    with static member Create nspace =
        { Namespace = nspace
          Alias = None }

type Namespace = 
    { Name: string
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
        | Assignment x -> this.OutputAssignment x
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



