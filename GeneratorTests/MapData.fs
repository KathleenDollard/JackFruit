// KAD-Don: What is the difference between a type being in a namespace and in a module?

namespace Generator.Tests

open Generator.Models

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
        let members = [ MemberDef("one", "string", ArbitraryMember, true) ]
        let commandDef = CommandDef("A", [], None, Arbitrary, members, [])

        { HandlerCode = [ "public static void A(string one) {}" ]
          CommandDef = [ commandDef ]
          OutputCode = [ "" ] }

    static member OneComplexMapping =
        let members = 
            [ MemberDef("packageName", "string", ArbitraryMember, true)
              MemberDef("two", "int", ArbitraryMember, true)
              MemberDef("three", "string", ArbitraryMember, true) ] 
        let commandDef = CommandDef("BLongName", [], None, Arbitrary, members, [])
  
        { HandlerCode = [ "public static void BLongName(string packageName, int two, string three) {}" ]
          CommandDef = [commandDef]
          OutputCode = [""]}

    static member ThreeMappings =
        let makeCommandDef id =
            CommandDef(id, [], None, Arbitrary, [], [])

        { HandlerCode = 
            [ "public static void A() { }"
              "public static void B() { }"
              "public static void C() { }" ]
          CommandDef =
            [ makeCommandDef "A"
              makeCommandDef "B"
              makeCommandDef "C" ]
          OutputCode = [""]}
