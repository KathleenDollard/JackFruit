module Utilities

open System

let StringJoin (separator: string) (list: string list) =
    String.Join(separator, list)