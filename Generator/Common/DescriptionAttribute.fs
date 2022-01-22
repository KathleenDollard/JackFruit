namespace Generator.Common 

open System

[< AttributeUsage(AttributeTargets.All) >]
type DescriptionAttribute(description: string) =
    inherit Attribute()

    member _.Description: string = description

