module Generator.AppModelHelpers

open Generator.Models
open Microsoft.CodeAnalysis
open System.Xml.Linq
open System.Linq
open System
open XmlCommentPatterns


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
        match xml with 
        | XmlCommentSummary summary -> 
            if String.IsNullOrWhiteSpace summary then 
                UsePreviousValue
            else
                NewValue (Some summary)
        | _ -> UsePreviousValue


let MemberDescFromXmlComment (memberDef: MemberDef) =
    match XmlCommentFromPocketOrMethod memberDef.CommandDef with 
    | None -> UsePreviousValue
    | Some xml -> 
        match xml with 
        | XmlCommentParamDesc memberDef.MemberId desc -> 
            if String.IsNullOrWhiteSpace desc then 
                UsePreviousValue
            else
                NewValue (Some desc)
        | _ -> UsePreviousValue

         
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
    | UserMethod (_, _) -> UsePreviousValue
    | _ -> UsePreviousValue

