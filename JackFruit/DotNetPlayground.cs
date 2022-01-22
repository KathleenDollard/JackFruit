using System.IO;
using System.Collections.Generic;
using ConsoleSupport;

namespace Jackfruit
{
    // Is it more clear for folks to add extra information? 
    //app.Map(" --additionalprobingpath --additional-deps --depsfile --fx-version --roll-forward --runtimeconfig",
    //        (DirectoryInfo additionalprobingpath, FileInfo additionalDeps, FileInfo depsfile, string fxVersion, RollForward roll_forward, FileInfo runtimeconfig)
    //        => Dotnet.RunRoot(additionalprobingpath, additionalDeps, depsfile, fxVersion, roll_forward, runtimeconfig));
    //app.MapInferred("",
    //(DirectoryInfo additionalprobingpath, FileInfo additionalDeps, FileInfo depsfile, string fxVersion, RollForward roll_forward, FileInfo runtimeconfig)
    //=> Dotnet.RunRoot(additionalprobingpath, additionalDeps, depsfile, fxVersion, roll_forward, runtimeconfig));

    // These are the minimum information. Seems better to me. Details added only when needed
    //    Args so we know what's not an option
    //    Option args when the name differs from the option
    // Naming is inferred. Normal rules plus (Camel in C#, Snake for options and comands, All Caps for arg names)
    //    <X|Y> turns into x_y
    //    Casing doesn't matter, it's considered part of the Posix rules/traditions
    // Not sure what syntax to use when arg/option name does not match parameter name
    // Not sure this is the right place for aliases
    // MapInferred should have a warning or error if an arg/option name has no parameter
    // Main mechanism for DI is the type. Does this method need a way to opt a common type like string into DI? 
    // Some things like validation and command aliases need to be applied here to the returned command.

    public class DotNetPlayground
    {
        public static void Main2(string[] args)
        {
            // Below are notes from conversation with Kevin Bost
            //var app = DefineCli();
            //var cmd = app.RootCommand();
            //var runtime = app.CommandSymbols.Add.Package.Runtime;

            //var weirdOptionOnAddPack = cmd.Descendant(new string[] { "add", "package", "--runtime" });
            //weirdOptionOnAddPack.Validation = x => true; app.Run();
        }

        public static void DefineCli(ConsoleApplication app)
        {
            // common aliases are applied only if that alias isn't expicit on another option
            // andalso the specified option does not have a different alias.
            app.AddCommonAlias("o", "output")
               .AddCommonAlias("f", "framework")
               .AddCommonAlias("v", "verbosity")
               .AddCommonAlias("n", "no-restore")
               .AddCommonAlias("c", "configuration")
               .AddCommonAlias("r", "runtime")
               .AddCommonAlias("i", "interactive");

            app.MapInferred("", Dotnet.RunRoot);
            app.MapInferred("add <PROJECT>", null); // null means command won't be run alone. If you need types, use a dummy delegate
            app.MapInferred("add package <PACKAGE_NAME>", Dotnet.RunAddPackage);
            app.MapInferred("add reference <PROJECT_PATH>", Dotnet.RunAddReference);
            app.MapInferred("build <PROJECT_OR_SOLUTION> " +
                            "--runtime <RUNTIME_IDENTIFIER> " +
                            "--output <OUTPUT_DIR>", Dotnet.RunBuild);

            // Below are notes from conversation with Kevin Bost
            // Generated class could have the generated as available properties that were easily accessed. Possibly 
            // based on the path

            //var runtime = app.CommandSymbols.Add.Package.Runtime;

            //app.LateBreakingMods.Add(runtime, option => WhateverTheUserWantsToDoWithTheOption(option) )
        }

        public static Dictionary<string, string> GetDescriptions()
           => new()
           {
               ["--additionalprobingpath"] = "Path containing probing policy and assemblies to probe for.",
               ["--additional-deps"] = " Path to additional deps.json file.",
               ["--depsfile"] = "Path to <application>.deps.json file.",
               ["--fx"] = "Version of the installed Shared Framework to use to run the application.",
               ["--roll-forward"] = "Roll forward to framework version(LatestPatch, Minor, LatestMinor, Major, LatestMajor, Disable).",
               ["--runtimeconfig"] = "Path to <application>.runtimeconfig.json file.",
               ["add"] = "Add a package or reference to a.NET project.",
               ["build"] = "Build a.NET project.",
               ["build --runtime"] = "Special run description.",
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

               ["add "] = "The project file to operate on. If a file is not specified, the command will search the current directory for one.",
               ["add --help"] = "Show help and usage information.",
               ["add package"] = "Add a NuGet package reference to the project.",
               ["add reference"] = "Add a project-to-project reference to the project.",

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

               ["add package --version"] = "The version of the package to add.",
               ["add package --framework"] = "Add the reference only when targeting a specific framework.",
               ["add package --no-restore"] = "Add the reference without performing restore preview and compatibility check.",
               ["add package --source"] = "The NuGet package source to use during the restore.",
               ["add package --package-directory"] = "The directory to restore packages to.",
               ["add package --interactive"] = "Allows the command to stop and wait for user input or action(for example to complete authentication).",
               ["add package --prerelease"] = "Allows prerelease packages to be installed.",
               ["add reference --framework"] = "Add the reference only when targeting a specific framework.",
               ["add reference --interactive"] = "Allows the command to stop and wait for user input or action(for example to complete authentication).",
           };

    }


}
/*
 
 */