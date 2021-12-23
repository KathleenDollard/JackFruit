module Generator.LanguageStatements

open Generator
open Common
open Generator.Language

type IfModel =
    { Condition: IExpression
      Statements: IStatement list
      Elses: IfModel list}
    interface IStatement

type ForEachModel =
    { LoopVar: string
      LoopOver: string
      Statements: IStatement list }
    interface IStatement

type AssignmentModel = 
    { Item: string
      Value: IExpression}
    interface IStatement

type AssignWithDeclareModel =
    { Variable: string
      TypeName: NamedItem option
      Value: IExpression}
    interface IStatement

type ReturnModel =
    { Expression: IExpression option }
    interface IStatement

type SimpleCallModel =
    { Expression: IExpression }
    interface IStatement


