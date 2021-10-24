namespace Generator


type FlatAppModel() =
    interface IAppModel<Microsoft.CodeAnalysis.IMethodSymbol> with 
        
        member _.Children _ = []
        
        // Id, method, stuff for pocket
        member _.Info model method =
            method.Name, Some method, [] // method already added
        
        member _.RunProviders commandDef =
            commandDef