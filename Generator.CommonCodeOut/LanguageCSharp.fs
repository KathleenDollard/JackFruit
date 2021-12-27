namespace Generator

open System
open Generator.Language
open Common
open Utilities
open Generator.LanguageRoslynOut
open Generator.LanguageStatements

type LanguageCSharp() =
    inherit LanguageBase()

    override _.PrivateKeyword = "private"
    override _.PublicKeyword = "public"
    override _.InternalKeyword = "internal"
    override _.ProtectedKeyword = "protected"
    override _.StaticKeyword = "static"
    override _.AsyncKeyword = "async"
    override _.PartialKeyword = "partial"
    override _.AbstractKeyword = "abstract"
    override _.ReadonlyKeyword = "readonly"
    override _.UsingKeyword = "using"
    override _.NamespaceKeyword = "namespace"
    override _.ClassKeyword = "class"
    override _.GetKeyword = "get"
    override _.SetKeyword = "set"
    override _.IfKeyword = "if"
    override _.ReturnKeyword = "return"
    override _.AwaitKeyword = "await"
    override _.NewKeyword = "new"
    override _.NullKeyword = "null"

    override _.EqualsOperator = "=="
    override _.NotEqualsOperator = "!="
    override _.BlockOpen = "{"
    override _.EndOfStatement = ";"
    override _.CommentPrefix = "//"

    override this.TypeAndName typeName name = $"{this.OutputNamedItem typeName} {name}"
    override _.BlockClose _ = "}"
    override _.ConstructorName cls  = cls.ClassName

    override this.Generic typeNames  = 
        match typeNames with 
        | [] -> ""
        | _ -> 
            let generics = 
                [ for t in typeNames do this.OutputNamedItem t ]
                |> StringJoin ", "
            $"<{generics}>"

    override this.ClassOpen cls  =     
        let addColonIfNeeded list =
                  match list with 
                  | [] -> []
                  | head::tail -> List.insertAt 0 $" : {head}" tail

        let baseAndInterfaces = 
            [ match cls.InheritedFrom with 
              | SomeBase t -> this.OutputNamedItem t
              | NoBase -> () 
                    
              for i in cls.ImplementedInterfaces do
                let (ImplementedInterface name) = i
                this.OutputNamedItem name ]
              |> addColonIfNeeded
              |> String.concat ", " 

        [ $"{this.ScopeOutput cls.Scope}{this.OutputModifiers cls.Modifiers} class {this.OutputNamedItem cls.ClassName}{baseAndInterfaces}"; 
          "{"]

    override this.MethodOpen method  = 
       let returnType =
            match method.ReturnType with 
            | Type t -> this.OutputNamedItem t
            | Void -> "void"
       [$"{this.ScopeOutput method.Scope}{this.OutputModifiers method.Modifiers} {returnType} {this.OutputNamedItem method.MethodName}({this.OutputParameters method.Parameters})"; "{"]
    override _.MethodClose _ = [ "}" ]

    override this.AutoProperty property  = 
        [$"{this.ScopeOutput property.Scope}{this.OutputModifiers property.Modifiers} {this.OutputNamedItem property.Type} {property.PropertyName} {{get; set;}}"]

    override this.PropertyOpen property  = 
        [$"{this.ScopeOutput property.Scope}{this.OutputModifiers property.Modifiers} {this.OutputNamedItem property.Type} {property.PropertyName}"; "{"]

    override this.Field field  = 
        [$"{this.ScopeOutput field.Scope}{this.OutputModifiers field.Modifiers} {this.OutputNamedItem field.FieldType} {field.FieldName}"]

    override this.IfOpen ifInfo  = 
        [$"if ({this.OutputExpression ifInfo.IfCondition})"; "{"]
    override this.ElseIfOpen ifInfo  = 
        [$"else if ({this.OutputExpression ifInfo.ElseIfCondition})"; "{"]
    override _.ElseOpen _  = 
        [$"else"; "{"]

    override _.ForEachOpen forEach  = 
        [$"foreach (var {forEach.LoopVar} in {forEach.LoopOver})"; "{"]
    override _.ForEachClose _ = ["}"]

    override this.AssignWithDeclare assign  = 
        let t = 
            match assign.TypeName with 
            | Some n -> this.OutputNamedItem n
            | None -> "var"
        [$"{t} {assign.Variable} = {this.OutputExpression assign.Value};"]

    override this.CompilerDirective directive =
        let resolveTriState action =
            match action with 
            | Disable -> "disable"
            | Enable -> "enable"
            | Restore -> "restore"

        let commaDelimit (list: int list) =
            String.Join (", ", list)

        match directive.CompilerDirectiveType with
        | CompilerDirectiveType.IfDef symbol -> [ $"#if {symbol}" ]
        | ElIfDef symbol -> [ $"#elif {symbol}" ]
        | ElseDef -> [ "#else" ]
        | EndIfDef -> [ "#endif" ]
        | Nullable (action, setting) -> [ $"" ]
        | Define symbol -> [ $"#def {symbol}" ]
        | UnDefine symbol -> [ $"#undef {symbol}" ]
        | Region name -> [ $"#region {name}" ]
        | EndRegion -> [ $"#endregion" ]
        | CompilerError message -> [ $"#error message" ]
        | CompilerWarning message -> [ $"#warning message" ]
        | Line (lineNumber, fileNameOverride) -> [ $"#line {lineNumber} {fileNameOverride}" ]
        | PragmaWarning (action, warnings) -> [ $"#pragma {resolveTriState action} {commaDelimit warnings}" ]
        // If people actually use the following, give some Guid and CheckSum help
        | PragmaCheckSum (filename, guid, checksumBytes) -> [ $"" ]
