namespace Generator

open System.Text

// This writer is used to make testing easier. The StringBuilder
// writer is much more efficeint and expected to be used, except for testing
type ArrayWriter(indentSize: int) =

    let mutable currentIndent = 0
    let mutable linePairs = []

    interface IWriter with
        member this.AddLines newLines =
            let withIndent = newLines |> List.map (fun x -> (currentIndent, x))
            linePairs <- List.append linePairs withIndent
            this

        member _.IncreaseIndent() =
            currentIndent <- currentIndent + 1
        member _.DecreaseIndent() =
            currentIndent <- currentIndent - 1

        // Output is flat because 
        member _.Output  =
            let sb = StringBuilder()
            for (indentCount, text) in linePairs do
                let space = String.replicate (indentCount * indentSize) " "
                sb.AppendLine (space + text) |> ignore
            sb.ToString()
