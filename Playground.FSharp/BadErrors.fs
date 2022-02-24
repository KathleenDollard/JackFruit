module BadErrors

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

    //// This is an easy mistake to make, especially responding to the above error,
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

//module ErrorIsFarFromProblem =
//    // Do we know there are other offside errors when we get to line 42?
//    // When this happened the methods were quite long and I didn't understand the importance of the offside warning
//    let f1 =

//    let f2 = 
//        //Many lines of methods and code so f3 and f1 were not on screen together
//        []

//    let f3 = 
//        []

//module NestedResultHardToSeee =
    //// Would it ever not be an error to have nested result error types like this. when this happens in a bind its really hard to see
    //let mergeWith listResult currentList =
    //    match listResult with
    //    | Ok list -> Ok (currentList @ list)
    //    | Error err -> err

    //let commandNamesFromModel semanticModel=
    //    Ok [ "A"; "B" ]
    //    |> Result.bind (mergeWith Ok [ "C"; "D" ])
