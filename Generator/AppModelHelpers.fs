namespace Generator

open Generator.Models
open Microsoft.CodeAnalysis
open System.Xml.Linq
open System.Linq
open System

type ItemReturn<'T> =
| NewValue of Value: 'T
| UsePreviousValue

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

    let XmlCommentFromPocketOrMethod (commandDef: CommandDef) (method: IMethodSymbol) =
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
        

    let private DescFromXmlSummary commandDef (method: IMethodSymbol) =
        match XmlCommentFromPocketOrMethod commandDef method with 
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

    
    let private DescFromXmlParam memberDef (commandDef: CommandDef) (method: IMethodSymbol) =
        match XmlCommentFromPocketOrMethod commandDef method with 
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
  
    let private MethodFromCommandDef (commandDef: CommandDef) =
        match commandDef.CommandDefUsage with
        | UserMethod (method, _) -> Some method
        | _ -> None


    let CommandDescFromXmlComment commandDef =
        match MethodFromCommandDef commandDef with
        | Some method -> DescFromXmlSummary commandDef method
        | None -> UsePreviousValue


    let MemberDescFromXmlComment memberDef =
        // This currently fails because it doesn't have a CommandDef to pass. 
        // We either redesign to pass CommandDef to all transforms or we 
        let desc = DescFromXmlParam memberDef 
        UsePreviousValue
        //let GetDesc method = 
        //    match memberDef.MemberDefUsage with
        //    | UserParameter param -> DescFromXmlParam memberDef commandDef method
        //    | _ -> UsePreviousValue

        //match commandDef.CommandDefUsage with
        //| UserMethod (method, _) -> GetDesc method
        //| _ -> UsePreviousValue
 
         
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

