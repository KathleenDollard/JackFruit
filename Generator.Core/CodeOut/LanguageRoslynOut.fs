module Generator.LanguageRoslynOut


open Generator
open Generator.LanguageModel
open Generator.LanguageStatements
open Generator.LanguageExpressions
open System

type String with
    member this.AsField = $"_{Char.ToLower(this[0])}{this[1..]}"
    member this.AsVariable = $"{Char.ToLower(this[0])}{this[1..]}"
    member this.AsPublic = $"{Char.ToUpper(this[0])}{this[1..]}"
    

type RoslynOut(language: ILanguage, writer: IWriter) =

    //let addLines (newLines: string list) oldLines =
    //    let mutable retLines = oldLines
    //    for line in newLines do
    //        retLines <- line::retLines
    //    retLines

    member _.BlankLine() =
        writer.AddLine ""

    member this.OutputStatement (statement: IStatement) =
        match statement with 
        | :? IfModel as x -> this.OutputIf x
        | :? AssignmentModel as x -> this.OutputAssignment x
        | :? AssignWithDeclareModel as x -> this.OutputAssignWithDeclare x
        | :? ReturnModel as x -> this.OutputReturn x
        | :? ForEachModel as x -> this.OutputForEach x
        | :? SimpleCallModel as x -> this.OutputSimpleCall x
        | :? InvocationModel as x -> this.OutputInvocation x
        | :? InstantiationModel as x -> this.OutputInstantiation x
        | :? CommentModel as x -> this.OutputComment x
        | :? CompilerDirectiveModel as x -> this.OutputCompilerDirective x
        | a -> invalidOp $"Unexpected statement type: {a}"

    member this.OutputStatements (statements: IStatement list) =
        for statement in statements do 
            this.OutputStatement statement

    member this.OutputIf (ifInfo: IfModel) = 
        writer.AddLines (language.IfOpen ifInfo)
        writer.IncreaseIndent()
        this.OutputStatements ifInfo.Statements
        writer.DecreaseIndent()
        writer.AddLines (language.IfClose ifInfo)

    member this.OutputElseIf (ifInfo: ElseIfModel) = 
        writer.AddLines (language.ElseIfOpen ifInfo)
        writer.IncreaseIndent()
        this.OutputStatements ifInfo.Statements
        writer.DecreaseIndent()
        writer.AddLines (language.ElseIfClose ifInfo)

    member this.OutputElse (elseInfo: ElseModel) = 
        writer.AddLines (language.ElseOpen elseInfo)
        writer.IncreaseIndent()
        this.OutputStatements elseInfo.ElseStatements
        writer.DecreaseIndent()
        writer.AddLines (language.ElseClose elseInfo)

    member _.OutputAssignWithDeclare assign =
        writer.AddLines (language.AssignWithDeclare assign)

    member _.OutputAssignment assignment =
        writer.AddLines (language.Assignment assignment)

    member _.OutputReturn ret =
        writer.AddLines (language.Return ret)

    member this.OutputForEach foreach :unit =
        writer.AddLines (language.ForEachOpen foreach) 
        writer.IncreaseIndent()
        this.OutputStatements foreach.Statements
        writer.DecreaseIndent()
        writer.AddLines (language.ForEachClose foreach)

    member _.OutputSimpleCall simple =
        writer.AddLines (language.SimpleCall simple)

    member _.OutputInvocation invocation = 
        writer.AddLines (language.InvocationStatement invocation)

    member _.OutputInstantiation instantiation =
        writer.AddLines (language.InstantiationStatement instantiation)

    member _.OutputComment comment =
        writer.AddLines (language.Comment comment)

    member _.OutputCompilerDirective directive =
        writer.AddLines (language.CompilerDirective directive)

    member this.OutputMethod (method: MethodModel) =
        writer.AddLines (language.MethodOpen method) 
        writer.IncreaseIndent()
        this.OutputStatements method.Statements
        writer.DecreaseIndent()
        writer.AddLines (language.MethodClose method)

    member this.OutputConstructor cls (ctor: ConstructorModel) =
       writer.AddLines (language.ConstructorOpen cls ctor) 
       writer.IncreaseIndent()
       this.OutputStatements ctor.Statements
       writer.DecreaseIndent()
       writer.AddLines (language.ConstructorClose ctor)

    member this.OutputProperty (prop : PropertyModel) =
        let isAutoProp = 
            match prop.Setter, prop.Getter with
            | None, None -> true
            | Some getter, None -> getter.Statements.IsEmpty
            | None, Some setter -> setter.Statements.IsEmpty
            | Some getter, Some setter ->
                match (getter.Statements.IsEmpty), (setter.Statements.IsEmpty) with
                | true, true -> true
                | _, _ -> false
        if isAutoProp then
            writer.AddLines (language.AutoProperty prop)
        else
            writer.AddLines (language.PropertyOpen prop)
            writer.IncreaseIndent()
            match prop.Getter with 
            | None -> ()
            | Some getter when getter.Statements.IsEmpty -> ()
            | Some getter ->
                writer.AddLines (language.GetOpen prop)
                writer.IncreaseIndent()
                this.OutputStatements getter.Statements
                writer.DecreaseIndent()
                writer.AddLines (language.GetClose prop)
            match prop.Setter with 
            | None -> ()
            | Some setter when setter.Statements.IsEmpty -> ()
            | Some setter ->
                writer.AddLines (language.SetOpen prop)
                writer.IncreaseIndent()
                this.OutputStatements setter.Statements
                writer.DecreaseIndent()
                writer.AddLines (language.SetClose prop)
            writer.DecreaseIndent()
            writer.AddLines (language.PropertyClose prop)

    member this.OutputField (field: FieldModel) =
        writer.AddLines (language.Field field)

    member this.OutputMembers cls (members: IMember list) =
        for mbr in members do 
            match mbr with 
            | :? FieldModel as x -> this.OutputField x
            | :? ConstructorModel as x -> this.OutputConstructor cls x
            | :? MethodModel as x -> this.OutputMethod x
            | :? PropertyModel as x -> this.OutputProperty x
            | a -> invalidOp $"Unexpected member type during output. Type: {a}"

    member this.OutputClass cls =
        writer.AddLine("")
        writer.AddLines (language.ClassOpen cls)
        writer.IncreaseIndent()
        this.OutputMembers cls cls.Members 
        writer.DecreaseIndent()
        writer.AddLines (language.ClassClose cls)

    member this.OutputClasses (classes: ClassModel list) =
        for cls in classes do 
            this.OutputClass cls

    member _.OutputUsings (usings: UsingModel list) =
        for using in usings do 
            writer.AddLines (language.Using using)

    member this.Output (nspace: NamespaceModel) = 
        // This was originally built without Result support, and I think we just need
        // to ensure no errors. Thus, just using a try here. 
        try
            this.OutputUsings nspace.Usings
            writer.AddLines (language.NamespaceOpen nspace)
            writer.IncreaseIndent()
            this.OutputClasses nspace.Classes
            writer.DecreaseIndent()
            writer.AddLines (language.NamespaceClose nspace)
            Ok writer
        with 
        | e -> Error e.Message
