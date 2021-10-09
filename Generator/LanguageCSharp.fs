namespace Generator

open System
open Generator.Language
open Generator.CSharpLanguageExtensions


type LanguageCSharp() =

    let staticOutput isStatic = 
        if isStatic then
            " static"
        else
            ""

    //let getParameters (parameters: Parameter list) = 
    //    let getDefault parameter =
    //        match parameter.Default with 
    //            | None -> ""
    //            | Some def -> " " + getExpression def

    //    let s = [ for param in parameters do
    //                $"{param.Type} {param.Name}{getDefault param}"]
    //    String.Join("", s)

    //let getArguments arguments = 
    //    let s = [ for arg in arguments do
    //                $"{getExpression arg}"]
    //    String.Join("", s)

    interface ILanguage with 

        member __.PrivateKeyword = "private"
        member __.PublicKeyword = "public"
        member __.InternalKeyword = "private"
        member __.StaticKeyword = "public"

        member __.Using(using) =
            let alias = 
                match using.Alias with 
                | Some a -> $"{a} = "
                | None -> ""
            $"using {alias}{using.Namespace};"

        member __.NamespaceOpen(nspace) = 
            [$"namespace {nspace.Name}"; "{"]
        member __.NamespaceClose(_) = 
            ["}"]

        member __.ClassOpen(cls) =
            [$"{cls.Scope.Output}{staticOutput cls.IsStatic} class {cls.Name.Output}"; "{"]
        member __.ClassClose(_) =
            ["}"]

        member __.MethodOpen(method: Method) =
            [$"{method.Scope.Output}{staticOutput method.IsStatic} {method.ReturnType.Output} {method.Name.Output}({OutputParameters method.Parameters})"; "{"]
        member __.MethodClose(_) =
            ["}"]

        member __.PropertyOpen(property: Property) =
            [$"{property.Scope.Output}{staticOutput property.IsStatic} {property.Type.Output} {property.Name}"; "{"]
        member __.PropertyClose(_) =
            ["}"]
        member __.GetOpen(_) =
            [$"get"; "{"]
        member __.GetClose(_) =
            ["}"]
        member __.SetOpen(_) =
            [$"set"; "{"]
        member __.SetClose(_) =
            ["}"]

        member __.IfOpen(ifInfo) =
            [$"if ({ifInfo.Condition.Output})"; "{"]
        member __.IfClose(_) =
            ["}"]

        member __.ForEachOpen(forEach) =
            [$"for (var {forEach.LoopVar} in {forEach.LoopOver})"; "{"]
        member __.ForEachClose(_) =
            ["}"]

        member __.Assignment(assignment) =
            $"{assignment.Item}.= {assignment.Value};"

        member __.Invocation(invocation) =
            $"{invocation.Instance}.{invocation.MethodName}({OutputArguments invocation.Arguments})"
        member __.Comparison(comparison) =
            $"{comparison.Left.Output}.{comparison.Operator.Output} {comparison.Right.Output}"


