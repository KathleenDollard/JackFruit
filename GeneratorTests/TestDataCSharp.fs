/// This module extends the basic code elements with the most common test cases
/// These are expected to be used with "with" syntax to add more specific local test cases
module Generator.Tests.TestData

open Generator.Language
open Common

type TestData<'T> = { Data: 'T; CSharp: string list } // Add VB later

type TestDataBlock<'T> =
    { Data: 'T
      CSharpOpen: string list
      CSharpBlock: string list
      CSharpClose: string list }

// Where a class may be used, use NamedType, even if it will generally be an instance
type NamedItem with
    static member ForTesting =
        let data = NamedItem.Create "RonWeasley"  []
        { Data = data
          CSharp = [ "RonWeasley(JackRussell)" ] }

type Invocation with
    static member ForTesting =
        let data =
            { Instance = NamedItem.ForTesting.Data
              MethodName = "JackRussell"
              Arguments = [] }

        { Data = data
          CSharp = [ "RonWeasley(JackRussell)" ] }

type Instantiation with
    static member ForTesting =
        let data =
            { TypeName = NamedItem.ForTesting.Data
              Arguments = [] }

        { Data = data
          CSharp = [ "new RonWeasley()" ] }

type Comparison with
    static member ForTesting =
        let data =
            { Left = Symbol "left"
              Right = StringLiteral "qwerty"
              Operator = Operator.Equals }

        { Data = data
          CSharp = [ "left = \"querty\"" ] }

type If with
    static member ForTesting =
        let data =
            { Condition =
                Comparison
                    { Left = Symbol "A"
                      Right = NonStringLiteral "42"
                      Operator = Operator.Equals }
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
            { Variable = "item"
              TypeName = None
              Value = StringLiteral "boo!" }

        { Data = data
          CSharp = [ "var item = \"boo!\";" ] }

type Parameter with
    static member ForTesting =
        let data =
            { ParameterName = "param1"
              Type = NamedItem.Create "string"  []
              Default = None
              IsParams = false }

        { Data = data
          CSharp = [ "string param1" ] }

type Method with
    static member ForTesting =
        let data = Method.Create "MyMethod" (Some (NamedItem.Create "string"  []))

        { Data = data
          CSharpOpen = [ "public string MyMethod()"; "{" ]
          CSharpBlock = []
          CSharpClose = [ "}" ] }

type Property with
    static member ForTesting =
        let data =
            { PropertyName = "MyProperty"
              Type = NamedItem.Create "MyReturnType"  []
              StaticOrInstance = Instance
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
            { ClassName = NamedItem.ForTesting.Data
              StaticOrInstance = Instance
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
            { NamespaceName = "MyNamespace"
              Usings = []
              Classes = [] }

        { Data = data
          CSharpOpen = [ "namespace MyNamespace"; "{" ]
          CSharpBlock = []
          CSharpClose = [ "}" ] }
