module DslCodeBuilder

open System
open Generator.Language

type NamespaceBuilder() =
    member _.Zero() = Some (NamespaceModel.Default())
    member _.Yield (v:NamespaceModel) = Some v
    member _.For m f = Option.bind f m

    [<CustomOperation("Using")>]
    member _.addUsing (state: NamespaceModel option, using: UsingModel): NamespaceModel option =
        match state with 
        | Some x -> Some (x.AddUsing using)
        | None -> None

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
