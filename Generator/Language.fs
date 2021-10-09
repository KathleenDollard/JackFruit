module rec Generator.Language

open System.Text
open GeneralUtils

type Scope =
    | Public
    | Private
    | Internal

// KAD-Don: Can a type be a restriction: IOW, can I create a SymbolName guaranteed to be a valid symbol?
type NamedItem = | NamedItem of string

// Where a class may be used, use NamedType, even if it will generally be an instance
type GenericNamedItem = 
    { TypeName: NamedItem 
      GenericTypes: GenericNamedItem list }

type Invocation =
    { Instance: GenericNamedItem
      MethodName: NamedItem
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
    | NamedItem of NamedItem

type If =
    { Condition: Expression
      Statements: Statement list
      Elses: If list}

type ForEach =
    { LoopVar: NamedItem
      LoopOver: NamedItem
      Statements: Statement list }

type Assignment = 
    { Item: NamedItem
      Value: Expression}

type Statement =
    | If of Expression
    | Assignment of Assignment
    | Simple of Expression
    | ForEach of ForEach

type Parameter =
    { Name: NamedItem
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
    { Name: NamedItem
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
      Alias: string option}

type Namespace = 
    { Name: NamedItem
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


type Output(language: ILanguage, spacesPerIndent: int) =
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

    let OutputProperty  mbr =
        []

    let OutputMember mbr =
        match mbr with 
        | Method m -> OutputMethod m
        | Property p -> OutputProperty p

    let OutputMembers (members: Member list) =
        let mutable retLines = []
        for mbr in members do 
            retLines <- addLines (OutputMember mbr) retLines
        retLines

    let OutputClass cls =
        []
        |> addLines (language.ClassOpen cls)
        |> addLines (OutputMembers cls.Members)
        |> addLines (language.ClassClose cls)

    let OutputClasses (classes: Class list) =
        // KAD-Don: Are there any shortcuts to pipelining in a loop?
        let mutable retLines = []
        for cls in classes do 
            retLines <- addLines (OutputClass cls) retLines
        retLines

    let OutputUsings (usings: Using list) =
        [ for using in usings do 
            language.Using using]

    let Output spaces (nspace: Namespace) = 
        []
        |> addLines (language.NamespaceOpen nspace)
        |> addLines (OutputUsings nspace.Usings)
        |> addLines (OutputClasses nspace.Classes)
        |> addLines (language.NamespaceClose nspace)



