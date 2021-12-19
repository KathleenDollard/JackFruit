module Generator.LanguageExpression

open Generator.Language
open Common

let method 
    (scope: Scope)
    (returnType: Return)
    (name: NamedItem) 
    (parameters: ParameterModel list)
    (statements: StatementModel list) =
    Member.Method
        { MethodName = name
          Scope = scope
          StaticOrInstance = Instance 
          IsExtension = false
          IsAsync = false
          ReturnType = returnType
          Parameters = parameters
          Statements = statements
        }


let asyncMethod 
    (scope: Scope)
    (returnType: Return)
    (name: NamedItem) 
    (parameters: ParameterModel list)
    (statements: StatementModel list) =
    Member.Method
        { MethodName = name
          Scope = scope
          StaticOrInstance = Instance 
          IsExtension = false
          IsAsync = true
          ReturnType = returnType
          Parameters = parameters
          Statements = statements
        }


let staticMethod 
    (scope: Scope)
    (returnType: Return)
    (name: NamedItem) 
    (parameters: ParameterModel list)
    (statements: StatementModel list) =
    Member.Method
        { MethodName = name
          Scope = scope
          StaticOrInstance = Static 
          IsExtension = false
          IsAsync = false
          ReturnType = returnType
          Parameters = parameters
          Statements = statements
        }

let ctor 
    (scope: Scope)
    (className: string)
    (parameters: ParameterModel list)
    (statements: StatementModel list) =
    Member.Constructor
        { ClassName = className
          Scope = scope
          StaticOrInstance = Instance 
          Parameters = parameters
          Statements = statements
        }

let staticCtor
    (scope: Scope)
    (className: string)
    (parameters: ParameterModel list)
    (statements: StatementModel list) =
    Member.Constructor
        { ClassName = className
          Scope = scope
          StaticOrInstance = Static 
          Parameters = parameters
          Statements = statements
        }
         
let field
    (fieldType: NamedItem)
    (name: string)
    (initialValue: ExpressionModel) =
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
    (parameters: ParameterModel list)
    (statements: StatementModel list) =
    Member.Method
        { MethodName = SimpleNamedItem name
          Scope = scope
          StaticOrInstance = Static
          IsExtension = true
          IsAsync = false
          ReturnType = returnType
          Parameters = parameters
          Statements = statements
        }

//let clsWithInterfaces
//    (scope: Scope) 
//    (name: string) 
//    (interfaces: NamedItem list)
//    (members: Member list) =
//    Member.Class
//        { ClassName = SimpleNamedItem name
//          Scope = scope
//          StaticOrInstance = Instance 
//          IsAsync = false
//          IsPartial = false
//          InheritedFrom = None
//          ImplementedInterfaces = interfaces
//          Members = members
//        }


let param
    (name: string)
    (paramType: NamedItem) =
    { ParameterName = name
      Type = paramType
      Default = None
      IsParams = false}

let assign 
    (item: string)
    (value: ExpressionModel) =
    StatementModel.Assign 
        { Item = item
          Value = value }

let prop
    (scope: Scope)
    (propertyType: NamedItem)
    (propertyName: string)
    (getStatements: StatementModel list)
    (setStatements: StatementModel list) =
    Member.Property
        { PropertyName = propertyName
          Type = propertyType
          StaticOrInstance = Instance
          Scope = scope
          GetStatements = getStatements
          SetStatements = setStatements }

let ifThen 
    (condition: ExpressionModel)
    (ifStatements: StatementModel list) =
    StatementModel.If
        { Condition = condition
          Statements = ifStatements
          Elses = []}

let invoke 
     ( instance: string)
     ( methodName: string)
     ( arguments: ExpressionModel list) =
     ExpressionModel.Invocation 
        { Instance = SimpleNamedItem instance 
          MethodName = SimpleNamedItem methodName
          ShouldAwait = false
          Arguments = arguments }

let invokeAwait
    ( instance: string)
    ( methodName: string)
    ( arguments: ExpressionModel list) =
    ExpressionModel.Invocation 
       { Instance = SimpleNamedItem instance 
         MethodName = SimpleNamedItem methodName
         ShouldAwait = true
         Arguments = arguments }

    
    