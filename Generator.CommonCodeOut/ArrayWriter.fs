namespace Generator

open System.Text

// This writer is used to make testing easier. The StringBuilder
// writer is much more efficeint and expected to be used, except for testing
type ArrayWriter(indentSize: int) =

    let mutable currentIndent = 0
    let mutable (linePairs: (struct (int * string)) list) = []

    member _.LinePairs() = linePairs

    interface IWriter with

        member _.AddLine newLine =
            linePairs <- List.append linePairs [ (currentIndent, newLine) ]

        member this.AddLines newLines =
            for line in newLines do
                (this :> IWriter).AddLine line

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
