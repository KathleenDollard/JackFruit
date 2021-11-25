module rec Generator.CodeCSharp

open Generator.Language
open GeneralUtils


let private ScopeOutput scope =
    match scope with 
    | Public -> "public"
    | Private -> "private"
    | Internal -> "internal"

let StaticOutput staticOrInstance = 
    match staticOrInstance with 
    | Instance -> ""
    | Static -> " static"

let OutputStatement (sb: SpaceStringBuilder) (statement: Statement) =
    ()


let OutputMethod (sb: SpaceStringBuilder) (mbr: Method) =
    // KAD-Don: It seems super ugly to need all these ignores. Is there a different design for SpaceStringBuilder?
    let _ = sb.AppendLine $"{ScopeOutput mbr.Scope}{StaticOutput mbr.StaticOrInstance} {mbr.ReturnType}"
    let _ = sb.AppendLine "{"
    let _ = sb.IncreaseIndent
    for statement in mbr.Statements do
        OutputStatement sb statement
    let _ = sb.DecreaseIndent 
    let _ = sb.AppendLine "}"
    ()

let OutputProperty (sb: SpaceStringBuilder) mbr =

    ()

let OutputMember (sb: SpaceStringBuilder) mbr =
    match mbr with 
    | Method m -> OutputMethod sb m
    | Property p -> OutputProperty sb p
    | Class c -> OutputClass sb c


let OutputClass (sb: SpaceStringBuilder) cls =
    let _ = sb.AppendLine $"{ScopeOutput cls.Scope}{StaticOutput cls.StaticOrInstance} class"
    let _ = sb.AppendLine "{"
    let _ = sb.IncreaseIndent
    for mbr in cls.Members do
        OutputMember sb mbr
    let _ = sb.DecreaseIndent 
    let _ = sb.AppendLine "}"
    ()

let Output spaces (nspace: Namespace) = 
    let sb = new SpaceStringBuilder(spaces)

    let _ = sb.AppendLine $"namespace {nspace.NamespaceName}"
    let _ = sb.AppendLine "{"
    let _ = sb.IncreaseIndent
    for cls in nspace.Classes do
        OutputClass sb cls
    let _ = sb.DecreaseIndent 
    let _ = sb.AppendLine "}"
    sb.ToString()

