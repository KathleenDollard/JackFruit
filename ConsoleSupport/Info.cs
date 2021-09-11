namespace ConsoleSupport
{
    public record Info();

    public record OptionInfo : Info
    {
        private readonly List<string> aliases = new();


        public OptionInfo(string name, Type type, string? alias = null, string? argHelpName = null)
        {
            Name = name;
            Type = type;
            if (alias is not null)
            {
                aliases.Add(alias);
            }
            ArgHelpName = argHelpName;
        }

        public void AddAlias(string alias) 
            => aliases.Add(alias);

        public string Name { get; init; }
        public Type Type { get; init; }
        public string? ArgHelpName { get; init; }
        public IEnumerable<string> Aliases => aliases;
    }
    public record ArgInfo : Info
    {

        public ArgInfo(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; init; }
        public Type Type { get; init; }
    }
    public record CommandInfo : Info
    {
        private readonly List<ArgInfo> arguments = new();
        private readonly List<OptionInfo> options = new();
        private readonly List<CommandInfo> parents = new();
        private readonly List<string> aliases = new();

        public CommandInfo(string name, string? alias = null)
        {
            Name = name;
            if (alias is not null)
            {
                aliases.Add(alias);
            }
        }

        public void Add(Info info)
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

        public void AddParents(IEnumerable<CommandInfo> parents)
            => this.parents.AddRange(parents);

        public string Name { get; init; }
        public Delegate? Delegate { get; set; }
        public IEnumerable<string> Aliases => aliases;
        public IEnumerable<OptionInfo> Options => options;
        public IEnumerable<CommandInfo> Parents => parents;
        public IEnumerable<ArgInfo> Arguments => arguments;
    }

}
