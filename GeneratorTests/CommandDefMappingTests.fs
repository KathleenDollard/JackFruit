module Generator.Tests.CommandDefMappingTests

open Xunit
open FsUnit.Xunit
open Generator.Tests.UtilsForTests
open Generator.Tests
open Generator.NewMapping
open Generator
open BuildCodePattern
open Generator.Language
open ApprovalTests
open ApprovalTests.Reporters
open Generator.Transforms
open UtilsForTests

// I'm not sure what we should be testing first
//  * Creating CommandDef from random method (per most APpModels) : ``When building CommandDefs``
//  * Creating CommandDef from SetHandler (Kevin's model)
//  * Creating a CommandDef by hand and testing providers
//  * Creating a CommandDef by hand and generating Kevin's code
//  * Something else 

type ``When building CommandDefs``() =

    let TestCommandDefFromSource map =
        let expected = map.CommandDef
        let actual = CommandDefFromHandlerSource map.HandlerCode
    
        let differences = (CommandDefDifferences expected actual)

        match differences with 
        | None -> () // All is great!
        | Some issues -> 
            raise (MatchException (expected.ToString(), actual.ToString(), String.concat "\r\n" issues))


    [<Fact>]
    member _.``One simple comand built``() =
        TestCommandDefFromSource MapData.OneSimpleMapping

    [<Fact>]
    member _.``One complex comand built``() =
        TestCommandDefFromSource MapData.OneComplexMapping

    [<Fact>]
    member _.``Three simple commands built``() =
        TestCommandDefFromSource MapData.ThreeMappings

    [<Fact>]
    member _.``No command does noto throw``() =
        TestCommandDefFromSource MapData.NoMapping


type ``When outputting code from CommandDef``() =
    let cSharp = LanguageCSharp() :> ILanguage
    let outputter = RoslynOut (cSharp, ArrayWriter(3))

    let OutputCodeFromCommandDef mapData =
        let commandDefs = mapData.CommandDef
        let codeModelResult = OutputCommandWrapper commandDefs
        match codeModelResult with 
        | Ok codeModel -> outputter.Output codeModel
        | Error _ -> invalidOp "Failed creating code model"

    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``Code outputs for one simple command``() =
        let writer = OutputCodeFromCommandDef MapData.OneSimpleMapping
        let actual = writer.Output
        Approvals.Verify(actual)

    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``Code outputs for three simple commands``() =
        let writer = OutputCodeFromCommandDef MapData.ThreeMappings
        let actual = writer.Output
        Approvals.Verify(actual)

    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``No command does not throw``() =
        let writer = OutputCodeFromCommandDef MapData.NoMapping
        let actual = writer.Output
        Approvals.Verify(actual)

type ``When outputting code from handler code``() =
    let cSharp = LanguageCSharp() :> ILanguage
    let outputter = RoslynOut (cSharp, ArrayWriter(3))

    let OutputCodeFromCode mapData =
        let commandDefs = CommandDefFromHandlerSource mapData.HandlerCode
        let codeModelResult = OutputCommandWrapper commandDefs
        match codeModelResult with 
        | Ok codeModel -> outputter.Output codeModel
        | Error _ -> invalidOp "Failed building code model"

    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``Code outputs for one simple command``() =
        let writer = OutputCodeFromCode MapData.OneSimpleMapping
        let actual = writer.Output
        Approvals.Verify(actual)

    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``Code outputs for three simple commands``() =
        let writer = OutputCodeFromCode MapData.ThreeMappings
        let actual = writer.Output
        Approvals.Verify(actual)

    [<Fact>]
    [<UseReporter(typeof<DiffReporter>)>]
    member _.``No command does not throw``() =
        let writer = OutputCodeFromCode MapData.NoMapping
        let actual = writer.Output
        Approvals.Verify(actual)


                
