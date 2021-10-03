module FSharp6

    let list = [1..10]

     // from docs
    // Generate a list of 100 integers
    let fullList = [ 1 .. 100 ]
    printfn $"{fullList.Length}"
    
    let smallSlice = fullList.[1..3]
    printfn $"fullList.[1..3] F#: {smallSlice}"
    let smallSlice2 = fullList.[2..3]
    printfn $"fullList.[2..3] F#: {smallSlice2}"
    let unboundedBeginning = fullList.[..2]
    printfn $"fullList.[..2] F#: {unboundedBeginning}"
    let unboundedEnd = fullList.[98..]
    printfn $"fullList.[98..] F#: {unboundedEnd}"
    //let fromEnd = fullList.[^2..]
    //printfn $"fullList.[^2..] F#: {fromEnd}"
    
    //let x = list.[^2..]
    //let Test = 
    //    printfn $"list.[^2..] F#: {x}"
    //    printfn ""

