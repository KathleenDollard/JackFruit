namespace Generator

open System
open Generator.Language
open Common

[<AbstractClass>]
type LanguageBase() =

    abstract PrivateKeyword: string with get
    abstract PublicKeyword: string with get
    abstract InternalKeyword: string with get
    abstract ProtectedKeyword: string with get
    abstract StaticKeyword: string with get
    abstract AsyncKeyword: string with get
    abstract UsingKeyword: string with get
    abstract NamespaceKeyword: string with get
    abstract ClassKeyword: string with get
    abstract GetKeyword: string with get
    abstract SetKeyword: string with get
    abstract IfKeyword: string with get
    abstract ReturnKeyword: string with get
    abstract AwaitKeyword: string with get
    abstract NewKeyword: string with get
    abstract NullKeyword: string with get

    abstract EqualsOperator: string with get
    abstract NotEqualsOperator: string with get
    abstract BlockOpen: string with get
    abstract EndOfStatement: string with get
    abstract CommentPrefix: string with get

    abstract member TypeAndName: typeName: NamedItem -> name: string -> string
    abstract member BlockClose: blockType: string -> string
    abstract member ConstructorName: Constructor -> string

    abstract member Generic: typeNames: NamedItem list -> string
    abstract member ClassOpen: Class -> string list
    abstract member MethodOpen: Method -> string list
    abstract member MethodClose: Method -> string list
    abstract member AutoProperty: Property -> string list
    abstract member PropertyOpen: Property -> string list
    abstract member Field: Field -> string list

    abstract member IfOpen: If -> string list
    abstract member ForEachOpen: ForEach -> string list
    abstract member ForEachClose : ForEach -> string list 

    abstract member AssignWithDeclare: AssignWithDeclare -> string list


    interface ILanguage with 

        member this.Using using =
            let alias = 
                match using.Alias with 
                | Some a -> $"{a} = "
                | None -> ""
            [ $"{this.UsingKeyword} {alias}{using.Namespace}{this.EndOfStatement}" ]

        member this.NamespaceOpen nspace = 
            [$"{this.NamespaceKeyword} {nspace.NamespaceName}"; this.BlockOpen]
        member this.NamespaceClose _ = 
            [this.BlockClose this.NamespaceKeyword]

        member this.ClassOpen cls = this.ClassOpen cls
        member this.ClassClose _ =
            [this.BlockClose this.ClassKeyword ]

        member this.ConstructorOpen(ctor: Constructor) =
            [ $"{this.ScopeOutput ctor.Scope}{this.StaticOutput ctor.StaticOrInstance} {this.ConstructorName ctor}({this.OutputParameters ctor.Parameters})"
              this.BlockOpen]
        member this.ConstructorClose _ =
            [this.BlockClose "Sub"]
 
        member this.MethodOpen method = this.MethodOpen method
        member this.MethodClose method = this.MethodClose method

        member this.AutoProperty property = this.AutoProperty property
        member this.PropertyOpen property = this.PropertyOpen property
        member this.PropertyClose _ = [this.BlockClose "Property"]        
        member this.GetOpen _ =[this.GetKeyword; this.BlockOpen]
        member this.GetClose _ =[this.BlockClose "Get"]
        member this.SetOpen _ =[this.SetKeyword; this.BlockOpen]
        member this.SetClose _ = [this.BlockClose "Set"]

        member this.Field (field: Field) = this.Field field

        member this.IfOpen ifInfo = this.IfOpen ifInfo
        member this.IfClose _ = [this.BlockClose "If"]

        member this.ForEachOpen forEach = this.ForEachOpen forEach
        member this.ForEachClose forEach = this.ForEachClose forEach

        member this.Assignment assignment =
            [$"{assignment.Item} = {this.OutputExpression assignment.Value}{this.EndOfStatement}"]
        member this.AssignWithDeclare assign = this.AssignWithDeclare assign
      
        member this.Return ret =
            [$"{this.ReturnKeyword} {this.OutputExpression ret}{this.EndOfStatement}"]
        member this.SimpleCall simple =
            [$"{this.OutputExpression simple}{this.EndOfStatement}"]
        member this.Comment comment =
            [$"{this.OutputExpression comment}"]
        member this.Pragma pragma =
            [$"#{pragma}"]


        member this.Invocation invocation =
            let awaitIfNeeded = 
                if invocation.ShouldAwait then
                    $"{this.AwaitKeyword} "
                else
                    ""
            $"{awaitIfNeeded}{invocation.Instance}.{invocation.MethodName}({this.OutputArguments invocation.Arguments})"

        member this.Comparison comparison =
            $"{this.OutputExpression comparison.Left} {this.OutputOperator comparison.Operator} {this.OutputExpression comparison.Right}"

        member this.NamedItemOutput namedItem =
            $"{this.OutputNamedItem namedItem}"

    
        

    member this.AsyncOutput isAsync =
        if isAsync then $"{this.AsyncKeyword} "
        else ""

    member this.StaticOutput staticOrInstance =
        match staticOrInstance with 
        | Static -> $"{this.StaticKeyword} "
        | _ -> ""

    member this.ScopeOutput scope =
        match scope with 
        | Public -> this.PublicKeyword
        | Private -> this.PrivateKeyword
        | Internal -> this.InternalKeyword
        | Protected -> this.ProtectedKeyword

    member this.OutputParameters (parameters: Parameter list) = 
        let getDefault parameter =
            match parameter.Default with 
                | None -> ""
                | Some def -> $" = {this.OutputExpression def}"
        
        let s = [ for param in parameters do
                    $"{this.TypeAndName param.Type param.ParameterName}{getDefault param}"]
        String.Join(", ", s)
    
    member this.OutputOperator operator =
        match operator with 
        | Equals -> this.EqualsOperator
        | NotEquals -> this.NotEqualsOperator
        | GreaterThan -> ">"
        | LessThan -> "<"
        | GreaterThanOrEqualTo -> ">="
        | LessThanOrEqualTo -> "<="

    member this.OutputNamedItem namedItem =
        match namedItem with 
        | SimpleNamedItem name -> name
        | GenericNamedItem (name, genericTypes) ->
            let generics = 
                match genericTypes with 
                | [] -> ""
                | _ -> 
                    this.Generic genericTypes
            $"{name}{generics}"
    
    member this.OutputArguments (arguments: Expression list) = 
        let s = [ for arg in arguments do
                    $"{this.OutputExpression arg}"]
        String.Join(", ", s)
    
    member this.OutputInvocation invocation = 
        $"{this.OutputNamedItem invocation.Instance}.{this.OutputNamedItem invocation.MethodName}({this.OutputArguments invocation.Arguments})" 
    
    member this.OutputComparison comparison = 
        $"{this.OutputExpression comparison.Left} {this.OutputOperator comparison.Operator} {this.OutputExpression comparison.Right}"
    
    member this.OutputInstantiation (instantiation: Instantiation) = 
        $"{this.NewKeyword} {this.OutputNamedItem instantiation.TypeName}({this.OutputArguments instantiation.Arguments})"  
    
    member this.OutputExpression expression =
        match expression with 
        | Invocation x -> this.OutputInvocation x
        | Comparison x -> this.OutputComparison x
        | Instantiation x -> this.OutputInstantiation x
        | StringLiteral x -> "\"" + x + "\""
        | NonStringLiteral x -> x
        | Symbol x -> x
        | Comment x -> $"{this.CommentPrefix} {x}"
        | Pragma x -> $"# {x}"
        | Null _ -> this.NullKeyword // TODO: Watch for issues with this. Comparisons with Null from Nullable can be flipped from C#
    
    member this.OutputAssignment assignment =
        $"{assignment.Item} = {assignment.Value}"


