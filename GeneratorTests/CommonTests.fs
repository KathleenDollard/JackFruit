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
open type Common.Generic

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

type ``I can create a type name ``() =

    [<Fact>]
    member _.``without generics explicitly``() =
        let typeName = "George"

        let actual = Generic typeName

        Assert.IsType<Generic>(actual)
        Assert.Equal(0, actual.GenericTypes.Count())
        Assert.Equal(typeName, actual.TypeName)
        
    [<Fact>]
    member _.``without generics implicitly``() =
        let typeName = "George"

        let actual: Generic = typeName

        Assert.IsType<Generic>(actual)
        Assert.Equal(0, actual.GenericTypes.Count())
        Assert.Equal(typeName, actual.TypeName)
    
    [<Fact>]
    member _.``with generics explicitly``() =
        let typeName = "A"
        let generictypeName = "B"

        let actual = Generic(typeName, generictypeName)

        Assert.IsType<Generic>(actual)
        Assert.Equal(1, actual.GenericTypes.Count())
        Assert.Equal(typeName, actual.TypeName)
  
    [<Fact>]
    member _.``without generics using Of``() =
        let typeName = "A"

        let actual = Of typeName

        Assert.IsType<Generic>(actual)
        Assert.Equal(0, actual.GenericTypes.Count())
        Assert.Equal(typeName, actual.TypeName)     
  
    [<Fact>]
    member _.``with generics using Of``() =
        let typeName = "A"
        let genericTypeName = "B"

        let actual = Of (typeName, genericTypeName)

        Assert.IsType<Generic>(actual)
        Assert.Equal(1, actual.GenericTypes.Count())
        Assert.Equal(typeName, actual.TypeName)     
        Assert.Equal(genericTypeName, actual.GenericTypes.First().TypeName)     

    [<Fact(Skip = "Not yet implemented")>]
    member _.``with generics infix``() =
        let typeName = "A"
        let genericTypeName = "B"

        //!, %, &, *, +, -, ., /, <, =, >, ?, @, ^, |,
        // Remove misleading ideas: %, &, *, +, -, ., /, <, =, >, ^, |,
        //let actual = typeName &<= [genericTypeName]

        //Assert.IsType<Generic>(actual)
        //Assert.Equal(1, actual.GenericTypes.Count())
        //Assert.Equal(typeName, actual.TypeName)     
        //Assert.Equal(genericTypeName, actual.GenericTypes.First().TypeName)  
        ()
type ``When parsing delimited strings``() =
    let ParseDelimtedString input = TreeFromDelimitedString '(' ')' ';' input

    [<Fact>]
    member _.``simple string is parsed``() =
        let input = "int"
        let actual = ParseDelimtedString input
        let expected = "int"

        Assert.IsType<TreeNodeType<string>>(actual)
        Assert.Equal(expected, actual.Data)

    [<Fact>]
    member _.``single string is parsed``() =
        let input = "List(int)"
        let actual = ParseDelimtedString input
        let expected1 = "List"
        let expected2 = "int"

        Assert.IsType<TreeNodeType<string>>(actual)
        Assert.Equal(expected1, actual.Data)
        Assert.Equal(1, actual.Children.Length)
        Assert.Equal(expected2, actual.Children[0].Data)

    [<Fact>]
    member _.``complex string is parsed``() =
        let input = "className (string; List(int); int; float; MyType(List(bool)))"
        let actual = ParseDelimtedString input
        let expected1 = "className"
        let expected2 = "MyType"

        Assert.IsType<TreeNodeType<string>>(actual)
        Assert.Equal(expected1, actual.Data)
        Assert.Equal(5, actual.Children.Length)
        Assert.Equal(1, actual.Children[4].Children.Length)
        Assert.Equal(expected2, actual.Children[4].Data)    

