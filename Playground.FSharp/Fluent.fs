module Fluent

open FSharp.Core.Fluent

let xs = [1 .. 10]
let xsf = [ for x in xs do 
            if x > 3 then x + 1 ]
            .sort()