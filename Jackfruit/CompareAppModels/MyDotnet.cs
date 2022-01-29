using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Jackfruit;

namespace Jackfruit
{
    public partial class MyApp : CliApp { }

    public class Strategy1
    {
        public Strategy1()
        {
            var app = new MyApp();
            app.AddCommand("", Dotnet.RunDotnet);
            app.AddCommand("add <PROJECT>", null); // null means command won't be run alone. If you need types, use a dummy delegate
            app.AddCommand("add package <PACKAGE_NAME>", Dotnet.RunAddPackage);
            app.AddCommand("add reference <PROJECT_PATH>", Dotnet.RunAddReference);
            app.AddCommand("build <PROJECT_OR_SOLUTION> " +
                            "--runtime <RUNTIME_IDENTIFIER> " +
                            "--output <OUTPUT_DIR>", Dotnet.RunBuild);

            app.AddCommonAliases(new List<(string alias, string optionName)>
            { ("o", "output"),
              ("f", "framework"),
              ("v", "verbosity"),
              ("n", "no-restore"),
              ("c", "configuration"),
              ("r", "runtime"),
              ("i", "interactive") });

        }
    }

    public class Strategy2
    {
        public Strategy2()
        {
            // Patience folks. Add was definitely not a good choice for a subcommand to demo
            var app = new MyApp();
            app.AddCommand(Dotnet.RunDotnet);
            app.Dotnet.AddSubCommand(Dotnet.RunAdd);
            app.Dotnet.Add.AddSubCommand(Dotnet.RunAddReference);
            app.Dotnet.Add.AddSubCommand(Dotnet.RunAddPackage);

            app.AddCommonAliases(new List<(string alias, string optionName)>
            { ("o", "output"),
              ("f", "framework"),
              ("v", "verbosity"),
              ("n", "no-restore"),
              ("c", "configuration"),
              ("r", "runtime"),
              ("i", "interactive") });
        }

        private void CustomizeAddReference(AddCommandWrapper cmd )
        {
            cmd.AdditionalprobingpathOption.Description = "";

        }
    }

    public class Strategy3
    {
        public Strategy3()
        {
            var app = new MyApp();
            app.AddCommand(Dotnet.RunDotnet);
            app.Dotnet.AddSubCommand(Dotnet.RunAdd, "<PROJECT>");
            app.Dotnet.Add.AddSubCommand(Dotnet.RunAddReference, "package <PACKAGE_NAME>");
            app.Dotnet.Add.AddSubCommand(Dotnet.RunAddPackage, 
                            "<PROJECT_OR_SOLUTION>" +
                            "--runtime <RUNTIME_IDENTIFIER>" +
                            "--output <OUTPUT_DIR>");

            app.AddCommonAliases(new List<(string alias, string optionName)>
            { ("o", "output"),
              ("f", "framework"),
              ("v", "verbosity"),
              ("n", "no-restore"),
              ("c", "configuration"),
              ("r", "runtime"),
              ("i", "interactive") });
        }
    }

}



