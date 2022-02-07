namespace DemoHandlers
{
    public static class Handlers
    {
        private static void Greet(string greeting, string name)
        {
            var defaultGreeting = "Hello";
            Console.WriteLine($"{greeting ?? defaultGreeting}, {name}"); 
            return ;
        }
        public static void StarTrek(string greeting, bool kirk, bool spock, bool uhura) {
            if (kirk) { Greet(greeting, "James T. Kirk"); }
            if (spock) { Greet(greeting, "Spock"); }
            if (uhura) { Greet(greeting, "Nyota Uhura"); }

        }
        public static void NextGeneration(string greeting, bool picard)  {
            if (picard) { Greet(greeting, "Jean-Luc Picard"); }
        } 
        public static void DeepSpaceNine(string greeting, bool sisko, bool odo, bool dax, bool worf, bool oBrien)  {
            if (sisko) { Greet(greeting, "Benjamin Sisko"); }
            if (odo) { Greet(greeting, "Constable Odo"); }
            if (dax) { Greet(greeting, "Ezri Dax"); }
            if (worf) { Greet(greeting, "Worf"); }
            if (oBrien) { Greet(greeting, "Miles O'Brien"); }
        }
        public static void Voyager(string greeting, bool janeway, bool chakotay, bool torres, bool tuvok, bool sevenOfNine)  {
            if (janeway) { Greet(greeting, "Kathryn Janeway"); }
            if (chakotay) { Greet(greeting, "Chakotay"); }
            if (torres) { Greet(greeting, "B'Elanna Torres"); }
            if (tuvok) { Greet(greeting, "Tuvok"); }
            if (sevenOfNine) { Greet(greeting, "Sevan of Nine"); }
        }
    }
}
