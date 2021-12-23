namespace Generator

open System
open Generator.Language
open Common
open Utilities
open Generator.LanguageRoslynOut

type LanguageVisualBasic() =
    inherit LanguageBase()

    override _.PrivateKeyword = "Private"
    override _.PublicKeyword = "Public"
    override _.InternalKeyword = "Friend"
    override _.ProtectedKeyword = "Protected"
    override _.StaticKeyword = "Shared"
    override _.AsyncKeyword = "Async"
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

    override _.EqualsOperator = "=="
    override _.NotEqualsOperator = "<>"
    override _.BlockOpen = ""
    override _.EndOfStatement = ""
    override _.CommentPrefix = "''"

    override this.TypeAndName typeName name = $"{name} As {this.OutputNamedItem typeName}"
    override _.BlockClose structureName = $"End {structureName}"
    override _.ConstructorName _  = "Sub New"

    override this.Generic typeNames  = 
        match typeNames with 
        | [] -> ""
        | _ -> 
            let generics = 
                [ for t in typeNames do this.OutputNamedItem t ]
                |> StringJoin ", "
            $"(Of {generics})"

    override this.ClassOpen cls  =           
        [ $"{this.ScopeOutput cls.Scope}{this.StaticOutput cls.StaticOrInstance} Class {this.OutputNamedItem cls.ClassName}"

          match cls.InheritedFrom with 
          | Some t -> this.OutputNamedItem t
          | None -> ()

          for inter in cls.ImplementedInterfaces do 
            $"Implements {inter}" ]


    override this.MethodOpen  method  = 
        let modifiers = $"{this.ScopeOutput method.Scope}{this.StaticOutput method.StaticOrInstance}{this.AsyncOutput method.IsAsync}"
        [ match method.ReturnType with 
            | Void -> $" Sub {this.OutputNamedItem method.MethodName}({this.OutputParameters method.Parameters})"
            | _ -> $" Function {this.OutputNamedItem method.MethodName}({this.OutputParameters method.Parameters}) As {method.ReturnType}" ]
        // TODO: Add implements to the basic data

    override _.MethodClose method  = 
        [ match method.ReturnType with 
             | Void -> "End Sub"
             | _ -> "End Function" ]

    override this.AutoProperty property  = 
        [$"{this.ScopeOutput property.Scope}{this.StaticOutput property.StaticOrInstance} Property {property.PropertyName} As  {this.OutputNamedItem property.Type}"]

    override this.PropertyOpen property  = 
        [$"{this.ScopeOutput property.Scope}{this.StaticOutput property.StaticOrInstance} Property {property.PropertyName} As  {this.OutputNamedItem property.Type}"]

    override this.Field field  = 
        [$"{this.ScopeOutput field.Scope}{this.StaticOutput field.StaticOrInstance} {field.FieldName} As {this.OutputNamedItem field.FieldType}"]
        // TODO: Intial value. Put conditional in base?

    override this.IfOpen ifInfo  = 
        [$"If {this.OutputExpression ifInfo.Condition} Then"]

    override _.ForEachOpen forEach  = 
        [$"For Each (var {forEach.LoopVar} In {forEach.LoopOver})"]
    override _.ForEachClose _ = ["Next"]


    override this.AssignWithDeclare assign  = 
        let t = 
            match assign.TypeName with 
            | Some n -> $" As {this.OutputNamedItem n}"
            | None -> "var"
        [$"Dim {assign.Variable} {t} = {this.OutputExpression assign.Value}"]

