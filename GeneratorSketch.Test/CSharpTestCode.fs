module CSharpTestCode

    open FsUnit
    open Xunit

    let createMethod name (statements: string list) = 
        let code = String.concat "\r" statements 
        @$"
        public void {name}()
        {{
            {code}
        }}
        "

    let createClass name code = @$"
        public class {name}
        {{
            {code}
        }}"

    let createNamespace (usings:string list) name code = 
        let usings = String.concat " " usings
        @$"using System;
        namespace {name}
        {{
            {code}
        }}"

    let createFromStatement (usings: string list) code = 
        createMethod "MethodA" code
        |> createClass "ClassA"
        |> createNamespace ("System"::usings) "NamespaceA"


    let readCodeFromFile fileName =
        System.IO.File.ReadAllText fileName


