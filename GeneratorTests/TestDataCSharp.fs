module Generator.Tests.TestDataCSharp

open Generator.Language

// Where a class may be used, use NamedType, even if it will generally be an instance
type GenericNamedItem with
    static member ForTesting =
    { Name = "RonWeasley"
      GenericTypes = [] }

type Invocation with
    static member ForTesting =
    { Instance = GenericNamedItem.ForTesting
      MethodName = "JackRussell"
      Arguments = []}

type Instantiation with
    static member ForTesting =
    { TypeName = GenericNamedItem.ForTesting
      Arguments = []}

type Comparison with 
    static member ForTesting =
    { Left = Symbol "left"
      Right = StringLiteral "qwerty"
      Operator = Equals}

type If with 
    static member ForTesting =
    { Condition = Comparison { Left = Symbol "A"; Right = NonStringLiteral "42"; Operator = Equals}
      Statements = []
      Elses = []}

type ForEach with 
    static member ForTesting =
    { LoopVar = "x"
      LoopOver = "listOfThings"
      Statements = [] }

type Assignment with 
    static member ForTesting = 
    { Item = "item"
      Value = StringLiteral "boo!"}

type Parameter with 
    static member ForTesting =
    { Name = "param1"
      Type = { Name = "string"; GenericTypes = []}
      Default = None
      IsParams = false}

type Method with 
    static member ForTesting =
    { Name = { Name = "MyMethod"; GenericTypes = [] }
      ReturnType = { Name = "string"; GenericTypes = [] }
      IsStatic = false
      Scope = Public
      Parameters = []
      Statements = []}

type Property with 
    static member ForTesting =
    { Name = "MyProperty"
      Type = { Name = "MyReturnType"; GenericTypes = []}
      IsStatic = false
      Scope = Public
      GetStatements = []
      SetStatements = []}

type Class with 
    static member ForTesting = 
    { Name = GenericNamedItem.ForTesting
      IsStatic = false
      Scope = Public
      Members = []}

type Using with 
    static member ForTesting = 
    { Namespace = "System"
      Alias = None}

type Namespace with 
    static member ForTesting = 
    { Name = "MyNamespace"
      Usings = []
      Classes = []}
