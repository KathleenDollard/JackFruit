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

    public class Dotnet
    {
        public static int RunRoot(DirectoryInfo additionalprobingpath,
                                     FileInfo additionalDeps,
                                     FileInfo depsfile,
                                     string fxVersion,
                                     RollForward rollForward,
                                     FileInfo runtimeconfig)
        {
            return 41;
        }
    }
}
