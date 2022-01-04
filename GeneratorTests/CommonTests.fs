module ``When creating a generic types``

open Xunit
open FsUnit.Xunit
open FsUnit.CustomMatchers
open Generator.RoslynUtils
open Microsoft.CodeAnalysis
open Generator.Language
open DslCodeBuilder
open System.Linq
open Common

let NameFromSimpleName (namedItem: NamedItem) =
    match namedItem with
    | SimpleNamedItem n -> n
    | _ -> invalidOp "Simple name not found"
  
let NameAndGenericsFromName (namedItem: NamedItem) =
    match namedItem with
    | GenericNamedItem (n, g) -> 
        n, 
        [for x in g do 
            match x with 
            | SimpleNamedItem gn -> gn
            | _ -> invalidOp "Generic was not a simple name" ]
    | _ -> invalidOp "Generic name not found"


/// The generic parsing uses a generic parser so validation is not done at this point. Instead 
/// it's done as code is output or during a validation pass, neither is written yet. 
type ``When parsing generic strings``() =
    let ParseGenericString input = TreeFromDelimitedString '<' '>' ',' input

    [<Fact>]
    member _.``simple type is parsed``() =
        let input = "int"
        let actual = ParseGenericString input
        let expected = { Data = input; Children = []}

        Assert.Equal(expected, actual)


    [<Fact>]
    member _.``single generic type is parsed``() =
        let input = "List<int>"
        let actual = ParseGenericString input
        let expected = 
            { Data = "List"
              Children = 
                [ { Data = "int"; Children = [] } ] }

        Assert.Equal(expected, actual)


    [<Fact>]
    member _.``complex generic type is parsed``() =
        let input = "className <string, List<int>, int, float, MyType<List<bool>>>"
        let actual = ParseGenericString input
        let expected = 
            { Data = "className"
              Children = 
                [ { Data = "string"; Children = [] } 
                  { Data = "List"
                    Children = 
                        [ { Data = "int"; Children = [] } ] }
                  { Data = "int"; Children = [] } 
                  { Data = "float"; Children = [] } 
                  { Data = "MyType"
                    Children = 
                        [ { Data = "List"
                            Children = 
                                [ { Data = "bool"; Children = [] } ] }
                        ] }
                 ] }

        Assert.Equal(expected, actual)


    [<Fact>]
    member _.``missing close bracket does not throw``() =
        let input = "className <string, List<int>, int, float, MyType<List<bool>>"
        let actual = ParseGenericString input
        let expected = 
            { Data = "className"
              Children = 
                [ { Data = "string"; Children = [] } 
                  { Data = "List"
                    Children = 
                        [ { Data = "int"; Children = [] } ] }
                  { Data = "int"; Children = [] } 
                  { Data = "float"; Children = [] } 
                  { Data = "MyType"
                    Children = 
                        [ { Data = "List"
                            Children = 
                                [ { Data = "bool"; Children = [] } ] }
                        ] }
                 ] }

        Assert.Equal(expected, actual) 
        
        
    [<Fact>]
    member _.``no close brackets gives same result (and doesn't throw)``() =
        let input = "className <string, List<int>, int, float, MyType<List<bool"
        let actual = ParseGenericString input
        let expected = 
            { Data = "className"
              Children = 
                [ { Data = "string"; Children = [] } 
                  { Data = "List"
                    Children = 
                        [ { Data = "int"; Children = [] } ] }
                  { Data = "int"; Children = [] } 
                  { Data = "float"; Children = [] } 
                  { Data = "MyType"
                    Children = 
                        [ { Data = "List"
                            Children = 
                                [ { Data = "bool"; Children = [] } ] }
                        ] }
                 ] }

        Assert.Equal(expected, actual)        
 
 
    [<Fact>]
    member _.``extra close bracket gives same result (and doesn't throw)``() =
        let input = "className <string, List<int>, int, float, MyType<List<bool>>>>"
        let actual = ParseGenericString input
        let expected = 
            { Data = "className"
              Children = 
                [ { Data = "string"; Children = [] } 
                  { Data = "List"
                    Children = 
                        [ { Data = "int"; Children = [] } ] }
                  { Data = "int"; Children = [] } 
                  { Data = "float"; Children = [] } 
                  { Data = "MyType"
                    Children = 
                        [ { Data = "List"
                            Children = 
                                [ { Data = "bool"; Children = [] } ] }
                        ] }
                 ] }

        Assert.Equal(expected, actual)

        
    [<Fact>]
    member _.``early close bracket ignores rest (and  does not throw)``() =
        let input = "className <string, List<int>>, int, float>"
        let actual = ParseGenericString input
        let expected = 
            { Data = "className"
              Children = 
                [ { Data = "string"; Children = [] } 
                  { Data = "List"
                    Children = 
                        [ { Data = "int"; Children = [] } ] }
                 ] }

        Assert.Equal(expected, actual)


    [<Fact>]
    member _.``double brackets gives node with empty string (and  does not throw)``() =
        let input = "className <string, List<<int>, int, float>>"
        let actual = ParseGenericString input
        let expected = 
            { Data = "className"
              Children = 
                [ { Data = "string"; Children = [] } 
                  { Data = "List"
                    Children = 
                        [ { Data = ""
                            Children = 
                                [ { Data = "int"; Children = [] } ] }
                                   
                          { Data = "int"; Children = [] } 
                          { Data = "float"; Children = [] } 
                               
                        ] }
                 ] }

        Assert.Equal(expected, actual)


    [<Fact>]
    member _.``missing open bracket ignores rest (does not throw)``() =
        let input = "className <string, List int>, int, float, MyType<List<bool>>>"
        let actual = ParseGenericString input
        let expected = 
            { Data = "className"
              Children = 
                [ { Data = "string"; Children = [] } 
                  { Data = "List int"; Children = [] } ] }

        Assert.Equal(expected, actual)

    
    [<Fact>]
    member _.``missing comma does not throw, uses string as is (doesn't throw, this is a general parser, validate later)``() =
        let input = "className <string, List<int>, int float, MyType<List<bool>>>"
        let actual = ParseGenericString input
        let expected = 
            { Data = "className"
              Children = 
                [ { Data = "string"; Children = [] } 
                  { Data = "List"
                    Children = 
                        [ { Data = "int"; Children = [] } ] }
                  { Data = "int float"; Children = [] } 
                  { Data = "MyType"
                    Children = 
                        [ { Data = "List"
                            Children = 
                                [ { Data = "bool"; Children = [] } ] }
                        ] }
                 ] }

        Assert.Equal(expected, actual)

 
    [<Fact>]
    member _.``double comma does not throw gives empty name (and does not throw)``() =
        let input = "className <string, List<int>, , int, float, MyType<List<bool>>>"
        let actual = ParseGenericString input
        let expected = 
            { Data = "className"
              Children = 
                [ { Data = "string"; Children = [] } 
                  { Data = "List"
                    Children = 
                        [ { Data = "int"; Children = [] } ] }
                  { Data = ""; Children = [] } 
                  { Data = "int"; Children = [] } 
                  { Data = "float"; Children = [] } 
                  { Data = "MyType"
                    Children = 
                        [ { Data = "List"
                            Children = 
                                [ { Data = "bool"; Children = [] } ] }
                        ] }
                 ] }

        Assert.Equal(expected, actual)
        
        
        // Save the following as a Playground about why I used strings for generics. 
    //[<Fact(Skip = "Not yet implemented")>]
    //member _.``with generics function``() =
    //    let typeName = "A"
    //    let genericTypeName = "B"
    //    let genericTypeName1 = "C"
    //    let genericTypeName2 = "D"
    //    let genericTypeName3 = "E"

    //    let actual = Generic typeName Of genericTypeName
    //    let actual = Generic typeName Of [genericTypeName1, genericTypeName2]
    //    let actual = Generic typeName Of (Generic genericTypeName1 Of genericTypeName2)
    //    let actual = Generic typeName Of [(Generic genericTypeName1 Of genericTypeName2), genericTypeName3]

    //    Assert.IsType<Generic>(actual)
    //    Assert.Equal(1, actual.GenericTypes.Count())
    //    Assert.Equal(typeName, actual.TypeName)     
    //    Assert.Equal(genericTypeName, actual.GenericTypes.First().TypeName)   
