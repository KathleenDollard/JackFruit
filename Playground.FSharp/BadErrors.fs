﻿module BadErrors

open System

// KAD-Don-Chet: Several questions here
module ExtraParamInBinding =
    let f1 i = Ok i
    let f2 i = Ok (i + 1)
    let f3 i j = Ok (i + j)
    // TODO: Test
    //// Why does the following not complain about inferrence? "Either define 'x' as a simple data term, make it a function with explicit arguments or, if you do not intend for it to be generic, add a type annotation.
    //let x = 
    //    f1 42
    //    |> Result.bind f2

    //// This is an easy mistake to make, especially responding to the above error, the error is awful
    //let (x:int) = 
    //    f1 42
    //    |> Result.bind f2

    //// This error is pretty hard to understand
    //let (x:Result<int, string>) = 
    //    f1 42
    //    |> Result.bind f2 5

    //// This is a variation that is even more difficult, because the issue is a lack of parens
    //let (x:Result<int, string>) = 
    //    f1 42
    //    |> Result.bind f3 5

    //Console.WriteLine(x)