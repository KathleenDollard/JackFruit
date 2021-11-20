﻿module Common

type NamedItem =
    | GenericNamedItem of Name: string * GenericTypes: NamedItem list
    | SimpleNamedItem of Name: string
    static member GenericsFromStrings (name: string) genericsAsStrings =
        genericsAsStrings |> List.map (fun x -> SimpleNamedItem x)
    static member Create (name: string) generics =
        match generics with 
        | [] -> SimpleNamedItem name
        | _ -> GenericNamedItem (name, generics)


type Return =
    | Void
    | Type of t: NamedItem

