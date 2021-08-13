namespace ConsoleSupport
{
    internal record Info();

    internal record OptionInfo : Info
    {
        private readonly List<string> aliases = new();


        internal OptionInfo(string name, Type type, string? alias = null, string? argHelpName = null)
        {
            Name = name;
            Type = type;
            if (alias is not null)
            {
                aliases.Add(alias);
            }
            ArgHelpName = argHelpName;
        }

        internal void AddAlias(string alias) 
            => aliases.Add(alias);

        internal string Name { get; init; }
        internal Type Type { get; init; }
        internal string? ArgHelpName { get; init; }
        internal IEnumerable<string> Aliases => aliases;
    }
    internal record ArgInfo : Info
    {

        internal ArgInfo(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        internal string Name { get; init; }
        internal Type Type { get; init; }
    }
    internal record CommandInfo : Info
    {
        private readonly List<ArgInfo> arguments = new();
        private readonly List<OptionInfo> options = new();
        private readonly List<CommandInfo> parents = new();
        private readonly List<string> aliases = new();

        internal CommandInfo(string name, string? alias = null)
        {
            Name = name;
            if (alias is not null)
            {
                aliases.Add(alias);
            }
        }

        internal void Add(Info info)
        {
            switch (info)
            {
                case OptionInfo opt:
                    options.Add(opt);
                    break;
                case ArgInfo arg:
                    arguments.Add(arg);
                    break;
                default:
                    throw new InvalidOperationException("Can only add options and args to CommandInfo");
            }
        }

        internal void AddParents(IEnumerable<CommandInfo> parents)
            => this.parents.AddRange(parents);

        internal string Name { get; init; }
        internal Delegate? Delegate { get; set; }
        internal IEnumerable<string> Aliases => aliases;
        internal IEnumerable<OptionInfo> Options => options;
        internal IEnumerable<CommandInfo> Parents => parents;
        internal IEnumerable<ArgInfo> Arguments => arguments;
    }

}
