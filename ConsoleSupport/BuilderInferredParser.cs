
// START HERE: Handle arguments specified as parts on options and aliases on options.

using System.CommandLine.Parsing;

namespace ConsoleSupport
{
    public class BuilderInferredParser
    {
        // !!!!!!!!!!! THIS FILLS INFO RECORDS INSTEAD OF BUILDING SYMBOLS BECAUSE IT WILL CHANGE TO SOURCE GENERATION !!!!!!!!!!!!


        internal void AddCommonAlias(string optionName, string alias)
            => commonAliases.Add(optionName, alias);
        internal void AddDIType(Type diType)
            => diTypes.Add(diType);

        public CommandInfo MapInferred(string def, Delegate? del)
        {
            var commandInfo = del is null
                                ? GetParentOnlyCommand(def)
                                : GetCommand(def, GetHandlerParameters(del));
            commandInfo.Delegate = del;
            return commandInfo;

            static IEnumerable<(string? name, Type type)> GetHandlerParameters(Delegate? del)
                       => del is null
                          ? throw new NotImplementedException()
                          : del.Method.GetParameters().Select(x => (x.Name, x.ParameterType));
        }

        private readonly List<Type> diTypes = new();
        private readonly Dictionary<string, string> commonAliases = new();

        private record Part(string Name);
        private record CommandArgPart(string Name, CommandPart Parent) : Part(Name);
        private record OptionArgPart(string Name, OptionPart Parent) : Part(Name);
        private record OptionPart(string Name, string? Alias = null) : Part(Name);
        private record CommandPart(string Name) : Part(Name);

        private CommandInfo GetParentOnlyCommand(string def)
        {
            // For arguments, assuming string seems like it will often be correct
            // Maybe no way to guess for options, but optins may be rare in this less common location
            var parts = GetParts(def);
            var commands = GetCommandsFromParts(parts);
            var leafCommand = GetLeafCommand(commands);
            foreach (var part in parts.Skip(commands.Count()))
            {
                Info info = part switch
                {
                    CommandPart => throw new ArgumentException("Commands must appear before other symbols. Use a separate call for parent commands."),
                    OptionPart => new OptionInfo(part.Name, typeof(bool)),
                    CommandArgPart => new ArgInfo(part.Name, typeof(string)),
                    _ => throw new InvalidOperationException("How did we get here?")
                };
                leafCommand.Add(info);
            }
            return leafCommand;
        }

        private CommandInfo GetCommand(string def,
                                       IEnumerable<(string? name, Type type)> parms)
        {
            var parts = GetParts(def);

            var results = FillCommand(parts, parms);

            return results switch
            {
                ResultsOk<CommandInfo> ok => ok.Value,
                _ => throw new InvalidOperationException(results.Message)
            };

         }

        private static IEnumerable<Part> GetParts(string def)
        {
            var arr = CommandLineStringSplitter.Instance.Split(def).ToArray();
            Part lastPart = new CommandPart("");
            var parts = new List<Part>()
                {
                    lastPart
                };
            for (int i = 0; i < arr.Length; i++)
            {
                var part = ClassifyPart(arr, i, lastPart);
                if (part is CommandPart || part is OptionPart)
                {
                    lastPart = part;
                }
                parts.Add(part);
            }
            return parts;

            static Part ClassifyPart(string[] parts, int i, Part lastPart)
            {
                var part = parts[i];
                return (part[0], part[1], lastPart) switch
                {
                    ('<', _, CommandPart cmd) => new CommandArgPart(part, cmd),
                    ('<', _, OptionPart opt) => new OptionArgPart(part, opt),
                    ('-', '-', _) => new OptionPart(part, GetOptionAliasPart(part)),
                    ('-', _, _) => throw new InvalidOperationException("Single dashes not allowed in definitions"),
                    _ => new CommandPart(part),

                };
            }

            static string? GetOptionAliasPart(string part)
            {
                var pos = part.IndexOf("|");
                return pos switch
                {
                    < 0 => null,
                    1 => throw new InvalidOperationException("Option name cannot begin with |"),
                    _ => part[pos..]
                };
            }
        }


        private Results FillCommand(IEnumerable<Part> parts,
                                    IEnumerable<(string? name, Type type)> parms)
        {
            try
            {
                var leafCommand = FillLeafCommand(GetCommandsFromParts(parts), parts, parms, diTypes);
                ApplyCommonAliases(leafCommand, commonAliases);
                return new ResultsOk<CommandInfo>(leafCommand);

            }
            catch (Exception e)
            {
                return new ResultsError(e.Message);
            }

            static CommandInfo FillLeafCommand(IEnumerable<CommandInfo> commands,
                                    IEnumerable<Part> parts,
                                    IEnumerable<(string? name, Type type)> parms,
                                    List<Type> diTypes)
            {
                var leafCommand = GetLeafCommand(commands);
                var partsDict = parts.Where(x => x is OptionPart || x is CommandArgPart).ToDictionary(x => x.Name);
                foreach (var (name, type) in parms)
                {
                    if (name is null)
                    {
                        throw new InvalidOperationException("How did we get an unnamed parm?");
                    }
                    else if (partsDict.TryGetValue(name, out var part))
                    {
                        var symbolInfo = GetInfo(part.Name, type, part, parts);
                        leafCommand.Add(symbolInfo);
                    }
                    else if (diTypes.Contains(type))
                    {
                        // skip, System.CommandLine will pick up 
                    }
                    else // infer as option
                    {
                        var cliName = name.ToKebabCase();
                        var symbolInfo = GetOptionInfo(cliName, type);
                        leafCommand.Add(symbolInfo);
                    }
                }
                return leafCommand;
            }



            static Info GetInfo(string name, Type type, Part part, IEnumerable<Part> parts)
                => part switch
                {
                    CommandArgPart => new ArgInfo(name, type),
                    OptionPart opt => new OptionInfo(name, type, opt.Alias, GetOptionArgInfo(parts, name, type)),
                    _ => throw new InvalidOperationException()
                };

            static Info GetOptionInfo(string name, Type type, string? alias = null, string? argHelpName = null)
                => new OptionInfo(name, type);

            static string? GetOptionArgInfo(IEnumerable<Part> parts, string optionName, Type type)
            {
                var part = parts.OfType<OptionArgPart>()
                                .Where(x => x.Parent.Name == optionName)
                                .FirstOrDefault();
                return part?.Name;
            }

            static void ApplyCommonAliases(CommandInfo command, Dictionary<string, string> aliases)
            {
                foreach (var opt in command.Options)
                {
                    if (opt.Aliases.Any())
                    {
                        return; // alias already set
                    }
                    if (aliases.TryGetValue(opt.Name, out var alias))
                    {
                        if (command.Options.Any(x => x.Aliases.Contains(alias)))
                        {
                            return; // alias is in use
                        }
                        opt.AddAlias(alias);
                    }
                }
            }
        }

        private static CommandInfo GetLeafCommand(IEnumerable<CommandInfo> commands)
        {
            var commandsArr = commands.ToArray();
            commandsArr[^1].AddParents(commandsArr[0..^1]);
            return commandsArr[^1];
        }

        private static  IEnumerable<CommandInfo> GetCommandsFromParts(IEnumerable<Part> parts)
        {
            var commands = new List<CommandInfo>();
            foreach (var part in parts)
            {
                if (part is not CommandPart cmd)
                {
                    break;
                }
                commands.Add(new CommandInfo(part.Name));
            }
            return commands;
        }
    }
}
