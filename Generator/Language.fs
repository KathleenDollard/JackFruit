module rec Generator.Language

open System.Text
open GeneralUtils
open System

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
    { Item: string
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
      ReturnType: GenericNamedItem
      IsStatic: bool
      Scope: Scope
      Parameters: Parameter list
      Statements: Statement list}

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

type ILanguage =
    abstract member PublicKeyword: string with get
    abstract member PrivateKeyword: string with get
    abstract member InternalKeyword: string with get
    abstract member StaticKeyword: string with get

    // Language structure
    abstract member Using: Using -> string
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

    abstract member Invocation: Invocation -> string
    abstract member Comparison: Comparison -> string


type RoslynWriter(language: ILanguage, spacesPerIndent: int) =
    let addLines (newLines: string list) oldLines =
        let mutable retLines = oldLines
        for line in newLines do
            retLines <- line::retLines
        retLines

    member this.OutputStatement (statement: Statement) =
        match statement with 
        | If x -> this.OutputIf x
        | Assignment x -> this.OutputAssignment x
        | AssignWithDeclare x -> this.OutputAssignWithDeclare x
        | Return x -> this.OutputReturn x
        | ForEach x -> this.OutputForEach x
        | SimpleCall x -> this.OutputSimpleCall x

    member this.OutputStatements (statements: Statement list) =
        [ for statement in statements do 
            this.OutputStatement statement ]
        |> List.concat

    member this.OutputIf (ifInfo: If) : string list= 
        []
        |> addLines (language.IfOpen ifInfo) 
        |> addLines (this.OutputStatements ifInfo.Statements)
        |> addLines (language.IfClose ifInfo)

    member _.OutputAssignWithDeclare assign =
        language.AssignWithDeclare assign

    member _.OutputAssignment assignment =
        language.Assignment assignment 

    member _.OutputReturn ret =
        language.Return ret 

    member this.OutputForEach foreach =
        []
        |> addLines (language.ForEachOpen foreach) 
        |> addLines (this.OutputStatements foreach.Statements)
        |> addLines (language.ForEachClose foreach)

    member _.OutputSimpleCall simple =
        language.SimpleCall simple 

    member this.OutputMethod (method: Method) =
        []
        |> addLines (language.MethodOpen method) 
        |> addLines (this.OutputStatements method.Statements)
        |> addLines (language.MethodClose method)
        |> List.rev

    member this.OutputProperty prop =
        let isAutoProp = 
            prop.SetStatements.IsEmpty && prop.GetStatements.IsEmpty
        if isAutoProp then
            language.AutoProperty prop
        else
            []
            |> addLines (language.PropertyOpen prop)
            |> addLines (language.GetOpen prop)
            |> addLines (this.OutputStatements prop.GetStatements)
            |> addLines (language.GetClose prop)
            |> addLines (language.SetOpen prop)
            |> addLines (this.OutputStatements prop.SetStatements)
            |> addLines (language.SetClose prop)
            |> addLines (language.PropertyClose prop)
            |> List.rev

    member this.OutputMember mbr =
        match mbr with 
        | Method m -> this.OutputMethod m
        | Property p -> this.OutputProperty p

    member this.OutputMembers (members: Member list) =
        let mutable retLines = []
        for mbr in members do 
            retLines <- addLines (this.OutputMember mbr) retLines
        retLines

    member this.OutputClass cls =
        []
        |> addLines (language.ClassOpen cls)
        |> addLines (this.OutputMembers cls.Members)
        |> addLines (language.ClassClose cls)

    member this.OutputClasses (classes: Class list) =
        // KAD-Don: Are there any shortcuts to pipelining in a loop?
        let mutable retLines = []
        for cls in classes do 
            retLines <- addLines (this.OutputClass cls) retLines
        retLines

    member this.OutputUsings (usings: Using list) =
        [ for using in usings do 
            language.Using using]

    member this.Output spaces (nspace: Namespace) = 
        []
        |> addLines (language.NamespaceOpen nspace)
        |> addLines (this.OutputUsings nspace.Usings)
        |> addLines (this.OutputClasses nspace.Classes)
        |> addLines (language.NamespaceClose nspace)



