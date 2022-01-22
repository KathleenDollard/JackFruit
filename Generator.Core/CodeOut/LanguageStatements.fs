module Generator.LanguageStatements

open Generator
open Common
open Generator.LanguageModel


// There are some tradeoffs on If. This version makes the If, ElseIf, and Else entirely 
// independent. This makes some coding easier and leads to very nice syntax. It also does
// not help at all if you get goofy with placement. Let's see whether this flies
// TODO: *** This approach will not work for VB. The Else and ElseIf's must be interleaved
// I will return to this when I am smarter about the state of CE's
//     If condition Then
//     Else
//     End If
// The approach I am considering is using the easy model for creation, and then 
// switching to the complex model prior to generation. Alternatively, for If, the 
// next statement could also be passed, which should be the next part of the If it exists.
type IfModel =
    { IfCondition: IExpression
      Statements: IStatement list }
    static member Create condition statements =
        { IfCondition = condition; Statements = statements }
    member this.AddStatements statements =
        { this with Statements = List.append this.Statements statements}
    interface IStatement
    interface IStatementContainer<IfModel> with
        member this.AddStatements statements = this.AddStatements statements

type ElseIfModel =
    { ElseIfCondition: ICompareExpression
      Statements: IStatement list }
    static member Create condition statements =
        { ElseIfCondition = condition; Statements = statements }
    member this.AddStatements statements =
        { this with Statements = List.append this.Statements statements}
    interface IStatement
    interface IStatementContainer<ElseIfModel> with
        member this.AddStatements statements = this.AddStatements statements
  
type ElseModel =
    { ElseStatements: IStatement list }
    static member Create elseStatements =
        { ElseStatements = elseStatements }
    member this.AddStatements statements =
        { this with ElseStatements = List.append this.ElseStatements statements}
    interface IStatement
    interface IStatementContainer<ElseModel> with
        member this.AddStatements statements = this.AddStatements statements

type ForEachModel =
    { LoopVar: string
      LoopOver: string
      Statements: IStatement list }
    interface IStatement
    static member Create loopvar loopover statements =
        { LoopVar = loopvar; LoopOver = loopover; Statements = statements }

type AssignmentModel = 
    { Variable: string
      Value: IExpression}
    interface IStatement
    static member Create variable value =
        { Variable = variable; Value = value }

type AssignWithDeclareModel =
    { Variable: string
      TypeName: NamedItem option
      Value: IExpression}
    interface IStatement
    static member Create variable typename value =
        { Variable = variable; TypeName = typename; Value = value }

type ReturnModel =
    { Expression: IExpression option }
    interface IStatement
    static member Create expression =
        { Expression = expression }

type SimpleCallModel =
    { Expression: IExpression }
    interface IStatement
    static member Create expression =
        { Expression = expression }

type CommentModel =
    { Text: string}
    interface IStatement
    static member Create text =
        { Text = text }

type CompilerActionTriState =
    | Disable
    | Enable
    | Restore

type CompilerNullableAction =
    | All
    | Annotations
    | Warnings

// KAD: Make a single member DU for Symbole which will allow validation at hte right time
type CompilerDirectiveType =
    | IfDef of symbol: string
    | ElIfDef of symbol: string
    | ElseDef
    | EndIfDef
    | Nullable of action: CompilerActionTriState * setting: CompilerNullableAction
    | Define of symbol: string
    | UnDefine of symbol: string
    | Region of name: string
    | EndRegion
    | CompilerError of message: string
    | CompilerWarning of message: string
    | Line of lineNumer: int * fileNameOverride: string
    | PragmaWarning of action: CompilerActionTriState * warnings: int list
    // If people actually use the following, give some Guid and CheckSum help
    | PragmaCheckSum of filename: string * guid: string * checksumBytes: string

    
type CompilerDirectiveModel =
    { CompilerDirectiveType: CompilerDirectiveType }
    interface IStatement
    static member Create directive =
        { CompilerDirectiveType = directive }
    

