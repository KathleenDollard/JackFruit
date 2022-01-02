namespace Generator

open System
open Generator.Language
open Common
open Utilities
open Generator.LanguageRoslynOut
open Generator.LanguageStatements

type LanguageVisualBasic() =
    inherit LanguageBase()

    override _.PrivateKeyword = "Private"
    override _.PublicKeyword = "Public"
    override _.InternalKeyword = "Friend"
    override _.ProtectedKeyword = "Protected"
    override _.StaticKeyword = "Shared"
    override _.AsyncKeyword = "Async"
    override _.PartialKeyword = "Partial"
    override _.AbstractKeyword = "MustInherit"
    override _.ReadonlyKeyword = "Readonly"
    override _.UsingKeyword = "Imports"
    override _.NamespaceKeyword = "Namespace"
    override _.ClassKeyword = "Class"
    override _.GetKeyword = "Get"
    override _.SetKeyword = "Set"
    override _.IfKeyword = "If"
    override _.ReturnKeyword = "Return"
    override _.AwaitKeyword = "Await"
    override _.NewKeyword = "New"
    override _.NullKeyword = "Nothing"
    override _.TrueKeyword = "True"
    override _.FalseKeyword = "False"

    override _.EqualsOperator = "=="
    override _.NotEqualsOperator = "<>"
    override _.BlockOpen = ""
    override _.EndOfStatement = ""
    override _.CommentPrefix = "''"

    override this.TypeAndName typeName name = $"{name} As {this.OutputNamedItem typeName}"
    override _.BlockClose structureName = $"End {structureName}"
    override _.ConstructorName _ _  = "Sub New"

    override this.Generic typeNames  = 
        match typeNames with 
        | [] -> ""
        | _ -> 
            let generics = 
                [ for t in typeNames do this.OutputNamedItem t ]
                |> StringJoin ", "
            $"(Of {generics})"

    override this.ClassOpen cls  =           
        [ $"{this.ScopeOutput cls.Scope}{this.OutputModifiers cls.Modifiers} Class {this.OutputNamedItem cls.ClassName}"

          match cls.InheritedFrom with 
          | SomeBase t -> this.OutputNamedItem t
          | NoBase -> ()

          for inter in cls.ImplementedInterfaces do 
            $"Implements {inter}" ]


    override this.MethodOpen  method  = 
        let modifiers = $"{this.ScopeOutput method.Scope}{this.OutputModifiers method.Modifiers}"
        [ match method.ReturnType with 
            | Void -> $" Sub {this.OutputNamedItem method.MethodName}({this.OutputParameters method.Parameters})"
            | _ -> $" Function {this.OutputNamedItem method.MethodName}({this.OutputParameters method.Parameters}) As {method.ReturnType}" ]
        // TODO: Add implements to the basic data

    override _.MethodClose method  = 
        [ match method.ReturnType with 
             | Void -> "End Sub"
             | _ -> "End Function" ]

    override this.AutoProperty property  = 
        [$"{this.ScopeOutput property.Scope}{this.OutputModifiers property.Modifiers} Property {property.PropertyName} As  {this.OutputNamedItem property.Type}"]

    override this.PropertyOpen property  = 
        [$"{this.ScopeOutput property.Scope}{this.OutputModifiers property.Modifiers} Property {property.PropertyName} As  {this.OutputNamedItem property.Type}"]

    override this.Field field  = 
        [$"{this.ScopeOutput field.Scope}{this.OutputModifiers field.Modifiers} {field.FieldName} As {this.OutputNamedItem field.FieldType}"]
        // TODO: Intial value. Put conditional in base?

    override this.IfOpen ifInfo  = 
        [$"If {this.OutputExpression ifInfo.IfCondition} Then"]
    override this.ElseIfOpen ifInfo  = 
        [$"ElseIf {this.OutputExpression ifInfo.ElseIfCondition} Then"]
    override _.ElseOpen _  = 
        [$"Else"]

    override _.ForEachOpen forEach  = 
        [$"For Each (var {forEach.LoopVar} In {forEach.LoopOver})"]
    override _.ForEachClose _ = ["Next"]


    override this.AssignWithDeclare assign  = 
        let t = 
            match assign.TypeName with 
            | Some n -> $" As {this.OutputNamedItem n}"
            | None -> "var"
        [$"Dim {assign.Variable} {t} = {this.OutputExpression assign.Value}"]

    override this.CompilerDirective directive =
         let resolveTriState action =
             match action with 
             | Disable -> "disable"
             | Enable -> "enable"
             | Restore -> "restore"

         let commaDelimit (list: int list) =
             String.Join (", ", list)

         match directive.CompilerDirectiveType with
         | CompilerDirectiveType.IfDef symbol -> [ $"#If {symbol}" ]
         | ElIfDef symbol -> [ $"#ElseIf {symbol}" ]
         | ElseDef -> [ "#Else" ]
         | EndIfDef -> [ "#End If" ]
         | Nullable (_, _) -> [ $"" ]
         | Define symbol -> [ $"#def {symbol}" ]
         | UnDefine symbol -> [ $"#undef {symbol}" ]
         | Region name -> [ $"#Region {name}" ]
         | EndRegion -> [ $"#End Region" ]
         | CompilerError message -> [ $"#error message" ]
         | CompilerWarning message -> [ $"#warning message" ]
         | Line (lineNumber, fileNameOverride) -> [ $"#line {lineNumber} {fileNameOverride}" ]
         | PragmaWarning (action, warnings) -> [ $"#pragma {resolveTriState action} {commaDelimit warnings}" ]
         // If people actually use the following, give some Guid and CheckSum help
         | PragmaCheckSum (filename, guid, checksumBytes) -> [ $"" ]