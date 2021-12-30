namespace Generator.Tests

open Generator.Models
open Common

// TODO: Remove MapInferredStatements from this file
type MapData =
    { HandlerCode: string list
      CommandDef: CommandDef list
      OutputCode: string list }

    static member NoMapping =
        { HandlerCode = []
          CommandDef = []
          OutputCode = [ "" ] }

    static member OneSimpleMapping =
        let commandDef = CommandDef("A", [], Void, Arbitrary "MyCommand")
        let members = [ MemberDef("one", commandDef, (SimpleNamedItem "string"), ArbitraryMember, true) ]
        commandDef.Members <- members

        { HandlerCode = [ "public static void A(string one) {}" ]
          CommandDef = [ commandDef ]
          OutputCode = [ "" ] }

    static member OneComplexMapping =
        let commandDef = CommandDef("BLongName", [], Void, Arbitrary "MyCommand")
        let members = 
            [ MemberDef("packageName", commandDef,(SimpleNamedItem "string"), ArbitraryMember, true)
              MemberDef("two", commandDef,(SimpleNamedItem "int"), ArbitraryMember, true)
              MemberDef("three", commandDef,(SimpleNamedItem "string"), ArbitraryMember, true) ] 
        commandDef.Members <- members
  
        { HandlerCode = [ "public static void BLongName(string packageName, int two, string three) {}" ]
          CommandDef = [commandDef]
          OutputCode = [""]}

    static member ThreeMappings =
        let makeCommandDef id =
            CommandDef(id, [], Void, Arbitrary "MyCommand")

        { HandlerCode = 
            [ "public static void A() { }"
              "public static void B() { }"
              "public static void C() { }" ]
          CommandDef =
            [ makeCommandDef "A"
              makeCommandDef "B"
              makeCommandDef "C" ]
          OutputCode = [""]}
