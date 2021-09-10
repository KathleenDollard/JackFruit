using System;
namespace TestCode
{

    public class ClassA
    {

        public void MethodA()
        {
            MapInferred("", Handlers.A);
        }
        public static void MapInferred(string archetype, Delegate handler) { }
    }
}