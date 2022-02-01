module AddCommandMappingTestUtils

open Microsoft.CodeAnalysis.CSharp

let appBaseCode =
    @"
namespace CliApp
{
    using System.Collections.Generic;
    using System;

    public class AppBase
    {

        public static List<string> DefaultPatterns = new() { ""*"", ""Run *"", ""* Handler"" };
        public static void AddCommandNamePattern(string pattern) { }
        public static void RemoveCommandNamePattern(string pattern) { }
        public void AddCommand(Delegate handler) { }
        public void AddSubCommand(Delegate handler) { }
    }

    public class MyCli : AppBase
    { 
        public AppBase Spock{ get; set; }
    }
}"

let SyntaxTreeWithStatements statements =
    let code = 
        @$"
namespace CliApp
{{
    public class A
    {{
        public void B()
        {{
            var app = new CliApp.MyCli();
            {statements}
        }}
    }}

    public class ClassB
    {{
        
        public static void Hndlr(int p1, string pStr) {{ }}
    }}
}}"
    CSharpSyntaxTree.ParseText(code)