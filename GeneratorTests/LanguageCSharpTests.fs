module Generator.Tests.LanguageCSharpTests

open Generator.Language

type NamedItem = | NamedItem of string

// Where a class may be used, use NamedType, even if it will generally be an instance
type GenericNamedItem with
    static member ForTesting =
    { TypeName = NamedItem 
      GenericTypes = GenericNamedItem list }

type Invocation with     static member ForTesting
    { Instance = GenericNamedItem
      MethodName = NamedItem
      Arguments = Expression list}

type Instantiation with     static member ForTesting
    { TypeName = GenericNamedItem
      Arguments = Expression list}

type Comparison with     static member ForTesting
    { Left = Expression
      Right = Expression
      Operator = Operator}

type If with     static member ForTesting
    { Condition = Expression
      Statements = Statement list
      Elses = If list}

type ForEach with     static member ForTesting
    { LoopVar = NamedItem
      LoopOver = NamedItem
      Statements = Statement list }

type Assignment with     static member ForTesting 
    { Item = NamedItem
      Value = Expression}

type Parameter with     static member ForTesting
    { Name = NamedItem
      Type = GenericNamedItem
      Default = Expression option
      IsParams = bool}

type Method with     static member ForTesting
    { Name = GenericNamedItem
      ReturnType = GenericNamedItem
      IsStatic = bool
      Scope = Scope
      Parameters = Parameter list
      Statements = Statement list}

type Property with     static member ForTesting
    { Name = NamedItem
      Type = GenericNamedItem
      IsStatic = bool
      Scope = Scope
      GetStatements = Statement list
      SetStatements = Statement list}

type Class with     static member ForTesting 
    { Name = GenericNamedItem
      IsStatic = bool
      Scope = Scope
      Members = Member list}

type Using with     static member ForTesting 
    { Namespace = string
      Alias = string option}

type Namespace with     static member ForTesting 
    { Name = NamedItem
      Usings = Using list
      Classes = Class list}
