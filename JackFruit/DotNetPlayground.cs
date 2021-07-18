using System.IO;

namespace JackFruit
{
    public class DotNetPlayground
    {
        public static void DefineCli(ConsoleApplication app)
        {
            app.MapInferred("A",
                    (DirectoryInfo additionalprobingpath, FileInfo additionalDeps, FileInfo depsfile, string fxVersion, RollForward roll_forward, FileInfo runtimeconfig)
                    => Dotnet.RunRoot(additionalprobingpath, additionalDeps, depsfile, fxVersion, roll_forward, runtimeconfig));
            app.MapInferred("B", Dotnet.RunRoot);


            app.Map("C --additionalprobingpath --additional-deps --depsfile --fx-version --roll-forward --runtimeconfig",
                    (DirectoryInfo additionalprobingpath, FileInfo additionalDeps, FileInfo depsfile, string fxVersion, RollForward roll_forward,FileInfo runtimeconfig)
                    => Dotnet.RunRoot(additionalprobingpath, additionalDeps, depsfile, fxVersion, roll_forward, runtimeconfig));
        }

        public Dictionary<string, string> GetDescriptions()
           => new()
           {
               ["--additionalprobingpath"] = "< path > Path containing probing policy and assemblies to probe for.",
               ["--additional-deps]< path >"] = " Path to additional deps.json file.",
               ["--depsfile"] = "Path to<application>.deps.json file.",
               ["--fx"] = " - version < version > Version of the installed Shared Framework to use to run the application.",
               ["--roll-forward"] = " - forward < setting > Roll forward to framework version(LatestPatch, Minor, LatestMinor, Major, LatestMajor, Disable).",
               ["--runtimeconfig"] = "Path to<application>.runtimeconfig.json file.",
               ["add"] = "Add a package or reference to a.NET project.",
               ["build"] = "Build a.NET project.",
               ["build-server"] = "Interact with servers started by a build.",
               ["clean"] = "Clean build outputs of a .NET project.",
               ["help"] = "Show command line help.",
               ["list"] = "List project references of a .NET project.",
               ["msbuild"] = "Run Microsoft Build Engine (MSBuild) commands.",
               ["new"] = "Create a new .NET project or file.",
               ["nuget"] = "Provides additional NuGet commands.",
               ["pack"] = "Create a NuGet package.",
               ["publish"] = "Publish a.NET project for deployment.",
               ["remove"] = "Remove a package or reference from a .NET project.",
               ["restore"] = "Restore dependencies specified in a.NET project.",
               ["run"] = "Build and run a.NET project output.",
               ["sdk"] = "Manage .NET SDK installation.",
               ["sln"] = "Modify Visual Studio solution files.",
               ["store"] = "Store the specified assemblies in the runtime package store.",
               ["test"] = "Run unit tests using the test runner specified in a.NET project.",
               ["tool"] = "Install or manage tools that extend the.NET experience.",
               ["vstest"] = "Run Microsoft Test Engine (VSTest) commands.",

               ["add <PROJECT>"] = "The project file to operate on. If a file is not specified, the command will search the current directory for one.",
               ["add --help"] = "Show help and usage information.",
               ["add package"] = "<PACKAGE_NAME>    Add a NuGet package reference to the project.",
               ["add reference"] = "<PROJECT_PATH>  Add a project-to-project reference to the project.",

               ["build <PROJECT | SOLUTION>"] = "The project or solution file to operate on. If a file is not specified, the command will search the current directory for one.",
               ["build --use-current-runtime"] = "Use current runtime as the target runtime.",
               ["build --framework"] = "The target framework to build for. The target framework must also be specified in the project file.",
               ["build --configuration"] = "The configuration to use for building the project. The default for most projects is 'Debug'.",
               ["build --runtime"] = "The target runtime to build for.",
               ["build --version-suffix"] = "Set the value of the $(VersionSuffix) property to use when building the project.",
               ["build --no-restore"] = "Do not restore the project before building.",
               ["build --interactive"] = "Allows the command to stop and wait for user input or action (for example to complete authentication).",
               ["build --verbosity"] = "Set the MSBuild verbosity level. Allowed values are q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic].",
               ["build --debug"] = "",
               ["build --output"] = "The output directory to place built artifacts in.",
               ["build --no-incremental"] = "Do not use incremental building.",
               ["build --no-dependencies"] = "Do not build project-to-project references and only build the specified project.",
               ["build --nologo"] = "Do not display the startup banner or the copyright message.",
               ["build --help"] = "Show help and usage information",
           };

    }


}
/*
 
 */