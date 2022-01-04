namespace Generator

open Common
open Generator.Language
open Generator.LanguageStatements
open Generator.LanguageExpressions


type ILanguage =

    // Language structure
    abstract member Using: UsingModel -> string list
    abstract member NamespaceOpen: NamespaceModel -> string list
    abstract member NamespaceClose: NamespaceModel -> string list
    abstract member ClassOpen: ClassModel -> string list
    abstract member ClassClose: ClassModel -> string list
    abstract member ConstructorOpen: ClassModel -> ConstructorModel -> string list
    abstract member ConstructorClose: ConstructorModel -> string list
    abstract member MethodOpen: MethodModel -> string list
    abstract member MethodClose: MethodModel -> string list
    abstract member AutoProperty: PropertyModel -> string list
    abstract member PropertyOpen: PropertyModel -> string list
    abstract member PropertyClose: PropertyModel -> string list
    abstract member Field: FieldModel -> string list
    abstract member GetOpen: PropertyModel-> string list
    abstract member GetClose: PropertyModel -> string list
    abstract member SetOpen: PropertyModel -> string list
    abstract member SetClose: PropertyModel -> string list

    // Statements
    abstract member IfOpen: IfModel -> string list
    abstract member ElseIfOpen: ElseIfModel -> string list // this gets the ElseIf's IfModel
    abstract member ElseOpen: ElseModel -> string list
    abstract member IfClose: IfModel -> string list
    abstract member ElseIfClose: ElseIfModel -> string list
    abstract member ElseClose: ElseModel -> string list
    abstract member ForEachOpen: ForEachModel -> string list
    abstract member ForEachClose: ForEachModel -> string list
    abstract member InvocationStatement: InvocationModel -> string list
    abstract member InstantiationStatement: InstantiationModel -> string list

    abstract member Assignment: AssignmentModel -> string list
    abstract member AssignWithDeclare: AssignWithDeclareModel -> string list
    abstract member Return: ReturnModel -> string list
    abstract member SimpleCall: SimpleCallModel -> string list
    abstract member Comment: CommentModel -> string list
    abstract member CompilerDirective: CompilerDirectiveModel -> string list

    abstract member Invocation: InvocationModel -> string
    abstract member Instantiation: InstantiationModel -> string
    abstract member Comparison: ComparisonModel -> string

    // Other
    abstract member NamedItemOutput: NamedItem -> string
