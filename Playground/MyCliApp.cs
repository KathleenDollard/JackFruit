using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CliApp;

namespace PlaygroundCliApp
{
    //public abstract class AppBase<T> : AppBase
    //    where T : CliBase, new()
    //{
    //    public abstract T RootCommand { get; }
    //}

    public class OriginalSeriesCli : CliBase
    {
    }

    public class MyAppBase : AppBase
    {
        public OriginalSeriesCli RootCommand { get; } = new();

        public static new MyAppBase CreateWithRootCommand(Delegate codeToRun)
        { return new MyAppBase(); }
    }
}
