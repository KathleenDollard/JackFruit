namespace Generator

open Generator.Models
open Microsoft.CodeAnalysis
open System.Xml.Linq
open System.Linq
open System

module AppModelHelpers =

    let DescriptionFromLookup mapOption (commandDef: CommandDef) =
        let key = commandDef.PathString
        match mapOption with 
        | None -> UsePreviousValue
        | Some map -> 
            let value = Map.tryFind key map
            match value with 
            | Some value -> NewValue (Some value)
            | None -> UsePreviousValue


    let private MethodFromCommandDef (commandDef: CommandDef) =
        match commandDef.CommandDefUsage with
        | UserMethod (method, _) -> Some method
        | _ -> None


    let private XmlCommentFromPocketOrMethod (commandDef: CommandDef) =
        match MethodFromCommandDef commandDef with
        | None -> None
        | Some method -> 
            // The expensive part is the parsing, so we do not want to repeat it for every parameter
            let cacheKey = "XmlComment"
            let cached = commandDef.Pocket(cacheKey)

            match cached with 
            | Some xmlAsObject -> 
                match xmlAsObject with 
                | :? XDocument as xml -> Some xml
                | _ -> None
            | None -> 
                let xmlString = method.GetDocumentationCommentXml()
                if String.IsNullOrEmpty xmlString then
                    None
                else
                    let newXml = XDocument.Parse(xmlString)
                    commandDef.AddToPocket cacheKey newXml
                    Some newXml
        
   
    let CommandDescFromXmlComment commandDef =
        match XmlCommentFromPocketOrMethod commandDef with 
        | None -> UsePreviousValue
        | Some xml -> 
            if xml.Root.HasElements then
                let methodRoot = xml.Elements().First()
                let summaries = 
                    [ for node in methodRoot.Elements() do
                        if node.Name = XName.Get("summary") then
                            let textNode = node.FirstNode
                            if textNode <> null then
                                let desc = textNode.ToString().Trim()
                                desc ]
                match summaries with 
                | [] -> UsePreviousValue
                | head::_ -> if String.IsNullOrEmpty head then UsePreviousValue else NewValue (Some head)
            else
                UsePreviousValue


    let MemberDescFromXmlComment (memberDef: MemberDef) =
        let matchingElement (elements: XElement seq) name =
            let matches =
                [ for element in elements do
                  let attributes = 
                     element.Attributes("name")
                     |> Seq.where (fun x -> x.Value = name)
                  if Seq.isEmpty attributes then
                     None
                  else
                     Some (element.ToString()) ]
            match matches with 
            | [] -> UsePreviousValue
            | head::_ -> NewValue head

        match XmlCommentFromPocketOrMethod memberDef.CommandDef with 
        | None -> UsePreviousValue
        | Some xml -> 
            if xml.Root.HasElements then
                matchingElement (xml.Elements()) memberDef.MemberId
            else
                UsePreviousValue


         
    let DescFromAttribute (roslynSymbol: ISymbol) = 
        let attributes = 
            [ for attr in roslynSymbol.GetAttributes() do
                if attr.AttributeClass.Name = "DescriptionAttribute" then
                    let arg = attr.ConstructorArguments.First()
                    arg.Value ]
        match attributes with 
        | [] -> UsePreviousValue
        | head:: _ -> NewValue (Some (head.ToString()))

    let CommandDescFromAttribute (commandDef: CommandDef) =
        match commandDef.CommandDefUsage with
        | UserMethod (method, _) -> DescFromAttribute method
        | _ -> UsePreviousValue

    let MemberDescFromAttribute (memberDef: MemberDef) =
        match memberDef.MemberDefUsage with
        | UserParameter parameter -> DescFromAttribute parameter
        | _ -> UsePreviousValue


    // Do we care? If we only use description to create Symbols, we do not care. 
    let DescriptionFromSymbol (commandDef: CommandDef) =
        let descriptionFromSymbol(symbol) =
            UsePreviousValue

        match commandDef.CommandDefUsage with 
        | SetHandlerMethod (_, _, symbol) -> descriptionFromSymbol symbol
        | _ -> UsePreviousValue

