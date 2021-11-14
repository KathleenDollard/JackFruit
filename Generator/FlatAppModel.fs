namespace Generator


type FlatAppModel() =
    interface IAppModel<Microsoft.CodeAnalysis.IMethodSymbol> with 
        
        member _.Children _ = []
        
        // Id, method, stuff for pocket
        member _.Info model method =
            { InfoCommandId = Some method.Name
              Path = [ method.Name ]
              Method = Some method
              ForPocket = [] }
        
        //member _.CommandDefTransformers = []            

        //member _.MemberDefTransformers = []
