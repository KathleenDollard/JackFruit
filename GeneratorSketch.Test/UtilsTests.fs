module UtilsTests

open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open GeneratorSketch.Utils

type ``When removing leading and trailing characters``() =

    [<Fact>]
    member _.``Empty string returns empty string``() =
        let source = ""

        let actual = RemoveLeadingTrailing '<' '>' source

        actual |> should equal ""

    [<Fact>]
    member _.``Just delimiters string returns empty string``() =
        let source = "<>"

        let actual = RemoveLeadingTrailing '<' '>' source

        actual |> should equal ""

    [<Fact>]
    member _.``Delimited text returns text``() =
        let source = "<Baby Yoda>"

        let actual = RemoveLeadingTrailing '<' '>' source

        actual |> should equal "Baby Yoda"

    
    [<Fact>]
    member _.``Not delimited text returns text``() =
        let source = "Baby Yoda"

        let actual = RemoveLeadingTrailing '<' '>' source

        actual |> should equal "Baby Yoda"
    
    [<Fact>]
    member _.``Partially delimited at start returns text with partial delimiter``() =
        let source = "<Baby Yoda"

        let actual = RemoveLeadingTrailing '<' '>' source

        actual |> should equal "<Baby Yoda"

    [<Fact>]
    member _.``Partially delimited at end returns text with partial delimiter``() =
        let source = "Baby Yoda>"

        let actual = RemoveLeadingTrailing '<' '>' source

        actual |> should equal "Baby Yoda>"

    [<Fact>]
    member _.``Double quoted text returns text``() =
        let source = "\"Baby Yoda\""

        let actual = RemoveLeadingTrailingDoubleQuote source

        actual |> should equal "Baby Yoda"


