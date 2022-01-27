using System;

namespace Jackfruit;

//These classes are expected to be in another class lib (nuget package) that takes a dependency on JackFruit
public interface ICustomizable<T> where T : CommandBase
{
    T Customize(Action<T> customizeAction);
}

public static class CustomizableExtensions
{
    public static T AddSubCommand<T>(this T command, Delegate codeToRun, string arctype)
        where T : CommandBase
    {
        //TODO: Is this even possible to do generically?
        return command;
    }

    public static ICustomizable<T> Customize<T>(this ICustomizable<T> commandWrapper, string archtype)
        where T : CommandBase
    {
        commandWrapper.Customize(x => ParseArctype(x, archtype));
        return commandWrapper;
    }

    private static void ParseArctype<T>(T commandBase, string archtype)
        where T : CommandBase
    {
        //TODO: Parse arc-type and build up item
    }
}

//Ideally these classes would be generated, but given they are only putting on the interface markers
//Could consider having generator take in MSBuild properties to customize interfaces on generated classes.
partial class DotnetCommandWrapper : ICustomizable<DotnetCommandWrapper>
{ }

partial class AddCommandWrapper : ICustomizable<AddCommandWrapper>
{ }
