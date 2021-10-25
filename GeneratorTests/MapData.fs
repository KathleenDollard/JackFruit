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
        let commandDef = CommandDef.Create "A"

        let commandDef =
            { commandDef with
                Members = [ MemberDef.Create "one" "string" ]
                Aliases = [ "A" ] }

        { HandlerCode = [ "public static void A(string one) {}" ]
          CommandDef = [ commandDef ]
          OutputCode = [ "" ] }

    static member OneComplexMapping =
        let commandDef = CommandDef.Create "BLongName"
        let commandDef = 
            {commandDef with
                Members = 
                    [ MemberDef.Create "packageName" "string"
                      MemberDef.Create "two" "int"
                      MemberDef.Create "three" "string" ] 
                Aliases = ["BLongName"]}
        { HandlerCode = [ "public static void BLongName(string packageName, int two, string three) {}" ]
          CommandDef = [commandDef]
          OutputCode = [""]}

    static member ThreeMappings =
        let makeCommandDef id =
            let commandDef = CommandDef.Create id
            { commandDef with Aliases = [ id ]}
        { HandlerCode = 
            [ "public static void A() { }"
              "public static void B() { }"
              "public static void C() { }" ]
          CommandDef =
            [ makeCommandDef "A"
              makeCommandDef "B"
              makeCommandDef "C" ]
          OutputCode = [""]}
