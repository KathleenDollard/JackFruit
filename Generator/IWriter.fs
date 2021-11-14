namespace Generator


type IWriter = 
    abstract member AddLine: string  -> unit
    abstract member AddLines: string list -> unit
    abstract member IncreaseIndent: unit -> unit
    abstract member DecreaseIndent: unit -> unit
    abstract member Output: string