type ``When parsing generic strings``() =
    let ParseGenericString input = TreeFromDelimitedString '<' '>' ',' input

    [<Fact>]
    member _.``simple type is parsed``() =
        let input = "int"
        let actual = ParseGenericString input
        let expected = "int"

        Assert.IsType<TreeNodeType<string>>(actual)
        Assert.Equal(expected, actual.Data)

    [<Fact>]
    member _.``single generic type is parsed``() =
        let input = "List<int>"
        let actual = ParseGenericString input
        let expected1 = "List"
        let expected2 = "int"

        Assert.IsType<TreeNodeType<string>>(actual)
        Assert.Equal(expected1, actual.Data)
        Assert.Equal(1, actual.Children.Length)
        Assert.Equal(expected2, actual.Children[0].Data)

    [<Fact>]
    member _.``complex generic type is parsed``() =
        let input = "className <string, List<int>, int, float, MyType<List<bool>>>"
        let actual = ParseGenericString input
        let expected1 = "className"
        let expected2 = "MyType"

        Assert.IsType<TreeNodeType<string>>(actual)
        Assert.Equal(expected1, actual.Data)
        Assert.Equal(5, actual.Children.Length)
        Assert.Equal(1, actual.Children[4].Children.Length)
        Assert.Equal(expected2, actual.Children[4].Data)    

    [<Fact(Skip="Causing infinite loop")>]
    member _.``missing close bracket does not throw``() =
        let input = "className <string, List<int>, int, float, MyType<List<bool>>"
        let actual = ParseGenericString input
        let expected1 = "className"
        let expected2 = "MyType"

        Assert.IsType<TreeNodeType<string>>(actual)
        Assert.Equal(expected1, actual.Data)
        Assert.Equal(5, actual.Children.Length)
        Assert.Equal(1, actual.Children[4].Children.Length)
        Assert.Equal(expected2, actual.Children[4].Data)    
        
        
    [<Fact(Skip="Causing infinite loop")>]
    member _.``no close brackets does not throw``() =
        let input = "className <string, List<int>, int, float, MyType<List<bool"
        let actual = ParseGenericString input
        let expected1 = "className"
        let expected2 = "MyType"

        Assert.IsType<TreeNodeType<string>>(actual)
        Assert.Equal(expected1, actual.Data)
        Assert.Equal(5, actual.Children.Length)
        Assert.Equal(1, actual.Children[4].Children.Length)
        Assert.Equal(expected2, actual.Children[4].Data)    
        
        
    [<Fact>]
    member _.``extra close bracket does not throw``() =
        let input = "className <string, List<int>, int, float, MyType<List<bool>>>>"
        let actual = ParseGenericString input
        let expected1 = "className"
        let expected2 = "MyType"

        Assert.IsType<TreeNodeType<string>>(actual)
        Assert.Equal(expected1, actual.Data)
        Assert.Equal(5, actual.Children.Length)
        Assert.Equal(1, actual.Children[4].Children.Length)
        Assert.Equal(expected2, actual.Children[4].Data)   
        
    [<Fact>]
    member _.``early close bracket does not throw``() =
        let input = "className <string, List<int>>, int, float>"
        let actual = ParseGenericString input
        let expected1 = "className"
        let expected2 = "List"

        Assert.IsType<TreeNodeType<string>>(actual)
        Assert.Equal(expected1, actual.Data)
        Assert.Equal(2, actual.Children.Length)
        Assert.Equal(1, actual.Children[1].Children.Length)
        Assert.Equal(expected2, actual.Children[1].Data)    


    [<Fact(Skip="Causing infinite loop")>]
    member _.``double brackets does not throw``() =
        let input = "className <string, List<<int>, int, float, MyType<List<bool>>>"
        let actual = ParseGenericString input
        let expected1 = "className"
        let expected2 = ""
        let expected3 = "float"

        Assert.IsType<TreeNodeType<string>>(actual)
        Assert.Equal(expected1, actual.Data) 
        Assert.Equal(6, actual.Children.Length)
        Assert.Equal(0, actual.Children[4].Children.Length)
        Assert.Equal(expected2, actual.Children[2].Data)   
        Assert.Equal(expected2, actual.Children[5].Data)   
      
    [<Fact(Skip="Throws out of bounds")>]
    member _.``missing open bracket does not throw``() =
        let input = "className <string, List int>, int, float, MyType<List<bool>>>"
        let actual = ParseGenericString input
        let expected1 = "className"
        let expected2 = "List int"

        Assert.IsType<TreeNodeType<string>>(actual)
        Assert.Equal(expected1, actual.Data)
        Assert.Equal(2, actual.Children.Length)
        Assert.Equal(1, actual.Children[4].Children.Length)
        Assert.Equal(expected2, actual.Children[1].Data)    

    
    [<Fact>]
    member _.``missing comma does not throw``() =
        let input = "className <string, List<int>, int float, MyType<List<bool>>>"
        let actual = ParseGenericString input
        let expected1 = "className"
        let expected2 = "int float"

        Assert.IsType<TreeNodeType<string>>(actual)
        Assert.Equal(expected1, actual.Data)
        Assert.Equal(4, actual.Children.Length)
        Assert.Equal(1, actual.Children[3].Children.Length)
        Assert.Equal(expected2, actual.Children[2].Data)    

 
    [<Fact>]
    member _.``double comma does not throw``() =
        let input = "className <string, List<int>, , int, float, MyType<List<bool>>>"
        let actual = ParseGenericString input
        let expected1 = "className"
        let expected2 = "MyType"
        let expected3 = ""

        Assert.IsType<TreeNodeType<string>>(actual)
        Assert.Equal(expected1, actual.Data)
        Assert.Equal(6, actual.Children.Length)
        Assert.Equal(1, actual.Children[5].Children.Length)
        Assert.Equal(expected2, actual.Children[5].Data) 
        Assert.Equal(expected3, actual.Children[2].Data)
        
        
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
