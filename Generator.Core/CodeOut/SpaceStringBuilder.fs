namespace Generator

open System.Text


type SpaceStringBuilder(spacesForIndent: int) =
    let sb = new StringBuilder()
    let mutable indentSize = spacesForIndent
    let mutable indentLevel = 0
      
    member _.AppendLine line = 
        let spaceCount = indentSize * indentLevel
        let spaces = String.replicate spaceCount " "
        sb.AppendLine(spaces + line)

    member _.AppendLines lines list = 
        let spaceCount = indentSize * indentLevel
        let spaces = String.replicate spaceCount " "
        for line in lines do
            sb.AppendLine(spaces + line) |> ignore
        ()
        
    member _.IncreaseIndent =
        indentLevel <- indentLevel + 1

    member _.DecreaseIndent =
        if indentLevel > 1 then
            indentLevel <- indentLevel - 1

    override _.ToString() =
        sb.ToString()

