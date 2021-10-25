namespace TestCode
{
    public static class Handlers
    {
        public static void A(string one) { }
        public static void BLongName(string packageName, int two, string three) { }
        public static void C() { }
    }

    public static class DotnetHandlers
    {
        public static void Dotnet(string project) { }
        public static void AddPackage(string packageName,
                                      string version,
                                      string framework,
                                      bool noRestore,
                                      string source,
                                      string packageDirectory,
                                      bool interactive,
                                      bool prerelease) { }
    }

 /*
  -v, --version<VERSION> The version of the package to add.
  -f, --framework<FRAMEWORK> Add the reference only when targeting a

                         specific framework.
  -n, --no-restore Add the reference without performing

                         restore preview and compatibility check.
  -s, --source<SOURCE> The NuGet package source to use during

                         the restore.
  --package-directory<PACKAGE_DIR> The directory to restore packages to.
  --interactive Allows the command to stop and wait for

                         user input or action (for example to

                         complete authentication).
  --prerelease Allows prerelease packages to be
                                     installed.
  -?, -h, --help Show command line help.
 */
}
