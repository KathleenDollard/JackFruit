module Fluent

open FSharp.Core.Fluent

let xs = [ 1 .. 10 ]

let xsf =
    [| for x in xs do
           if x > 3 then x + 1 |]
        .sort ()

let xsf2 = List.map (fun x -> x + 1) xs

let xsf3 = xs |> List.map (fun x -> x + 1)

let xsf4 =
    xs
    |> List.map (fun x -> x + 1)
    |> List.filter (fun x -> x > 4)

let xsf5 = xs.map(fun x -> x + 1).filter(fun x -> x > 4)

let mutable x = 4
x <- 5