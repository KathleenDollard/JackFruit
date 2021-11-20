module Generator.LanguageExpression

open Generator.Language


module Public2 = 
    type Static() =
        static member A() = ""



let PublicStaticMethodOf 
    (returnType: Return)
    (name: string) 
    (generics: NamedItem list)
    (parameters: Parameter list)
    (statements: Statement list) =
    Member.Method
        { MethodName = NamedItem.Create name generics
          Scope = Public
          StaticOrInstance = Static 
          IsExtension = false
          ReturnType = returnType
          Parameters = parameters
          Statements = statements
        }

let PublicStaticMethod
    (returnType: Return)
    (name: string) 
    (parameters: Parameter list)
    (statements: Statement list) =
    Member.Method
        { MethodName = SimpleNamedItem name
          Scope = Public
          StaticOrInstance = Static 
          IsExtension = false
          ReturnType = returnType
          Parameters = parameters
          Statements = statements
        }

let PublicMethodOf 
    (returnType: Return)
    (name: string) 
    (generics: NamedItem list)
    (parameters: Parameter list)
    (statements: Statement list) =
    Member.Method
        { MethodName = NamedItem.Create name generics
          Scope = Public
          StaticOrInstance = Instance 
          IsExtension = false
          ReturnType = returnType
          Parameters = parameters
          Statements = statements
        }


let PublicMethod
    (returnType: Return)
    (name: string) 
    (parameters: Parameter list)
    (statements: Statement list) =
    Member.Method
        { MethodName = SimpleNamedItem name
          Scope = Public
          StaticOrInstance = Instance 
          IsExtension = false
          ReturnType = returnType
          Parameters = parameters
          Statements = statements
        }

let method 
    (scope: Scope)
    (staticOrInstance: StaticOrInstance)
    (returnType: Return)
    (name: NamedItem) 
    (parameters: Parameter list)
    (statements: Statement list) =
    Member.Method
        { MethodName = name
          Scope = scope
          StaticOrInstance = staticOrInstance 
          IsExtension = false
          ReturnType = returnType
          Parameters = parameters
          Statements = statements
        }

let extensionMethod 
    (scope: Scope) 
    (returnType: Return)
    (name: string) 
    (parameters: Parameter list)
    (statements: Statement list) =
    Member.Method
        { MethodName = SimpleNamedItem name
          Scope = scope
          StaticOrInstance = Static
          IsExtension = true
          ReturnType = returnType
          Parameters = parameters
          Statements = statements
        }

let cls 
    (scope: Scope) 
    (name: string) 
    (members: Member list) =
    Member.Class
        { ClassName = SimpleNamedItem name
          Scope = scope
          StaticOrInstance = Instance 
          Members = members
        }

let genericCls 
    (scope: Scope) 
    (name: NamedItem) 
    (members: Member list) =
    Member.Class
        { ClassName = name
          Scope = scope
          StaticOrInstance = Instance 
          Members = members
        }    