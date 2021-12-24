module Generator.LanguageRoslynOut


open Generator
open Generator.Language
open Generator.LanguageStatements
open Generator.LanguageExpressions


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

    member _.OutputComment comment =
        writer.AddLines (language.Comment comment)

    member _.OutputCompilerDirective pragma =
        writer.AddLines (language.CompilerDirective pragma)

    member this.OutputMethod (method: MethodModel) =
        writer.AddLines (language.MethodOpen method) 
        writer.IncreaseIndent()
        this.OutputStatements method.Statements
        writer.DecreaseIndent()
        writer.AddLines (language.MethodClose method)

    member this.OutputConstructor (ctor: ConstructorModel) =
       writer.AddLines (language.ConstructorOpen ctor) 
       writer.IncreaseIndent()
       this.OutputStatements ctor.Statements
       writer.DecreaseIndent()
       writer.AddLines (language.ConstructorClose ctor)

    member this.OutputProperty (prop : PropertyModel) =
        let isAutoProp = 
            prop.SetStatements.IsEmpty && prop.GetStatements.IsEmpty
        if isAutoProp then
            writer.AddLines (language.AutoProperty prop)
        else
            writer.AddLines (language.PropertyOpen prop)
            writer.IncreaseIndent()
            writer.AddLines (language.GetOpen prop)
            writer.IncreaseIndent()
            this.OutputStatements prop.GetStatements
            writer.DecreaseIndent()
            writer.AddLines (language.GetClose prop)
            writer.AddLines (language.SetOpen prop)
            writer.IncreaseIndent()
            this.OutputStatements prop.SetStatements
            writer.DecreaseIndent()
            writer.AddLines (language.SetClose prop)
            writer.DecreaseIndent()
            writer.AddLines (language.PropertyClose prop)

    member this.OutputField (field: FieldModel) =
        writer.AddLines (language.Field field)

     //member this.OutputMember mbr =
     //   match mbr with 
     //   | Method m -> this.OutputMethod m
     //   | Property p -> this.OutputProperty p
     //   | Class c -> this.OutputClass c
     //   | Field f -> this.OutputField f
     //   | Constructor c -> this.OutputConstructor c

    member this.OutputMembers (members: IMember list) =
        for mbr in members do 
            ()
            //this.OutputMember mbr

    member this.OutputClass cls =
        writer.AddLines (language.ClassOpen cls)
        writer.IncreaseIndent()
        this.OutputMembers cls.Members
        writer.DecreaseIndent()
        writer.AddLines (language.ClassClose cls)

    member this.OutputClasses (classes: ClassModel list) =
        for cls in classes do 
            this.OutputClass cls

    member _.OutputUsings (usings: UsingModel list) =
        for using in usings do 
            writer.AddLines (language.Using using)

    member this.Output (nspace: NamespaceModel) = 
        this.OutputUsings nspace.Usings
        writer.AddLines (language.NamespaceOpen nspace)
        writer.IncreaseIndent()
        this.OutputClasses nspace.Classes
        writer.DecreaseIndent()
        writer.AddLines (language.NamespaceClose nspace)
        writer
