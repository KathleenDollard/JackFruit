namespace Generator

open System.Text

type StringBuilderWriter(indentSize: int) =

    let mutable currentIndent = 0
    let sb = StringBuilder()

    interface IWriter with
        member this.AddLines newLines =
            for line in newLines do
                let space = String.replicate (currentIndent * indentSize) " "
                sb.AppendLine (space + line) |> ignore
            this

        member _.IncreaseIndent() =
            currentIndent <- currentIndent + 1
        member _.DecreaseIndent() =
            currentIndent <- currentIndent - 1

        member _.Output =
            sb.ToString()
