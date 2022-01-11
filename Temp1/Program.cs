using Generator.ConsoleSupport;

static void Main(string[] args)
{
    var app = new ConsoleApplication();
    app.MapInferred("", HelloTo);

}

static void HelloTo(string who, int howManyTimes)
{
    howManyTimes = howManyTimes <= 0 ? 0 : howManyTimes;
    for (int i = 0; i < howManyTimes; i++)
    {
        Console.WriteLine($"Hello {who}");
    }
}
