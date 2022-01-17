namespace Dsl


type M<'T, 'Vars> =
    { Model: 'T
      Variables: 'Vars }

type M<'T> = M<'T, unit>


[<AbstractClass>]
type DslBase<'T, 'TItem> () =
    abstract member Empty: unit -> 'T
    abstract member CombineModels: 'T -> 'T -> 'T
    abstract member NewMember: 'TItem -> 'T

    member this.Zero() : M<'T, unit> = 
        { Model = this.Empty() 
          Variables = () }

    member this.Combine (varModel1: M<'T, unit>, varModel2: M<'T, unit>) : M<'T, unit> =
        { Model = this.CombineModels varModel1.Model varModel2.Model
          Variables = () }

    member _.Delay(f) : M<'T, 'Vars> = f()

    member this.Run(varModel: M<'T, 'Vars>) : M<'T, unit> =
        { Model = this.CombineModels varModel.Model (this.Empty())
          Variables = () }

    member this.For(methods, f) :M<'T, unit> = 
        let methodList = Seq.toList methods
        match methodList with 
        | [] -> this.Zero()
        | [x] -> f(x)
        | head::tail ->
            let mutable headResult = f(head)
            for x in tail do 
                headResult <- this.Combine(headResult, f(x))
            headResult

    // This is needed to unpack children that are
    member this.Yield (wrappedItem: M<'TItem, 'Vars>) : M<'T, unit> = 
        let item = wrappedItem.Model
        { Model = this.NewMember item
          Variables = () }

    member this.Yield (item: 'TItem) : M<'T, unit> = 
        { Model = this.NewMember item
          Variables = () }

    // Only for packing/unpacking the implicit variable space
    member this.Bind (varModel1: M<'T, 'Vars>, f: ('Vars -> M<'T, unit>)) : M<'T, unit>  =
        let varModel2 = f varModel1.Variables
        let combined = this.CombineModels varModel1.Model varModel2.Model
        { Model = combined
          Variables = varModel2.Variables }

    // Only for packing/unpacking the implicit variable space
    member this.Return (varspace: 'Vars) : M<'T, 'Vars> = 
        { Model = this.Empty() 
          Variables = varspace }

    member _.SetModel (varModel: M<'T, 'Vars>) (newModel: 'T) =
        { Model = newModel
          Variables = varModel.Variables }            

