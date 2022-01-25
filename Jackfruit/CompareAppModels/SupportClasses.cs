using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jackfruit
{
    public abstract class CommandBase : ICommandHandler
    {
        public CommandBase AddSubCommand(Delegate codeToRun, string? archetype = null) { return null; }

        public abstract Task<int> InvokeAsync(InvocationContext context);
    }

    // NOTES:
    // Lookup differs from Dictionary because Dictionary is a sparse model for just that concern,
    // and Lookup is a single dictionary with a data type that has all values. I doubt we should
    // do both.
    //
    // We will not use any of the sources at runtime. They are all for generation.
    public enum DescriptionSource
    {
        Dictionary = 1, // Should dictionary be included? It implies it could be entered and ignored
        XmlComment = 2, // I think users should be able to request xml comments be ignored, as they may be using that for internal reasons
        Attribute = 4,
        Lookup = 8,
        All = 15
    }

    public enum AliasSource
    {
        Archetype = 0b_0001,  // Should archetype be included? It implies it could be entered and ignored
        Dictionary = 0b_0010, 
        XmlComment = 0b_0100, // convention:  param name="pname"> -x | My description</param>
        Attribute = 0b_1000,
        Lookup = 0b_0000_0001,v
        All = 0b1111_0001
    }

    public enum IsArgumentSource
    {
        ArchetypeOnly = 0,
        Dictionary = 1,
        XmlComment = 2, // convention:  param name="pname"> ARG | My description</param>
        Attribute = 4,
        Lookup = 8,
        All = 15
    }

    public enum OptionArgumentNameSource
    {
        Dictionary = 1,
        XmlComment = 2, // convention:  param name="pname"> OPT ARGNAME  | My description</param>
        Attribute = 4,
        Lookup = 8,
        All = 15
    }



    public class CliApp
    {
        public Dictionary<string, string> CommonAliases { get; } = new();
        public Dictionary<Symbol, string> Descriptions { get; set; }
        public Dictionary<Symbol, bool> IsArgument { get; set; }
        public Dictionary<Symbol, string> OptionArgumentName { get; set; }

        // This is compile time extensible
        public void AddDelegatePattern(string pattern) { }

        // This isn't compile time extensible
        public List<string> delegatePatterns = new()
        {
            "Run*",
            "*"
        };

        public DescriptionSource DescriptionSources = DescriptionSource.All;
        public AliasSource AliasSources = AliasSource.All;
        public IsArgumentSource IsArgumentSource = IsArgumentSource.All;
        public OptionArgumentNameSource OptionArgumentNameSource = OptionArgumentNameSource.All;
        public void AddCommand(Delegate codeToRun) { }
        public void AddCommand(string archetype, Delegate? codeToRun) { }
        public void AddCommonAliases(List<(string alias, string optionName)> pairs)
        {
            foreach (var pair in pairs)
            {
                CommonAliases.Add(pair.alias, pair.optionName);
            }
        }
    }


}
