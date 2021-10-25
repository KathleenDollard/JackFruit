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
                                      bool prerelease)
        { }
    }


}
