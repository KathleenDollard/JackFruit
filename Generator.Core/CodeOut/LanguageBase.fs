namespace Generator

open System
open Generator.LanguageModel
open Generator.LanguageStatements
open Generator.LanguageExpressions
open Common

[<AbstractClass>]
type LanguageBase() =
    abstract UnknownKeyword: string with get
    default _.UnknownKeyword = "<UNKNOWN>"

    abstract PrivateKeyword: string with get
    abstract PublicKeyword: string with get
    abstract InternalKeyword: string with get
    abstract ProtectedKeyword: string with get
    abstract ProtectedInternalKeyword: string with get
    abstract PrivateProtectedKeyword: string with get

    abstract StaticKeyword: string with get
    abstract AsyncKeyword: string with get
    abstract PartialKeyword: string with get
    abstract AbstractKeyword: string with get
    abstract ReadonlyKeyword: string with get
    abstract SealedKeyword: string with get

    abstract TrueKeyword: string with get
    abstract FalseKeyword: string with get

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
    abstract member ConstructorName: ClassModel -> ConstructorModel -> string

    abstract member Generic: typeNames: NamedItem list -> string
    abstract member ClassOpen: ClassModel -> string list
    abstract member MethodOpen: MethodModel -> string list // Extension is not currently supported
    abstract member MethodClose: MethodModel -> string list
    abstract member AutoProperty: PropertyModel -> string list
    abstract member PropertyOpen: PropertyModel -> string list
    abstract member Field: FieldModel -> string list

    abstract member IfOpen: IfModel -> string list
    abstract member ElseIfOpen: ElseIfModel -> string list
    abstract member ElseOpen: ElseModel -> string list
    abstract member ForEachOpen: ForEachModel -> string list
    abstract member ForEachClose : ForEachModel -> string list 

    abstract member AssignWithDeclare: AssignWithDeclareModel -> string list
    abstract member CompilerDirective: CompilerDirectiveModel -> string list


    interface ILanguage with 

        member this.Using using =
            let alias = 
                match using.Alias with 
                | Some a -> $"{a} = "
                | None -> ""
            [ $"{this.UsingKeyword} {alias}{using.UsingNamespace}{this.EndOfStatement}" ]

        member this.NamespaceOpen nspace = 
            [$"{this.NamespaceKeyword} {nspace.NamespaceName}"; this.BlockOpen]
        member this.NamespaceClose _ = 
            [this.BlockClose this.NamespaceKeyword]

        member this.ClassOpen cls = this.ClassOpen cls
        member this.ClassClose _ =
            [this.BlockClose this.ClassKeyword ]

        member this.ConstructorOpen cls (ctor: ConstructorModel) =
            [ $"{this.ScopeOutput ctor.Scope}{this.OutputModifiers ctor.Modifiers} {this.ConstructorName cls ctor}({this.OutputParameters ctor.Parameters})"
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

        member this.Field (field: FieldModel) = this.Field field

        member this.IfOpen ifInfo = this.IfOpen ifInfo
        member this.ElseIfOpen ifInfo = this.ElseIfOpen ifInfo
        member this.ElseOpen ifInfo = this.ElseOpen ifInfo
        member this.IfClose _ = [this.BlockClose "If"]
        member this.ElseIfClose _ = [this.BlockClose "ElseIf"]
        member this.ElseClose _ = [this.BlockClose "Else"]

        member this.ForEachOpen forEach = this.ForEachOpen forEach
        member this.ForEachClose forEach = this.ForEachClose forEach

        member this.Assignment assignment =
            [$"{assignment.Variable} = {this.OutputExpression assignment.Value}{this.EndOfStatement}"]
        member this.AssignWithDeclare assign = this.AssignWithDeclare assign
      
        member this.Return ret =
            match ret.Expression with 
            | None -> [$"{this.ReturnKeyword}{this.EndOfStatement}"]
            | Some expr -> [$"{this.ReturnKeyword} {this.OutputExpression expr}{this.EndOfStatement}"]
        member this.SimpleCall simple =
            [$"{this.OutputExpression simple.Expression}{this.EndOfStatement}"]
        member this.Comment comment =
            [$"{this.CommentPrefix} {comment.Text}"]
        member this.CompilerDirective directive = this.CompilerDirective directive
        member this.InvocationStatement invocation =
            [$"{(this :> ILanguage).Invocation invocation}{this.EndOfStatement}"]
        member this.InstantiationStatement instantiation =
            [$"{(this :> ILanguage).Instantiation instantiation}{this.EndOfStatement}"]

        member this.Invocation invocation = this.OutputInvocation invocation
        member this.Instantiation instantiation = this.OutputInstantiation instantiation
        member this.Comparison comparison = this.OutputComparison comparison

        member this.NamedItemOutput namedItem =
            $"{this.OutputNamedItem namedItem}"

    
    member this.OutputModifiers (modifiers: Modifier list) =
        let modifierList =
            [ for m in modifiers do
                match m with 
                | Modifier.Static -> this.StaticKeyword
                | Async -> this.AsyncKeyword
                | Partial -> this.PartialKeyword
                | Abstract -> this.AbstractKeyword
                | Readonly -> this.ReadonlyKeyword
                | Extension -> "" // extensions need special handling
                | Sealed -> this.SealedKeyword
           ] 
        String.Join(", ", modifierList)

    member this.ScopeOutput scope =
        match scope with 
        | Public -> this.PublicKeyword
        | Private -> this.PrivateKeyword
        | Internal -> this.InternalKeyword
        | Protected -> this.ProtectedKeyword
        | ProtectedInternal -> this.ProtectedInternalKeyword
        | PrivateProtected -> this.PrivateProtectedKeyword
        | Unknown -> this.PublicKeyword

    member this.OutputParameters (parameters: ParameterModel list) = 
        let getParameter (param:ParameterModel) =
            let nameAndType =  $"{this.TypeAndName param.Type param.ParameterName}"
            match param.Style with 
                | Normal -> nameAndType
                | DefaultValue def -> $"{nameAndType} = {this.OutputExpression def}"
                | IsParamArray -> $"params {nameAndType}[]"
        
        let s = [ for param in parameters do getParameter param ]
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
    
    member this.OutputArguments (arguments: IExpression list) = 
        let s = [ for arg in arguments do
                    $"{this.OutputExpression arg}"]
        String.Join(", ", s)
    
    member this.OutputInvocation invocation = 
        let awaitIfNeeded = 
            if invocation.ShouldAwait then
                $"{this.AwaitKeyword} "
            else
                ""
        $"{awaitIfNeeded}{this.OutputNamedItem invocation.Instance}.{this.OutputNamedItem invocation.MethodName}({this.OutputArguments invocation.Arguments})" 
    
    member this.OutputComparison comparison = 
        $"{this.OutputExpression comparison.Left} {this.OutputOperator comparison.Operator} {this.OutputExpression comparison.Right}"
    
    member this.OutputInstantiation (instantiation: InstantiationModel) = 
        $"{this.NewKeyword} {this.OutputNamedItem instantiation.TypeName}({this.OutputArguments instantiation.Arguments})"  
    
    member this.OutputExpression expression =
        let x = CompareLiteralsModel.TrueLiteral // trying to force the resolution
        match expression with 
        | :? InvocationModel as x -> this.OutputInvocation x
        | :? ComparisonModel as x -> this.OutputComparison x
        | :? InstantiationModel as x -> this.OutputInstantiation x
        | :? LiteralsModel as asLiteralModel ->
            match asLiteralModel with 
            | StringLiteral x -> "\"" + x + "\""
            | IntegerLiteral x -> x.ToString()
            | DoubleLiteral x -> x.ToString()
            | SymbolLiteral x ->
                match x with 
                | Symbol s -> s
            | NullLiteral _ -> this.NullKeyword // TODO: Watch for issues with this. Comparisons with Null from Nullable can be flipped from C#
            | UnknownLiteral x -> $"**Unknown<{x}>**"
        | :? CompareLiteralsModel as asLiteralModel ->
            match asLiteralModel with 
            | TrueLiteral -> this.TrueKeyword
            | FalseLiteral -> this.FalseKeyword
        | _ -> invalidOp "This expression type is not yet implemented"

    member this.OutputAssignment assignment =
        $"{assignment.Variable} = {assignment.Value}"


