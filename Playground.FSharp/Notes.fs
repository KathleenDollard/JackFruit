module Notes

open System.Runtime.CompilerServices
open System

// KAD: Without the parens, the constructor below is private. Also, this is a C# extension method. Also, int requires parens the dot is currently interpretted as a float.
[<Extension>]
type StringExtensions = 
    [<Extension>]
    static member A(t: int) =
        0


type String with
    member this.ToString() =
        "Does this work"