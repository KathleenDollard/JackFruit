module XmlCommentPatterns

open System.Xml.Linq

let private ItemRoot (xDoc: XDocument) =
    match xDoc.Root with
    | null -> None
    | _ -> Some xDoc.Root


// This is private because the name is confusing. It isn't an attribute with the name, but 
// an attribute with the matching value an an attribute named name. 
let private AttributesWithName (element: XElement) name =
    let xName = XName.Get(name)
    let test = element.Attributes(XName.Get("name"))
    let test2 = element.Attributes(name)
    [ for attr in element.Attributes("name") do
        if attr.Value = name then 
            attr ]


// This is private because there must be a better way, and naming is poor
let private ElementContents (element: XElement) =
    let firstNode = element.FirstNode
    if firstNode = null then
        None
    else
        let desc = firstNode.ToString().Trim()
        Some desc


let (|XmlCommentSummary|_|) (xDoc: XDocument) =
    match ItemRoot xDoc with
    | None -> None
    | Some  root ->
        let summaries = root.Elements("summary") 
        if Seq.isEmpty summaries then 
            None
        else
            let summary = Seq.head summaries
            ElementContents summary


// This is a separate method so that other AppModels can use this code
let (|XmlCommentParamElemWithName|_|) name (xDoc: XDocument)  =
    match ItemRoot xDoc with
    | None -> None
    | Some  root ->
        let elements =
            [ for element in root.Elements("param") do
                match AttributesWithName element name with 
                | [] -> ()
                | head::_ when name = head.Value -> element
                | _ -> () ]
        List.tryHead elements


let (|XmlCommentParamDesc|_|) name (xDoc: XDocument) =
    match xDoc with
    | XmlCommentParamElemWithName name item -> ElementContents item
    | _ -> None

        
        