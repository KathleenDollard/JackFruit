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

type Statement =
    | If of Expression
    | Assignment of Assignment
    | Simple of Expression
    | ForEach of ForEach

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
    abstract member PropertyOpen: Property -> string list
    abstract member PropertyClose: Property -> string list
    abstract member GetOpen: Property-> string list
    abstract member GetClose: Property -> string list
    abstract member SetOpen: Property -> string list
    abstract member SetClose: Property -> string list
    abstract member IfOpen: If -> string list
    abstract member IfClose: If -> string list
    abstract member ForEachOpen: ForEach -> string list
    abstract member ForEachClose: ForEach -> string list
    abstract member Assignment: Assignment -> string

    // Expressions
    abstract member Invocation: Invocation -> string
    abstract member Comparison: Comparison -> string


type RoslynWriter(language: ILanguage, spacesPerIndent: int) =
    let addLines (newLines: string list) oldLines =
        let mutable retLines = oldLines
        for line in newLines do
            retLines <- line::retLines
        retLines

    let ScopeOutput scope =
        match scope with 
        | Public -> language.PublicKeyword
        | Private -> language.PrivateKeyword
        | Internal ->  language.InternalKeyword

    let StaticOutput isStatic = 
        if isStatic then
            " " + language.StaticKeyword
        else
            ""

    let OutputStatement (statement: Statement) =
        ()

    let OutputStatements (statements: Statement list) =
        [ for statement in statements do 
            OutputStatement statement ]
            

    let OutputMethod (method: Method) =
        []
        |> addLines (language.MethodOpen method) 
        |> addLines (OutputStatements method.Statements)
        |> addLines (language.MethodClose method)

    member this.OutputProperty  mbr =
        []

    member this.OutputMember mbr =
        match mbr with 
        | Method m -> OutputMethod m
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