type ``Transforms for descriptions``() =
    let ApplyTransformsToCommandDefs handlerCode (transforms: Transformer list) = 
        let model, methods = MethodSymbolsFromSource [handlerCode]

        let commandDefs = 
            [ for method in methods do
                CommandDefFromMethod model {InfoCommandId = None; Method = Some method; Path = []; ForPocket = []} ]
        [ for commandDef in commandDefs do 
            let mutable newCommandDef = commandDef
            for transform in transforms do
                newCommandDef <- ApplyTransform newCommandDef transform
            newCommandDef ]

    let commandDesc = "Command Description"
    let memberDesc = "Member Description"
    
    [<Fact>]
    member _.``No transformers do not throw``() =
        let commandDefs =
            ApplyTransformsToCommandDefs 
                "" 
                [DescriptionsFromXmlCommentsTransformer()]
        Assert.Empty (commandDefs)

    [<Fact>]
    member _.``XmlComment xform finds command description``() =
        let commandDef =
            ApplyTransformsToCommandDefs 
                @$"/// <summary>
                 /// {commandDesc}
                 /// </summary>
                 /// <param name=""one""></param>
                 public static void A(string one) {{}}"
                [DescriptionsFromXmlCommentsTransformer()]
            |> List.head
        Assert.Equal (Some commandDesc, commandDef.Description)
        Assert.Equal (None, commandDef.Members.Head.Description)

    [<Fact>]
    member _.``XmlComment xform finds member description``() =
         let commandDef =
            ApplyTransformsToCommandDefs 
                @$"/// <summary>
                 /// 
                 /// </summary>
                 /// <param name=""one"">{memberDesc}</param>
                 public static void A(string one) {{}}"
                [DescriptionsFromXmlCommentsTransformer()]
            |> List.head
         Assert.Equal (None, commandDef.Description)
         Assert.Equal (Some memberDesc, commandDef.Members.Head.Description)

    [<Fact>]
    member _.``Attribute xform finds command description``() =
        let commandDef =
            ApplyTransformsToCommandDefs 
                @$"[Description(""{commandDesc}"")]
                 public static void A(string one) {{}}"
                [DescriptionsFromAttributesTransformer()]
            |> List.head
        Assert.Equal (Some commandDesc, commandDef.Description)
        Assert.Equal (None, commandDef.Members.Head.Description)

    [<Fact>]
    member _.``Attribute xform finds member description``() =
         let commandDef =
            ApplyTransformsToCommandDefs 
                @$"/// <summary>
                 /// 
                 /// </summary>
                 /// <param name=""one""></param>
                 public static void A([Description(""{memberDesc}"")] string one) {{}}"
                [DescriptionsFromAttributesTransformer()]
            |> List.head
         Assert.Equal (None, commandDef.Description)
         Assert.Equal (Some memberDesc, commandDef.Members.Head.Description)

    //[<Fact>]
    //member _.``Map xform finds command description``() =
    //    ApplyTransformsToCommandDefs
    //        MapData.OneSimpleMapping 
    //        [DescriptionsFromXmlCommentsTransforer()]

    //[<Fact>]
    //member _.``Map xform finds member description``() =
    //    ApplyTransformsToCommandDefs 
    //        MapData.OneSimpleMapping 
    //        [DescriptionsFromXmlCommentsTransforer()]

    [<Fact>]
    member _.``Multiple transforms work together``() =
         let commandDef =
            ApplyTransformsToCommandDefs 
                @$"/// <summary>
                 /// 
                 /// </summary>
                 /// <param name=""one"">{memberDesc}</param>
                 [Description(""{commandDesc}"")]
                 public static void A(string one) {{}}"
                [DescriptionsFromXmlCommentsTransformer()]
            |> List.head
         Assert.Equal (Some commandDesc, commandDef.Description)
         Assert.Equal (Some memberDesc, commandDef.Members.Head.Description)


    [<Fact>]
    member _.``Single transforms work on multiple fields``() =
        let commandDef =
            ApplyTransformsToCommandDefs 
                @$"/// <summary>
                 /// {commandDesc}
                 /// </summary>
                 /// <param name=""one"">{memberDesc}</param>
                 public static void A(string one) {{}}"
                [DescriptionsFromXmlCommentsTransformer()]
            |> List.head
        Assert.Equal (Some commandDesc, commandDef.Description)
        Assert.Equal (Some memberDesc, commandDef.Members.Head.Description)
