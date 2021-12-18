module DslCodeBuilder

// KAD-Don: What is this syntax (from:https://putridparrot.com/blog/my-first-attempt-as-implementing-a-computational-expression-in-f/
//     let (Items items) = curves
//     Specifically, the part in parens. A deconstructing cast from context?

open System
open Generator.Language
open Common

type AliasWord =
| Alias

type StaticWord =
| Static

 //KAD-Don: I want to do the following but get an error. Tomas had an example that looked like this, so I want 
 //         to check if I really can't do this before I abandon. https://stackoverflow.com/questions/5745172/f-overload-functions
//          Or, can I do optional overloads for this. Tuples would make the DSL ugly
//type DslHelpers =
//    static member Using (name: string) (_: AliasWord) (alias: string): UsingModel =
//        if String.IsNullOrWhiteSpace alias then
//            { Namespace = name; Alias = None } 
//        else
//            { Namespace = name; Alias = Some alias } 
//    static member Using (name: string) = { Namespace = name; Alias = None } 

type NamespaceBuilder(name: string) =

    member _.Yield (()) =  NamespaceModel.Create(name)

    [<CustomOperation("Usings", MaintainsVariableSpace = true)>]
    member _.addUsings (nspace: NamespaceModel, usings): NamespaceModel =
        nspace.AddUsings usings

    [<CustomOperation("Using", MaintainsVariableSpace = true)>]
    member _.addUsingWithAlias (nspace: NamespaceModel, name: string, _:AliasWord, alias: string): NamespaceModel =
        let newUsing =
            if String.IsNullOrWhiteSpace alias then
                { Namespace = name; Alias = None } 
            else
                { Namespace = name; Alias = Some alias } 
        nspace.AddUsings [ newUsing ]
        
    [<CustomOperation("Using", MaintainsVariableSpace = true)>]
    member this.addUsing (nspace: NamespaceModel, name: string): NamespaceModel =
        this.addUsingWithAlias(nspace, name, Alias, "")
  
    [<CustomOperation("Classes", MaintainsVariableSpace = true)>]
    member _.addClasses (nspace: NamespaceModel, classes): NamespaceModel =
        nspace.AddClasses classes


type ClassBuilder(name: string) =
      
    let updateModifiers (cls: ClassModel) scope staticOrInstance  =
        { cls with Scope = scope; StaticOrInstance = staticOrInstance }
        
    member _.Yield (_) = ClassModel.Create(name, Public, [])

    // TODO: Passing a named item will be quite messy. This problemw will recur. Harder if we can't get overloads
    [<CustomOperation("Generics", MaintainsVariableSpace = true)>]
    member _.generics (cls: ClassModel, generics: NamedItem list) =
        let currentName =
            match cls.ClassName with 
            | SimpleNamedItem n -> n
            | GenericNamedItem (n, _ ) -> n
        { cls with ClassName = NamedItem.Create currentName generics }

    // TODO: Add async and partial to class model and here
    [<CustomOperation("Public", MaintainsVariableSpace = true)>]
    member _.modifiers (cls: ClassModel) =
        updateModifiers cls Public Instance

    [<CustomOperation("Public", MaintainsVariableSpace = true)>]
    member _.modifiersWithStatic (cls: ClassModel) (_: StaticWord): ClassModel =
        updateModifiers cls Public StaticOrInstance.Static

    //    TODO: [<CustomOperation("Private")>], etc

    // TODO: Passing a named item will be quite messy. This problemw will recur. Harder if we can't get overloads
    [<CustomOperation("InheritedFrom", MaintainsVariableSpace = true)>]
    member _.inheritedFrom (cls: ClassModel, inheritedFrom: NamedItem option) =
        { cls with InheritedFrom = inheritedFrom }

    // TODO: Passing a named item will be quite messy. This problemw will recur. Harder if we can't get overloads
    [<CustomOperation("Interfaces", MaintainsVariableSpace = true)>]
    member _.interfaces (cls: ClassModel, interfaces: NamedItem list) =
        { cls with ImplementedInterfaces = interfaces }

    [<CustomOperation("Members", MaintainsVariableSpace = true)>]
    member _.members (cls: ClassModel, members: Member list) =
        { cls with Members = members }





//type ClassModel = unit
//type FieldModel = unit
//type ClassBuilder () =
//    let mutable count = 0
//    member x.Zero (): unit  = () // this is our state for the CE run
//    member x.Yield (v) = v
    
//    // Combine allows you to integrate some kind of external state 
//    // into your 'base' state
    
//    member x.Combine(state: ClassModel, field: FieldModel) = 
//        state.AddField field
    
//    member x.Combine(state: ClassModel, method: MethodModel) = 
//        state.AddMethod method
        
//    [<CustomOperation("Field")>]
//    member x.Field(state: unit, scope: string, name: string, ty: System.Type) = 
//        // add these field members to some backing state
//        ()
    
    
//type ModelClassBuilder() = 
//    inherit ClassBuilder ()
    
//    member x.Run(state: unit) = state
    
    
//let Class = ModelClassBuilder()

//let Field = //FieldBuilder()

//let thingClass = 
//    Class {
//      Field {
//          scope "public"
//          name "Thing"
//          typ typeof<int>
//      }
//      Method {
          
//      }
//    }
