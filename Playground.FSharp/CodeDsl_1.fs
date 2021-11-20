module CodeDsl_1

open System

type Scope =
    | Public
    | Internal
    | Private


type Attribute<'T> =
    { Name: string // If this comes up, more work needed
    }


    
[<AbstractClass>]
type CodeElement(atrributes: Attribute list) =
    abstract member Output: string
    


[<AbstractClass>]
type Statement() =
    abstract member Output: string

[<AbstractClass>]
type Member(name: string, scope: Scope) =
    abstract member Output: string
    member _.Name = name
    static member OutputList members =
        // KAD-Don: This concatenates the result of Output. Easier way?
        String.concat "/n/n" (((List.map (fun (x: Member) -> x.Output) members) )|> Seq.ofList)

type Parameter =
    { Name: string
      Type: string
    }

type Method(name: string, scope: Scope, parameters: Parameter list, statements: Statement list) =
    inherit Member(name, scope)
        override this.Output = ""

    static member publicMethod (name: string) (parameters: Parameter list) statements =
        // KAD-Don: Why does this work?
        Method (name, Public, parameters, statements)

type Class =
    { Name: string
      Scope : Scope
      IsStatic: bool
      Members: Member list
    }

    static member publicClass name members  =
        { Class.Name = name
          Scope = Public 
          IsStatic = false 
          Members = members }

    member this.Output = 
        $@"
        {this.Scope} class {this.Name}
        {{
            {Member.OutputList this.Members}
        }}"


let clsName = "MyClass"
let method = "MyMethod"
let demo =
    Class.publicClass clsName [
        Method.publicMethod clsName [] []
        ]