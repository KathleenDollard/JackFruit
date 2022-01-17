module DslForCode

open Dsl
open Generator.LanguageModel
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

    [<CustomOperation("Public2", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPublic(varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetModel varModel 
            { varModel.Model with 
                Scope = Scope.Public
                Modifiers = varModel.Model.Modifiers @ Modifier.CombineModifiers modifier1 modifier2 }

    [<CustomOperation("Private", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPrivate (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetModel varModel 
            { varModel.Model with 
                Scope = Scope.Private 
                Modifiers = varModel.Model.Modifiers @ Modifier.CombineModifiers modifier1 modifier2 }

    [<CustomOperation("Internal", MaintainsVariableSpaceUsingBind = true)>]
    member this.setInternal (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetModel varModel 
            { varModel.Model with 
                Scope = Scope.Internal 
                Modifiers = varModel.Model.Modifiers @ Modifier.CombineModifiers modifier1 modifier2 }

    [<CustomOperation("Friend", MaintainsVariableSpaceUsingBind = true)>]
    member this.setFriend (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetModel varModel 
            { varModel.Model with 
                Scope = Scope.Internal 
                Modifiers = varModel.Model.Modifiers @ Modifier.CombineModifiers modifier1 modifier2 }

    [<CustomOperation("Protected", MaintainsVariableSpaceUsingBind = true)>]
    member this.setProtected (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetModel varModel 
            { varModel.Model with 
                Scope = Scope.Protected 
                Modifiers = varModel.Model.Modifiers @ Modifier.CombineModifiers modifier1 modifier2 }

    [<CustomOperation("ProtectedInternal", MaintainsVariableSpaceUsingBind = true)>]
    member this.setProtectedInternal (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetModel varModel  
            { varModel.Model with 
                Scope = Scope.ProtectedInternal 
                Modifiers = varModel.Model.Modifiers @ Modifier.CombineModifiers modifier1 modifier2 }

    [<CustomOperation("PrivateProtected", MaintainsVariableSpaceUsingBind = true)>]
    member this.setPrivateProtected (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
        this.SetModel varModel  
            { varModel.Model with 
                Scope = Scope.PrivateProtected 
                Modifiers = varModel.Model.Modifiers @ Modifier.CombineModifiers modifier1 modifier2 }

    // KAD!! Test next that you can combine scope and members

    [<CustomOperation("InheritedFrom", MaintainsVariableSpace = true)>]
    member this.inheritedFrom (varModel, [<ProjectionParameter>] inheritedFrom) =
        let inheritedFrom: string = inheritedFrom varModel.Variables
        this.SetModel varModel { varModel.Model with InheritedFrom = SomeBase inheritedFrom }

    //// TODO: Passing a named item will be quite messy. This problemw will recur. Harder if we can't get overloads
    //[<CustomOperation("ImplementedInterfaces", MaintainsVariableSpace = true)>]
    //member _.interfaces (cls: ClassModel, [<ParamArray>]interfaces: ImplementedInterface[]) =
    //    { cls with ImplementedInterfaces = List.ofArray interfaces }

    [<CustomOperation("Members", MaintainsVariableSpace = true)>]
    member _.addMember (cls: ClassModel, memberModels: IMember list) =
        { cls with Members =  List.append cls.Members memberModels }
 
 // Field apparently can't inherit from DslBase??
 type Field(name: string, typeName: NamedItem) =
     inherit DslBase<IMember, unit>()

     override _.Empty() =  FieldModel.Create name typeName
     // KAD: Clean this up. Either put a default in the base or combine the modifier comparison as elsewhere
     override _.CombineModels cls cls2 = cls
 
     override _.NewMember item = invalidOp "What called this?"

     //[<CustomOperation("Public2", MaintainsVariableSpaceUsingBind = true)>]
     //member this.setPublic(varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
     //   this.SetModel varModel 
     //       { varModel.Model with 
     //           Scope = Scope.Public
     //           Modifiers = varModel.Model.Modifiers @ Modifier.CombineModifiers modifier1 modifier2 }

     //[<CustomOperation("Private", MaintainsVariableSpaceUsingBind = true)>]
     //member this.setPrivate (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
     //    this.SetModel varModel 
     //        { varModel.Model with 
     //            Scope = Scope.Private 
     //            Modifiers = varModel.Model.Modifiers @ Modifier.CombineModifiers modifier1 modifier2 }
  
     //[<CustomOperation("Internal", MaintainsVariableSpaceUsingBind = true)>]
     //member this.setInternal (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
     //    this.SetModel varModel 
     //        { varModel.Model with 
     //            Scope = Scope.Internal 
     //            Modifiers = varModel.Model.Modifiers @ Modifier.CombineModifiers modifier1 modifier2 }
  
     //[<CustomOperation("Friend", MaintainsVariableSpaceUsingBind = true)>]
     //member this.setFriend (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
     //    this.SetModel varModel 
     //        { varModel.Model with 
     //            Scope = Scope.Internal 
     //            Modifiers = varModel.Model.Modifiers @ Modifier.CombineModifiers modifier1 modifier2 }
  
     //[<CustomOperation("Protected", MaintainsVariableSpaceUsingBind = true)>]
     //member this.setProtected (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
     //    this.SetModel varModel 
     //        { varModel.Model with 
     //            Scope = Scope.Protected 
     //            Modifiers = varModel.Model.Modifiers @ Modifier.CombineModifiers modifier1 modifier2 }
  
     //[<CustomOperation("ProtectedInternal", MaintainsVariableSpaceUsingBind = true)>]
     //member this.setProtectedInternal (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
     //    this.SetModel varModel  
     //        { varModel.Model with 
     //            Scope = Scope.ProtectedInternal 
     //            Modifiers = varModel.Model.Modifiers @ Modifier.CombineModifiers modifier1 modifier2 }
  
     //[<CustomOperation("PrivateProtected", MaintainsVariableSpaceUsingBind = true)>]
     //member this.setPrivateProtected (varModel, ?modifier1: Modifier, ?modifier2: Modifier) =
     //    this.SetModel varModel  
     //        { varModel.Model with 
     //            Scope = Scope.PrivateProtected 
     //            Modifiers = varModel.Model.Modifiers @ Modifier.CombineModifiers modifier1 modifier2 }
  
