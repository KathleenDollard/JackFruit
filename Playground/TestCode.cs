using System;
namespace TestCode
{

    public class ClassA
    {

        public void MethodA()
        {
            MapInferred("dotnet tool install", Handlers.A);
        }
        public static void MapInferred(string archetype, Delegate handler) { }
    }
}