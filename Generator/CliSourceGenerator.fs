namespace Generator

open Microsoft.CodeAnalysis


[<Generator>]
type CliSourceGenerator() =
    interface ISourceGenerator with
        member _.Initialize(context) =
            ()

        member _.Execute(context) =
            let output = @"
            namespace GeneratedNamespace
            {
                public class GeneratedClass
                {
                    public static void GeneratedMethod()
                    {
                        // generated code
                    }
                }
            }"
            context.AddSource("myGeneratedFile.cs", output)
            ()
