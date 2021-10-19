using System.CommandLine;
using System.IO;

namespace Jackfruit
{
    public class SystemCommandLineImplementation
    {
        public static Command BuildRoot()
        {
            return new RootCommand
            {
                new Option<DirectoryInfo>("additionalrobingpath"),
                new Option<FileInfo>("additionalDeps"),
                new Option<FileInfo>("depsfile"),
                new Option<string>("fxVersion"),
                new Option<RollForward>("rollForward"),
                new Option<FileInfo>("runtimeconfig")
            };

        }
    }
}
