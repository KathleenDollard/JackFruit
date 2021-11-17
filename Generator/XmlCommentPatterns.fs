module XmlCommentPatterns

open System.Xml.Linq

let private ItemElement (xDoc: XDocument) =
    match xDoc.Root.FirstNode with 
    | :? XElement as x -> Some x // Consider testing for one of hte prefixes, assume one
    | _ -> None


let AttributesWithName (element: XElement) name =
    element.Attributes(XName.Get(name))
    |> List.ofSeq




// This is private because there must be a better way, and naming is poor
let private ElementContents (element: XElement) =
    let firstNode = element.FirstNode
    if firstNode = null then
        None
    else
        let desc = firstNode.ToString().Trim()
        Some desc


let (|XmlCommentSummary|_|) (xDoc: XDocument) =
    match ItemElement xDoc with
    | None -> None
    | Some  item ->
        let summaries = item.Elements("summary") 
        if Seq.isEmpty summaries then 
            None
        else
            let summary = Seq.head summaries
            ElementContents summary


let (|XmlCommentParamElemWithName|_|) name (xDoc: XDocument)  =
    match ItemElement xDoc with
    | None -> None
    | Some  item ->
        let elements =
            [ for element in item.Elements("param") do
                match AttributesWithName element name with 
                | [] -> ()
                | head::_ when name = head.Value -> element
                | _ -> () ]
        List.tryHead elements


let (|XmlCommentParamDesc|_|) name (xDoc: XDocument) =
    match xDoc with
    | XmlCommentParamElemWithName name item -> Some (ElementContents item)
    | _ -> None

        
        