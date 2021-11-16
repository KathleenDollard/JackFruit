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
        let commandDef = CommandDef("A", [], Some "void", Arbitrary)
        let members = [ MemberDef("one", commandDef, "string", ArbitraryMember, true) ]
        commandDef.Members <- members

        { HandlerCode = [ "public static void A(string one) {}" ]
          CommandDef = [ commandDef ]
          OutputCode = [ "" ] }

    static member OneComplexMapping =
        let commandDef = CommandDef("BLongName", [], Some "void", Arbitrary)
        let members = 
            [ MemberDef("packageName", commandDef,"string", ArbitraryMember, true)
              MemberDef("two", commandDef,"int", ArbitraryMember, true)
              MemberDef("three", commandDef,"string", ArbitraryMember, true) ] 
        commandDef.Members <- members
  
        { HandlerCode = [ "public static void BLongName(string packageName, int two, string three) {}" ]
          CommandDef = [commandDef]
          OutputCode = [""]}

    static member ThreeMappings =
        let makeCommandDef id =
            CommandDef(id, [], Some "void", Arbitrary)

        { HandlerCode = 
            [ "public static void A() { }"
              "public static void B() { }"
              "public static void C() { }" ]
          CommandDef =
            [ makeCommandDef "A"
              makeCommandDef "B"
              makeCommandDef "C" ]
          OutputCode = [""]}
