namespace Generator

open System.Text

type StringBuilderWriter(indentSize: int) =

    let mutable currentIndent = 0
    let sb = StringBuilder()

    interface IWriter with

        member this.AddLine newLine =
            let space = String.replicate (currentIndent * indentSize) " "
            sb.AppendLine (space + newLine) |> ignore

        member this.AddLines newLines =
            for line in newLines do
                (this :> IWriter).AddLine line

        member _.IncreaseIndent() =
            currentIndent <- currentIndent + 1
        member _.DecreaseIndent() =
            currentIndent <- currentIndent - 1

        member _.Output =
            sb.ToString()
