module Common

type NamedItem =
    | GenericNamedItem of Name: string * GenericTypes: NamedItem list
    | SimpleNamedItem of Name: string
    static member GenericsFromStrings (name: string) genericsAsStrings =
        genericsAsStrings |> List.map (fun x -> SimpleNamedItem x)
    member this.SimpleName() =
        match this with 
        | SimpleNamedItem name -> name
        | GenericNamedItem (name, t) -> name
    static member Create (name: string) generics =
        match generics with 
        | [] -> SimpleNamedItem name
        | _ -> GenericNamedItem (name, generics)


type Return =
    | Void
    | Type of t: NamedItem
    static member Create typeName =
        match typeName with 
         | "void" -> Void
         | _ -> Type (NamedItem.Create (typeName) [])

