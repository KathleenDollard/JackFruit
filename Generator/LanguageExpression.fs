module Generator.LanguageExpression

open Generator.Language
open Common

let method 
    (scope: Scope)
    (returnType: Return)
    (name: NamedItem) 
    (parameters: Parameter list)
    (statements: Statement list) =
    Member.Method
        { MethodName = name
          Scope = scope
          StaticOrInstance = Instance 
          IsExtension = false
          ReturnType = returnType
          Parameters = parameters
          Statements = statements
        }


let staticMethod 
    (scope: Scope)
    (returnType: Return)
    (name: NamedItem) 
    (parameters: Parameter list)
    (statements: Statement list) =
    Member.Method
        { MethodName = name
          Scope = scope
          StaticOrInstance = Static 
          IsExtension = false
          ReturnType = returnType
          Parameters = parameters
          Statements = statements
        }

let ctor 
    (scope: Scope)
    (className: string)
    (parameters: Parameter list)
    (statements: Statement list) =
    Member.Constructor
        { ClassName = SimpleNamedItem className
          Scope = scope
          StaticOrInstance = Instance 
          Parameters = parameters
          Statements = statements
        }

let staticCtor
    (scope: Scope)
    (className: string)
    (parameters: Parameter list)
    (statements: Statement list) =
    Member.Constructor
        { ClassName = SimpleNamedItem className
          Scope = scope
          StaticOrInstance = Static 
          Parameters = parameters
          Statements = statements
        }
         
let field
    (fieldType: NamedItem)
    (name: string)
    (initialValue: Expression) =
    Member.Field
        { FieldName = name
          StaticOrInstance = Instance 
          IsReadonly = false
          FieldType = fieldType
          Scope = Private
          InitialValue = Some initialValue
        }
        
let readonlyField 
    (fieldType: NamedItem)
    (name: string) =
    Member.Field
        { FieldName = name
          StaticOrInstance = Instance 
          IsReadonly = true
          FieldType = fieldType
          Scope = Private
          InitialValue = None
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
          InheritedFrom = None
          ImplementedInterfaces = []
          Members = members
        }

let clsWithBase
    (scope: Scope) 
    (name: string) 
    (inheritedFrom: NamedItem)
    (members: Member list) =
    Member.Class
        { ClassName = SimpleNamedItem name
          Scope = scope
          StaticOrInstance = Instance 
          InheritedFrom = Some inheritedFrom
          ImplementedInterfaces = []
          Members = members
        }


let clsWithInterfaces
    (scope: Scope) 
    (name: string) 
    (interfaces: NamedItem list)
    (members: Member list) =
    Member.Class
        { ClassName = SimpleNamedItem name
          Scope = scope
          StaticOrInstance = Instance 
          InheritedFrom = None
          ImplementedInterfaces = interfaces
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
          InheritedFrom = None
          ImplementedInterfaces = []
          Members = members
        }    

let param
    (name: string)
    (paramType: NamedItem) =
    { ParameterName = name
      Type = paramType
      Default = None
      IsParams = false}

let assign 
    (item: string)
    (value: Expression) =
    Statement.Assign 
        { Item = item
          Value = value }

let prop
    (scope: Scope)
    (propertyType: NamedItem)
    (propertyName: string)
    (getStatements: Statement list)
    (setStatements: Statement list) =
    Member.Property
        { PropertyName = propertyName
          Type = propertyType
          StaticOrInstance = Instance
          Scope = scope
          GetStatements = getStatements
          SetStatements = setStatements }

let ifThen 
    (condition: Expression)
    (ifStatements: Statement list) =
    Statement.If
        { Condition = condition
          Statements = ifStatements
          Elses = []}