using System;
using System.Collections.Generic;
namespace CliApp
{


    public class AppBase
    {

        public static List<string> DefaultPatterns = new() { "*", "Run *", "* Handler" };
        public static void AddCommandNamePattern(string pattern) { }
        public static void RemoveCommandNamePattern(string pattern) { }
        public void AddCommand(Delegate handler) { }
        public void AddSubCommand(Delegate handler) { }
    }


    public class OriginalSeries : AppBase
    {
        public NextGeneration NextGeneration { get; set; }
        public AppBase Kirk { get; set; }
        public AppBase Spock { get; set; }
        public AppBase Uhura { get; set; }
    }


    public class NextGeneration : AppBase
    {
        public Voyager Voyager { get; set; }
        public DeepSpaceNine DeepSpaceNine { get; set; }
        public AppBase Picard { get; set; }
    }


    public class DeepSpaceNine : AppBase
    {
        public AppBase Sisko { get; set; }
        public AppBase Odo { get; set; }
        public AppBase Dax { get; set; }
        public AppBase Worf { get; set; }
        public AppBase OBrien { get; set; }
    }


    public class Voyager : AppBase
    {
        public AppBase Janeway { get; set; }
        public AppBase Chakotay { get; set; }
        public AppBase Torres { get; set; }
        public AppBase Tuvok { get; set; }
        public AppBase SevenOfNine { get; set; }
    }



    public static class Handlers
    {
        public static void OriginalSeries(string kirk, string spock, string uhura) { }
        public static void NextGeneration(string picard) { }
        public static void DeepSpaceNine(string sisko, string odo, string dax, string worf, string oBrien) { }
        public static void Voyager(string janeway, string chakotay, string torres, string tuvok, string sevenOfNine) { }
    }

    public class MyCli : AppBase
    {
        public OriginalSeries OriginalSeries { get; set; }
    }

    public class Program
    {

        public void DefineCli()
        {
            var app = new MyCli();
            AppBase.AddCommandNamePattern("Cmd*"); ;
        }
    }
}
