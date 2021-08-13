using System.IO;

namespace JackFruit
{
    public enum RollForward
    {
        LatestPatch,
        Minor,
        LatestMinor,
        Major,
        LatestMajor,
        Disable
    }

    public enum Verbosity
    {
        detailed,
        diagnostic,
        minimal, 
        normal,
        quiet
    }

    public class Dotnet
    {
        public static int RunRoot(DirectoryInfo additionalprobingpath,
                                  FileInfo additionalDeps,
                                  FileInfo depsfile,
                                  string fxVersion,
                                  RollForward rollForward,
                                  FileInfo runtimeconfig)
        {
            return 42;
        }

        public static int RunAddPackage(string packageName, 
                                        string version,
                                        string framework,
                                        bool noRestore,
                                        string source,
                                        DirectoryInfo packageDirectory,
                                        bool interactive,
                                        bool prerelease)
        {
            return 43;
        }

        public static int RunAddReference(DirectoryInfo ProjectPath, 
                                          string framework,
                                          bool interactive)
        {
            return 44;
        }

        public static int RunBuild(FileInfo project_solution,
                                   bool useCurrentRuntime,
                                   string framework,
                                   string configuration,
                                   string runtime,
                                   string versionSuffix,
                                   bool noRestore,
                                   bool interactive,
                                   Verbosity verbosity,
                                   DirectoryInfo output,
                                   bool noIncremental,
                                   bool noDependencies,
                                   bool nologo)
        {
            return 45;
        }
    }
}
