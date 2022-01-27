using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CommandLine.Invocation;
using System.CommandLine;
using Jackfruit;
using System.IO;
using System.ComponentModel;

namespace Jackfruit
{
    // These classes are GENERATED
    public partial class MyApp
    {
        public MyApp()
        {
            // use fully qualified here
            Dotnet = new DotnetCommandWrapper();
        }
        public DotnetCommandWrapper Dotnet { get; }
    }

    public partial class DotnetCommandWrapper : CommandBase
    {
        public Command Command { get; }
        public Option<DirectoryInfo> AdditionalprobingpathOption { get; }
        // etc
        public DotnetCommandWrapper()
        {
            Command = new Command("dotnet");

            AdditionalprobingpathOption = new Option<DirectoryInfo>("additionalprobingpath");
            Command.Add(AdditionalprobingpathOption);
            // etc

            Add = new AddCommandWrapper();
            Command.Add(Add.Command);
            // etc

            Command.Handler = this;
        }

        public AddCommandWrapper Add { get; }

        public DirectoryInfo? AdditionalprobingpathOptionResult(InvocationContext context)
            => context.ParseResult.GetValueForOption<DirectoryInfo>(AdditionalprobingpathOption);
        public FileInfo? AdditionalDepsOptionResult(InvocationContext context)
            => null;
        public FileInfo? DepsfileOptionResult(InvocationContext context)
            => null;
        public string? FxVersionOptionResult(InvocationContext context)
            => null;
        public RollForward? RollForwardOptionResult(InvocationContext context)
            => null;
        public FileInfo? RuntimeconfigOptionResult(InvocationContext context)
            => null;

        public override Task<int> InvokeAsync(InvocationContext context)
        {
            Dotnet.RunDotnet(
                AdditionalprobingpathOptionResult(context),
                AdditionalDepsOptionResult(context),
                DepsfileOptionResult(context),
                FxVersionOptionResult(context),
                RollForwardOptionResult(context),
                RuntimeconfigOptionResult(context));
            return Task.FromResult(context.ExitCode);
        }

        private Action<DotnetCommandWrapper> CustomizeAction { get; set; }

        public DotnetCommandWrapper AddSubCommand(Delegate codeToRun) { return this; }

        public DotnetCommandWrapper Customize(Action<DotnetCommandWrapper> customizeAction)
        {
            CustomizeAction = customizeAction;
            return this;
        }
    }

    public partial class AddCommandWrapper : CommandBase
    {
        public Command Command { get; }
        public Option<DirectoryInfo> AdditionalprobingpathOption { get; }
        // etc
        public AddCommandWrapper()
        {
            Command = new Command("add");

            Package = new PackageCommandWrapper();
            Command.Add(Package.Command);
            Reference = new ReferenceCommandWrapper();
            Command.Add(Reference.Command);
            Command.Handler = this;
        }
        public PackageCommandWrapper Package { get; }
        public ReferenceCommandWrapper Reference { get; }

        public override Task<int> InvokeAsync(InvocationContext context)
        {
            return Task.FromResult(0);
        }

        public AddCommandWrapper AddSubCommand(Delegate codeToRun) { return this; }


        public AddCommandWrapper Customize(Action<AddCommandWrapper> customizeAction)
        {
            customizeAction?.Invoke(this);
            return this;
        }
    }

    public partial class PackageCommandWrapper : CommandBase
    {
        public Command Command { get; }
        public Argument<FileInfo> ProjectArgument { get; }
        public Argument<string> PackageNameArgument { get; }
        public Option<string> VersionOption { get; }
        // etc

        public PackageCommandWrapper()
        {
            Command = new Command("package");

            ProjectArgument = new Argument<FileInfo>("project");
            Command.Add(ProjectArgument);
            PackageNameArgument = new Argument<string>("packageName");
            Command.Add(PackageNameArgument);
            VersionOption = new Option<string>("version");
            Command.Add(VersionOption);
            // etc

            Command.Handler = this;
        }

        public FileInfo? Project(InvocationContext context)
            => null;
        public string? PackageName(InvocationContext context)
            => null;
        public string? Version(InvocationContext context)
            => null;
        public string? Framework(InvocationContext context)
            => null;
        public bool? NoRestore(InvocationContext context)
            => null;
        public string? Source(InvocationContext context)
            => null;
        public DirectoryInfo? PackageDirectory(InvocationContext context)
            => null;
        public bool? Interactive(InvocationContext context)
            => null;
        public bool? Prerelease(InvocationContext context)
            => null;

        public override Task<int> InvokeAsync(InvocationContext context)
        {
            Dotnet.RunAddPackage(
                Project(context),
                PackageName(context),
                Version(context),
                Framework(context),
                NoRestore(context),
                Source(context),
                PackageDirectory(context),
                Interactive(context),
                Prerelease(context));
            return Task.FromResult(context.ExitCode);
        }
    }

