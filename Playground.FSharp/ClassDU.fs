module ClassDU

open System

// This was an attempt to play with paramarrays and optional parameters. Both require tuples, commas, blech
// In the current layout, the scopes (Public, etc) shadow each other, no joy. Without shadowing
//    this requires `Scope Public` potentially with parens.
//    I tried a static member on a DU, and that was not sufficient to switch the resolution scope to method
            // Where ClassPartsDU renamed to Class, etc
            //static member Make (parts: Class list) = ""


            //let c = Class.Make [
            //Public
            //Static
            //]

            //let m = Method.Make [
            //Public  // Resolves as Class
            //Static ]
// If a list is used for the parts, 

// Desire something like: 

//Class { Public Static InheritedFrom [Interfaces]
//    Field Scope Name Type
//    Field Scope Static Name Type
//    Ctor {
//        Scope BaseStuff
//        Statements [..]}
//    Method {
//        Scope Override Static Name ReturnType
//        Statements [..]}
//    Method {
//        Scope Override Static Name ReturnType
//        Statements [..]}
//    Property {
//        Scope Override Static Name Type
//        GetStatements [..]
//        SetStatements [..]}
//}

type FieldPartsDU =
    | Public
    | Private
    | Internal
    | Protected
    | PrivateProtected
    | InternalProtected
    | Static
    //| Statements of statements: Statement list

type CtorPartsDU =
    | Public
    | Private
    | Internal
    | Protected
    | PrivateProtected
    | InternalProtected
    | Static
    | Name of name: string 
    | TypeName of typeName: string // later make this NamedItem
    //| Statements of statements: Statement list

type PropertyPartsDU =
    | Public
    | Private
    | Internal
    | Protected
    | PrivateProtected
    | InternalProtected
    | Name of name: string 
    | TypeName of typeName: string // later make this NamedItem
    //| GetStatements of statements: Statement list
    //| SetStatements of statements: Statement list


type MethodPartsDU =
    | Public
    | Private
    | Internal
    | Protected
    | PrivateProtected
    | InternalProtected
    | Static
    | Name of name: string 
    | ReturnType of typeName: string // later make this NamedItem
    //| Statements of statements: Statement list

    
type ClassPartsDU =
    | Public
    | Private
    | Internal
    | Protected
    | PrivateProtected
    | InternalProtected
    | Static
    | Field of field: FieldPartsDU
    | Ctor of ctor: CtorPartsDU
    | Property of property: PropertyPartsDU
    | Method of method: MethodPartsDU


