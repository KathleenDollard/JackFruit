namespace TestCode
{
    public static class Handlers 
    {
        public static void A(string one) { }
        public static void B(string PackageName, int two) {}
    }

    public static class DotnetHandlers
    {
        public static void Dotnet(string project) { }
        public static void AddPackage(string packageName, int other) { }
    }
}
