namespace Generator

open System
open Generator.Language
open Generator.CSharpLanguageExtensions
open Common

type LanguageCSharp() =

    let staticOutput staticOrInstance = 
        match staticOrInstance with 
        | Instance -> ""
        | Static -> " static"

    let asyncOutput isAsync =
        if isAsync then 
            " async"
        else
            ""

    interface ILanguage with 

        member _.PrivateKeyword = "private"
        member _.PublicKeyword = "public"
        member _.InternalKeyword = "private"
        member _.StaticKeyword = "public"

        member _.Using using =
            let alias = 
                match using.Alias with 
                | Some a -> $"{a} = "
                | None -> ""
            [ $"using {alias}{using.Namespace};" ]

        member _.NamespaceOpen nspace = 
            [$"namespace {nspace.NamespaceName}"; "{"]
        member _.NamespaceClose _ = 
            ["}"]

        member _.ClassOpen cls =
            let addColonIfNeeded list =
                match list with 
                | [] -> []
                | head::tail -> List.insertAt 0 $" : {head}" tail

            let baseAndInterfaces = 
                [ match cls.InheritedFrom with 
                  | Some t -> t.Output
                  | None -> () 
                  
                  for i in cls.ImplementedInterfaces do
                    i.Output ]
                |> addColonIfNeeded
                |> String.concat ", "

            [ $"{cls.Scope.Output}{staticOutput cls.StaticOrInstance} class {cls.ClassName.Output}{baseAndInterfaces}"; 
               
              "{"]
        member _.ClassClose _ =
            ["}"]

        member _.MethodOpen(method: Method) =
            let returnType =
                match method.ReturnType with 
                | Type t -> t.Output
                | Void -> "void"
            [$"{method.Scope.Output}{staticOutput method.StaticOrInstance}{asyncOutput method.IsAsync} {returnType} {method.MethodName.Output}({OutputParameters method.Parameters})"; "{"]
        member _.MethodClose _ =
            ["}"]

        member _.ConstructorOpen(ctor: Constructor) =
            [$"{ctor.Scope.Output}{staticOutput ctor.StaticOrInstance} {ctor.ClassName.Output}({OutputParameters ctor.Parameters})"; "{"]
        member _.ConstructorClose _ =
            ["}"]

        member _.AutoProperty(property: Property) =
            [$"{property.Scope.Output}{staticOutput property.StaticOrInstance} {property.Type.Output} {property.PropertyName} {{get; set;}}"]
        member _.PropertyOpen(property: Property) =
            [$"{property.Scope.Output}{staticOutput property.StaticOrInstance} {property.Type.Output} {property.PropertyName}"; "{"]
        member _.PropertyClose _ =
            ["}"]
        member _.GetOpen _ =
            [$"get"; "{"]
        member _.GetClose _ =
            ["}"]
        member _.SetOpen _ =
            [$"set"; "{"]
        member _.SetClose _ =
            ["}"]

        member _.Field (field: Field) =
            [$"{field.Scope.Output}{staticOutput field.StaticOrInstance} {field.FieldType.Output} {field.FieldName} {{get; set;}}"]

        member _.IfOpen ifInfo =
            [$"if ({ifInfo.Condition.Output})"; "{"]
        member _.IfClose _ =
            ["}"]

        member _.ForEachOpen forEach =
            [$"foreach (var {forEach.LoopVar} in {forEach.LoopOver})"; "{"]
        member _.ForEachClose _ =
            ["}"]

        member _.Assignment assignment =
            [$"{assignment.Item} = {assignment.Value.Output};"]
        member _.AssignWithDeclare assign =
            let t = 
                match assign.TypeName with 
                | Some n -> n.Output
                | None -> "var"
            [$"{t} {assign.Variable} = {assign.Value.Output};"]
        member _.Return ret =
            [$"return {ret.Output};"]
        member _.SimpleCall simple =
            [$"{simple.Output};"]
        member _.Comment comment =
            [$"{comment.Output}"]
        member _.Pragma pragma =
            [$"{pragma.Output}"]


        member _.Invocation invocation =
            let awaitIfNeeded = 
                if invocation.ShouldAwait then
                    "await "
                else
                    ""

            $"{awaitIfNeeded}{invocation.Instance}.{invocation.MethodName}({OutputArguments invocation.Arguments})"
        member _.Comparison comparison =
            $"{comparison.Left.Output}.{comparison.Operator.Output} {comparison.Right.Output}"

        member _.NamedItemOutput namedItem =
            $"{namedItem.Output}"
