module DslForCode

open Dsl
open Generator.Language
open Common
open System
open DslKeywords


[<AbstractClass>]
type StatementBuilderBase<'T>() =
    inherit DslBase<'T, IStatement>()

[< AbstractClass >]
type MethodBase<'T>() =
    inherit StatementBuilderBase<'T>()

type Namespace(name: string) =
    inherit DslBase<NamespaceModel, ClassModel>()

    override _.Empty() : NamespaceModel =  NamespaceModel.Create name

    override this.NewMember item =
        { this.Empty() with Classes = [ item ] }

    override _.CombineModels nspace nspace2 : NamespaceModel =
        let newName = 
            if String.IsNullOrWhiteSpace nspace.NamespaceName then nspace2.NamespaceName
            else nspace.NamespaceName
        { nspace with 
            NamespaceName = newName
            Usings =  List.append nspace.Usings nspace2.Usings
            Classes = List.append nspace.Classes nspace2.Classes }  

    [<CustomOperation("Name", MaintainsVariableSpaceUsingBind = true)>]
    member this.setName (varModel, [<ProjectionParameter>] name)   =
        this.SetModel varModel { varModel.Model with NamespaceName = name varModel.Variables }

    // KAD-Don: Uncommenting this gives error at call site in LanguageDslTests
    //[<CustomOperation("Using", MaintainsVariableSpaceUsingBind = true)>]
    //member this.setUsing (varModel, [<ProjectionParameter>] using)   =
    //    let value: string = using varModel.Variables
    //    let newUsing = UsingModel.Create(value)
    //    this.SetModel varModel { varModel.Model with Usings = varModel.Model.Usings @ [ newUsing ] }

    [<CustomOperation("Using", MaintainsVariableSpaceUsingBind = true)>]
    member this.setUsing (varModel, [<ProjectionParameter>] using, [<ProjectionParameter>] alias)   =
        let usingName: string = using varModel.Variables
        let aliasValue: string = alias varModel.Variables
        let alias = 
            if String.IsNullOrWhiteSpace aliasValue then None
            else Some aliasValue
        let newUsing = { UsingNamespace = usingName; Alias = alias }
        this.SetModel varModel { varModel.Model with Usings = varModel.Model.Usings @ [ newUsing ] }


type Class(name: string) =
    inherit DslBase<ClassModel, IMember>()

    override _.Empty() =  ClassModel.Create name

    override this.NewMember item =
        { this.Empty() with Members = [ item ] }

    override _.CombineModels cls1 cls2 =
        let newScope = 
            match cls1.Scope with 
            | Unknown -> cls2.Scope
            | _ -> cls1.Scope  
        let newInheritedFrom =
            match (cls1.InheritedFrom, cls2.InheritedFrom) with
            | (NoBase, NoBase) -> NoBase
            | (SomeBase baseClass, NoBase) -> SomeBase baseClass
            | (NoBase, SomeBase baseClass) -> SomeBase baseClass
            | (_, SomeBase baseClass) -> SomeBase baseClass
        { cls1 with 
            Scope = newScope
            Modifiers = List.append cls1.Modifiers cls2.Modifiers
            InheritedFrom = newInheritedFrom
            ImplementedInterfaces = List.append cls1.ImplementedInterfaces cls2.ImplementedInterfaces
            Members =  List.append cls1.Members cls2.Members }  

    //[<CustomOperation("Public", MaintainsVariableSpaceUsingBind = true)>]
    //member this.setPublic (varModel) =
    //    this.SetModel varModel { varModel.Model with Scope = Scope.Public }

    [<CustomOperation("Public2", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPublic 
          ( varModel, 
            ?modifier1: Modifier, 
            ?modifier2: Modifier) =
        let modifiers =
            [ match modifier1 with | Some m -> m | None -> ()
              match modifier2 with | Some m -> m | None -> () ]
        this.SetModel varModel 
            { varModel.Model with 
                Scope = Scope.Public
                Modifiers = varModel.Model.Modifiers @ modifiers }

    [<CustomOperation("Private", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPrivate (varModel) =
        this.SetModel varModel { varModel.Model with Scope = Scope.Private }

    [<CustomOperation("Internal", MaintainsVariableSpaceUsingBind = true)>]
    member this.setInternal (varModel) =
        this.SetModel varModel { varModel.Model with Scope = Scope.Internal }

    [<CustomOperation("Friend", MaintainsVariableSpaceUsingBind = true)>]
    member this.setFriend (varModel) =
        this.setInternal varModel

    [<CustomOperation("Protected", MaintainsVariableSpaceUsingBind = true)>]
    member this.setProtected (varModel) =
        this.SetModel varModel { varModel.Model with Scope = Scope.Protected }

    [<CustomOperation("ProtectedInternal", MaintainsVariableSpaceUsingBind = true)>]
    member this.setProtectedInternal (varModel) =
        this.SetModel varModel { varModel.Model with Scope = Scope.ProtectedInternal }

    [<CustomOperation("PrivateProtected", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPrivateProtected (varModel) =
        this.SetModel varModel { varModel.Model with Scope = Scope.PrivateProtected }



    //// TODO: Passing a named item will be quite messy. This problemw will recur. Harder if we can't get overloads
    //[<CustomOperation("InheritedFrom", MaintainsVariableSpace = true)>]
    //member _.inheritedFrom (cls: ClassModel, inheritedFrom: NamedItem) =
    //    // Consider whether resetting this to None is a valid scenario
    //    { cls with InheritedFrom = SomeBase inheritedFrom }

    //// TODO: Passing a named item will be quite messy. This problemw will recur. Harder if we can't get overloads
    //[<CustomOperation("ImplementedInterfaces", MaintainsVariableSpace = true)>]
    //member _.interfaces (cls: ClassModel, [<ParamArray>]interfaces: ImplementedInterface[]) =
    //    { cls with ImplementedInterfaces = List.ofArray interfaces }

    //[<CustomOperation("Generics", MaintainsVariableSpace = true)>]
    //member _.generics (cls: ClassModel, generics: NamedItem list) =
    //    let currentName =
    //        match cls.ClassName with 
    //        | SimpleNamedItem n -> n
    //        | GenericNamedItem (n, _ ) -> n
    //    { cls with ClassName = NamedItem.Create currentName generics }

    //[<CustomOperation("Members", MaintainsVariableSpace = true)>]
    //member _.addMember (cls: ClassModel, memberModels: IMember list) =
    //    { cls with Members =  List.append cls.Members memberModels }
 

