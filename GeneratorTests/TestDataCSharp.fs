/// This module extends the basic code elements with the most common test cases
/// These are expected to be used with "with" syntax to add more specific local test cases
module Generator.Tests.TestData

open Generator.Language

type TestData<'T> = { Data: 'T; CSharp: string list } // Add VB later

type TestDataBlock<'T> =
    { Data: 'T
      CSharpOpen: string list
      CSharpBlock: string list
      CSharpClose: string list }

// Where a class may be used, use NamedType, even if it will generally be an instance
type GenericNamedItem with
    static member ForTesting =
        let data =
            { Name = "RonWeasley"
              GenericTypes = [] }    
        { Data = data
          CSharp = [ "RonWeasley(JackRussell)" ] }

type Invocation with
    static member ForTesting =
        let data =
            { Instance = GenericNamedItem.ForTesting.Data
              MethodName = "JackRussell"
              Arguments = [] }

        { Data = data
          CSharp = [ "RonWeasley(JackRussell)" ] }

type Instantiation with
    static member ForTesting =
        let data =
            { TypeName = GenericNamedItem.ForTesting.Data
              Arguments = [] }

        { Data = data
          CSharp = [ "new RonWeasley()" ] }

type Comparison with
    static member ForTesting =
        let data =
            { Left = Symbol "left"
              Right = StringLiteral "qwerty"
              Operator = Equals }

        { Data = data
          CSharp = [ "left = \"querty\"" ] }

type If with
    static member ForTesting =
        let data =
            { Condition =
                Comparison
                    { Left = Symbol "A"
                      Right = NonStringLiteral "42"
                      Operator = Equals }
              Statements = []
              Elses = [] }

        { Data = data
          CSharpOpen = [ "if (A == 42)"; "{" ]
          CSharpBlock = []
          CSharpClose = [ "}" ] }

type ForEach with
    static member ForTesting =
        let data =
            { LoopVar = "x"
              LoopOver = "listOfThings"
              Statements = [] }

        { Data = data
          CSharpOpen =
            [ "foreach (var x in listOfThings)"
              "{" ]
          CSharpBlock = []
          CSharpClose = [ "}" ] }

type Assignment with
    static member ForTesting =
        let data =
            { Item = "item"
              Value = StringLiteral "boo!" }

        { Data = data
          CSharp = [ "item = \"boo!\";" ] }


type AssignWithDeclare with
    static member ForTesting =
        let data =
            { Item = "item"
              TypeName = None
              Value = StringLiteral "boo!" }

        { Data = data
          CSharp = [ "var item = \"boo!\";" ] }

type Parameter with
    static member ForTesting =
        let data =
            { Name = "param1"
              Type = { Name = "string"; GenericTypes = [] }
              Default = None
              IsParams = false }

        { Data = data
          CSharp = [ "string param1" ] }

type Method with
    static member ForTesting =
        let data =
            { Name = { Name = "MyMethod"; GenericTypes = [] }
              ReturnType = { Name = "string"; GenericTypes = [] }
              IsStatic = false
              Scope = Public
              Parameters = []
              Statements = [] }

        { Data = data
          CSharpOpen = [ "public string MyMethod()"; "{" ]
          CSharpBlock = []
          CSharpClose = [ "}" ] }

type Property with
    static member ForTesting =
        let data =
            { Name = "MyProperty"
              Type =
                { Name = "MyReturnType"
                  GenericTypes = [] }
              IsStatic = false
              Scope = Public
              GetStatements = []
              SetStatements = [] }

        { Data = data
          CSharpOpen = [ "public MyReturnType MyProperty"; "{"]
          CSharpBlock = []
          CSharpClose = [ "}"] }

type Class with
    static member ForTesting =
        let data =
            { Name = GenericNamedItem.ForTesting.Data
              IsStatic = false
              Scope = Public
              Members = [] }

        { Data = data
          CSharpOpen = [ "public class RonWeasley"; "{" ]
          CSharpBlock = []
          CSharpClose = [ "}" ] }

type Using with
    static member ForTesting =
        let data = { Namespace = "System"; Alias = None }

        { Data = data
          CSharp = [ "using System;" ]}

type Namespace with
    static member ForTesting =
        let data =
            { Name = "MyNamespace"
              Usings = []
              Classes = [] }

        { Data = data
          CSharpOpen = [ "namespace MyNamespace"; "{" ]
          CSharpBlock = []
          CSharpClose = [ "}" ] }
