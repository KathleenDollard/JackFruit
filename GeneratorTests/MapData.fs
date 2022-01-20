module Generator.Tests.MapData

open Generator.Models
open Common
open Generator.LanguageModel

type Data = 
    { HandlerCode: string list
      CommandDef: CommandDef list
      OutputCode: string list }

let NoMapping =
    { HandlerCode = []
      CommandDef = []
      OutputCode = [ "" ] }

let OneSimpleMapping =
    let commandDef = CommandDef("A", [], ReturnType.ReturnTypeVoid, Arbitrary "MyCommand")
    let members = [ MemberDef("one", commandDef, (SimpleNamedItem "string"), ArbitraryMember, true) ]
    commandDef.Members <- members

    { HandlerCode = [ "public static void A(string one) {}" ]
      CommandDef = [ commandDef ]
      OutputCode = [ "" ] }

let OneComplexMapping =
    let commandDef = CommandDef("BLongName", [], ReturnType.ReturnTypeVoid, Arbitrary "MyCommand")
    let members = 
        [ MemberDef("packageName", commandDef,(SimpleNamedItem "string"), ArbitraryMember, true)
          MemberDef("two", commandDef,(SimpleNamedItem "int"), ArbitraryMember, true)
          MemberDef("three", commandDef,(SimpleNamedItem "string"), ArbitraryMember, true) ] 
    commandDef.Members <- members
  
    { HandlerCode = [ "public static void BLongName(string packageName, int two, string three) {}" ]
      CommandDef = [commandDef]
      OutputCode = [""]}

let ThreeMappings =
    let makeCommandDef id =
        CommandDef(id, [], ReturnType.ReturnTypeVoid, Arbitrary "MyCommand")

    { HandlerCode = 
        [ "public static void A() { }"
          "public static void B() { }"
          "public static void C() { }" ]
      CommandDef =
        [ makeCommandDef "A"
          makeCommandDef "B"
          makeCommandDef "C" ]
      OutputCode = [""]}
