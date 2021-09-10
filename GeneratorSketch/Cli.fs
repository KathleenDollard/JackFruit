module Cli

open System.CommandLine

let  option<'T> (name:string) = 
   let typeName = nameof<'T>
   $"""new Option<{typeName}>("{name}")"""