    public partial class ReferenceCommandWrapper : CommandBase
    {
        public Command Command { get; }
        public Argument<FileInfo> ProjectArgument { get; }
        public Argument<DirectoryInfo> ProjectPathArgument { get; }
        public Option<string> VersionOption { get; }
        // etc

        public ReferenceCommandWrapper()
        {
            Command = new Command("package");

            ProjectArgument = new Argument<FileInfo>("project");
            Command.Add(ProjectArgument);
            ProjectPathArgument = new Argument<DirectoryInfo>("project-path");
            Command.Add(ProjectPathArgument);
            VersionOption = new Option<string>("version");
            Command.Add(VersionOption);
            // etc

            Command.Handler = this;
        }

        public FileInfo? ProjectResult(InvocationContext context)
            => null;
        public DirectoryInfo? ProjectPathResult(InvocationContext context)
            => null;
        public string? FrameworkResult(InvocationContext context)
           => null;
        public bool? InteractiveResult(InvocationContext context)
            => null;

        public override Task<int> InvokeAsync(InvocationContext context)
        {
            Dotnet.RunAddReference(
                ProjectResult(context),
                ProjectPathResult(context),
                FrameworkResult(context),
                InteractiveResult(context));
            return Task.FromResult(context.ExitCode);
        }
    }

    public partial class BuildCommandWrapper : CommandBase
    {
        public Command Command { get; }
        public Argument<FileInfo> ProjectOrSolutionArgument { get; }
        public Option<bool> UseCurrentRuntimeOption { get; }
        public Option<string> FrameworkOption { get; }
        public Option<string> ConfigurationOption { get; }
        public Option<string> RuntimeOption { get; }
        public Option<string> VersionSuffixOption { get; }
        public Option<bool> NoRestoreOption { get; }
        public Option<bool> InteractiveOption { get; }
        public Option<Verbosity> VerbosityOption { get; }
        public Option<DirectoryInfo> OutputOption { get; }
        public Option<bool> NoIncrementalOption { get; }
        public Option<bool> NoDependenciesOption { get; }
        public Option<bool> NologoOption { get; }

        public BuildCommandWrapper()
        {
            Command = new Command("package");

            ProjectOrSolutionArgument = new Argument<FileInfo>("projecOrSolution");
            Command.Add(ProjectOrSolutionArgument);
            UseCurrentRuntimeOption = new Option<bool>("useCurrentRuntime");
            Command.Add(UseCurrentRuntimeOption);
            FrameworkOption = new Option<string>("framework");
            Command.Add(FrameworkOption);
            ConfigurationOption = new Option<string>("configuration");
            Command.Add(ConfigurationOption);
            RuntimeOption = new Option<string>("runtime");
            Command.Add(RuntimeOption);
            VersionSuffixOption = new Option<string>("versionSuffix");
            Command.Add(VersionSuffixOption);
            NoRestoreOption = new Option<bool>("noRestore");
            Command.Add(NoRestoreOption);
            InteractiveOption = new Option<bool>("interactive");
            Command.Add(InteractiveOption);
            VerbosityOption = new Option<Verbosity>("verbosity");
            Command.Add(VerbosityOption);
            OutputOption = new Option<DirectoryInfo>("output");
            Command.Add(OutputOption);
            NoIncrementalOption = new Option<bool>("noIncremental");
            Command.Add(NoIncrementalOption);
            NoDependenciesOption = new Option<bool>("noDependencies");
            Command.Add(NoDependenciesOption);
            NologoOption = new Option<bool>("nologo");
            Command.Add(NologoOption);

            Command.Handler = this;
        }

        public FileInfo? ProjecOrSolutionResult(InvocationContext context) => null;
        public bool? UseCurrentRuntimeResult(InvocationContext context) => null;
        public string? FrameworkResult(InvocationContext context) => null;
        public string? ConfigurationResult(InvocationContext context) => null;
        public string? RuntimeResult(InvocationContext context) => null;
        public string? VersionSuffixResult(InvocationContext context) => null;
        public bool? NoRestoreResult(InvocationContext context) => null;
        public bool? InteractiveResult(InvocationContext context) => null;
        public Verbosity? VerbosityResult(InvocationContext context) => null;
        public DirectoryInfo? OutputResult(InvocationContext context) => null;
        public bool? NoIncrementalResult(InvocationContext context) => null;
        public bool? NoDependenciesResult(InvocationContext context) => null;
        public bool? NologoResult(InvocationContext context) => null;


        public override Task<int> InvokeAsync(InvocationContext context)
        {
            Dotnet.RunBuild(
                   ProjecOrSolutionResult(context),
                   UseCurrentRuntimeResult(context),
                   FrameworkResult(context),
                   ConfigurationResult(context),
                   RuntimeResult(context),
                   VersionSuffixResult(context),
                   NoRestoreResult(context),
                   InteractiveResult(context),
                   VerbosityResult(context),
                   OutputResult(context),
                   NoIncrementalResult(context),
                   NoDependenciesResult(context),
                   NologoResult(context));
            return Task.FromResult(context.ExitCode);
        }
    }
}