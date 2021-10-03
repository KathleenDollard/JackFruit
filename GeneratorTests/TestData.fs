module Generator.Tests.TestData

open Generator.Models

type testData =
    { MapInferredStatements: string list
      CommandNames: string list
      CommandDef: CommandDef }

let badMapping = """MapInferredX("", Handlers.X);"""

let oneMapping = ["""builder.MapInferred("", Handlers.A);"""]
let oneMappingCommandNames = [ "" ]

let noMapping = [ ]
let noMappingCommandNames = []

let threeMappings = ["""builder.MapInferred("dotnet", Handlers.A);"""
                     """builder.MapInferred("dotnet add <PROJECT>", null);"""
                     """builder.MapInferred("dotnet add package <PACKAGE_NAME>", Handlers.B);"""]
let threeMappingsCommandNames = [ "dotnet"; "add"; "package" ]


