// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System



let fullNameFor firstName middleName lastName =
    let middleName = 
        if String.IsNullOrWhiteSpace middleName
        then ""
        else " " + middleName

    firstName + middleName + lastName

let x = 42

let fullNames =
    let firstLastName = fullNameFor "John" ""
    $"""Hello to our guests {firstLastName "Smith"} and {firstLastName "Green"}"""

type rgb = {red:int; green:int; blue: int}

let rgbRemoveRed (red, green, blue) = (0, green, blue)
let rgbRemoveGreen color = { red = color.red; blue = color.blue;  green = 0 }
let rgbRemoveGreen2 color = { color with  green = 0 }

let rgbList = ["red"; "green"; "blue"]
let green = rgbList.[1]

type Shape =
    | Circle of radius : float
    | Rectangle of width : float * length : float
    | Prism of width : float * length: float * height : float

let matchShape shape =
    match shape with 
    | Circle r -> $"Radius: {r}"
    | Rectangle (width = w) -> $"Width: {w}, length ignored"
    | Prism (height = 0.0) -> "Flat prism, width and length ignored"
    | Prism (width = w; height = h; length = l) -> $"Prism: {w}, {l}, {h}" 

type Shape2 =
    | Circle of float
    | Rectangle of float * float
    | Prism of width : float * float * height : float

type Option<'a> = 
   | Some of 'a 
   | None   
   

let sillyFind id found=
    if found
    then Some id
    else None

let helloFromAnimals () = 
    let list = ["meow"; "woof"; "rattle"]
    for value in list do
        printfn "%s" value
    

let sumSquaresToTen () =
    let mutable current = 0
    for value in 1..10 do
        current <- (current * current) + value
    current

let sumSquares top =
    let mutable current = 0
    for value in 1..top do
        current <- (current * current) + value
    current


let sumEven () =
    let mutable current = 0
    for value in 1..10 do
        if value % 2 = 0
        then current <- current + value
    current

type Animal = {
    name: string
    sound: String }


//type Animal2 = {
//    name: string
//    sound: String }

let makeSounds animal =
    match animal with
    | { Animal.name = "cat"} when DateTime.Now.Hour > 8 -> "Yawn, I was sleeping"
    | { sound = null } -> "<no sound>"
    | { sound = s } -> s
    | _ -> "Huh"

let constPattern i =
    match i with 
    | 0 -> "zero"
    | 1|2|3 -> "1, 2, 3"
    | _ when i < 0 -> "less than zero"
    | _ -> "greater than 3"

let listPattern list =
    match list with 
    | [] -> "empty list"
    | [ 42; _ ] -> "list with two items where the first is 42"
    | [ _; var1 ] -> $"list with any two items. The second items is {var1}"
    | _ -> "list with more than two items"

let saturedColor color =
    match color with 
    | (255, 255, 255) -> printfn $"Black"
    | (255, _, _) -> printfn $"Saturated red"
    | (_, 255, _) -> printfn $"Saturated green"
    | (_, _, 255) -> printfn $"Saturated blue"
    | _ -> printfn $"Not saturated"

type Name = {
    first: string
    middle: string Option
    last: string }

let fullName name = 
    match name with
    | { first = f; middle = Some m; last = l} -> $"{f} {m} {l}"
    | { first = f; last = l} -> $"{f} {l}"


type Result<'T,'TError> =
    | Ok of ResultValue:'T
    | Error of ErrorValue:'TError

// Similar to docs
type Person =
    { name: string
      email: string }

let validateName person =
    match person.name with
    | null -> Error "No name found."
    | "" -> Error "Name is empty."
    | "bananas" -> Error "Bananas is not a name."
    | _ -> Ok person

let test() = 
    let person = { name = "John Jones"; email = "john.jones@contoso.biz" }
    let result = validateName person
    match result with
    | Ok p -> printfn $"{p} is OK!"
    | Error e ->  printfn $"Error: {e}"

// Define a function to construct a message to print
let from whom =
    sprintf "from %s" whom

let listEx = [1..10]
let arrayEx = [|"red"; "yellow"; "blue"|]
let colors = [ for c in arrayEx do c ] // or Array.toList arrayEx
let printColors = for c in colors do printfn $"{c}"

let s1 = "1. John Smith"          // simple string
let s2 = "2. John\nSmith"         // simple string, Smith on new line
let s2a = "2a. John
Smith"                            // simple string, Smith on new line
let s3 = @"3. John\nSmith"        // verbatim string, \n is output
let firstName = "John"
let s4 = $"4. {firstName} Smith"  // interploated string

let t = (42, "yellow", true)
type cust = {id: int; faveColor: string; active: bool}
let c = {id = 42; faveColor = "yellow"; active = true}

           
    
[<EntryPoint>]
let main argv =

    FSharp6.Test

    let a = true
    let x = 42    // inferred as integer 
    let y = 42.   // inferred as float/double
    let z = 42m   // inferred as decimal

    let t = (42, "yellow", true)

    let message = from "F#" // Call the function
    printfn "Hello world %s" message
    let x = fullNameFor "John" "" "Smith"
    let rgbPink = (231, 84, 128)
    let withoutRed = rgbRemoveRed rgbPink
    let unknownColor = rgbRemoveRed (231, 84, 128)
    let rgbPurple = {red = 160; green = 32; blue = 240}
    let fish = {name = "fish"; sound = null}
    let cat = {name = "cat"; sound = "meow"}
    printfn $"{(makeSounds fish)}"
    printfn $"{(makeSounds cat)}"
    0 // return an integer exit code


 
