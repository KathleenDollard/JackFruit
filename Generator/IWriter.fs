namespace Generator


type IWriter = 
    abstract member AddLines: string list -> IWriter
    abstract member IncreaseIndent: unit -> unit
    abstract member DecreaseIndent: unit -> unit
    abstract member Output: string