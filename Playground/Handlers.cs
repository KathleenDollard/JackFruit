using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using System;

namespace TestCode
{
    public static class Handlers
    {
        public static void A(string one, int two) { }
        public static void B(string packageName) { }
        //public static void C(string one, IConsole console, int two) { }

        public class WrappThingee_A
        {
            private readonly Action<string, int> method;

            public WrappThingee_A(Action<string, int> method)
            {
                this.method = method;
            }
            // Make sure there is not more than one instance, so make not-auto properties
            public Option<string> option_One => new Option<string>("one");
            public Option<int> option_Two => new Option<int>("two");

            public Command Command
            {
                get
                {
                    var command =
                        new Command("A")
                        {
                            option_One,
                            option_Two
                        };
                    var command2 = new Command("A");
                    command2.Add(option_One);
                    command2.Add(option_Two);
                    command.Handler = new GeneratedHandler_1(method, option_One, option_Two);
                    return command;
                }
            }
        }

        //private class GeneratedHandler_1 : System.CommandLine.Invocation.ICommandHandler
        //{
        //    public GeneratedHandler_1(
        //        System.Action<string, int> method,
        //        global::System.CommandLine.Option<string> param1,
        //        global::System.CommandLine.Option<int> param2)
        //    {
        //        Method = method;
        //        Param1 = param1;
        //        Param2 = param2;
        //    }

        //    public System.Action<string, System.CommandLine.IConsole, int> Method { get; }

        //    private global::System.CommandLine.Argument<string> Param1 { get; }
        //    private global::System.CommandLine.Option<int> Param2 { get; }
        //    public async Task<int> InvokeAsync(InvocationContext context)
        //    {

        //        Method.Invoke(context.ParseResult.GetValueForArgument(Param1), context.Console, context.ParseResult.GetValueForOption(Param2));

        //        return await Task.FromResult(context.ExitCode);
        //    }
        //}
    }
}

//namespace System.CommandLine
//{
//    [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
//    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
//    internal static class GeneratedCommandHandlers
//    {

//        public static void SetHandler<T1>(
//            this Command command,
//            global::System.Action<string, global::System.CommandLine.IConsole, int> method,
//            global::System.CommandLine.Argument<string> param1,
//            global::System.CommandLine.Option<int> param2)

//        {
//            command.Handler = new GeneratedHandler_1(method, param1, param2);
//        }

//        private class GeneratedHandler_1 : System.CommandLine.Invocation.ICommandHandler
//        {
//            public GeneratedHandler_1(
//                System.Action<string, System.CommandLine.IConsole, int> method,
//                global::System.CommandLine.Argument<string> param1,
//                global::System.CommandLine.Option<int> param2)
//            {
//                Method = method;
//                Param1 = param1;
//                Param2 = param2;
//            }

//            public System.Action<string, System.CommandLine.IConsole, int> Method { get; }

//            private global::System.CommandLine.Argument<string> Param1 { get; }
//            private global::System.CommandLine.Option<int> Param2 { get; }
//            public async Task<int> InvokeAsync(InvocationContext context)
//            {

//                Method.Invoke(context.ParseResult.GetValueForArgument(Param1), context.Console, context.ParseResult.GetValueForOption(Param2));

//                return await Task.FromResult(context.ExitCode);
//            }
//        }
//        public static void SetHandler<T1>(
//            this Command command,
//            global::System.Action<global::System.CommandLine.Generator.Tests.CommandHandlerTests.Character, global::System.CommandLine.IConsole> method,
//            global::System.CommandLine.Option<string> param1,
//            global::System.CommandLine.Option<int> param2)

//        {
//            command.Handler = new GeneratedHandler_2(method, param1, param2);
//        }

//        private class GeneratedHandler_2 : System.CommandLine.Invocation.ICommandHandler
//        {
//            public GeneratedHandler_2(
//                System.Action<System.CommandLine.Generator.Tests.CommandHandlerTests.Character, System.CommandLine.IConsole> method,
//                global::System.CommandLine.Option<string> param1,
//                global::System.CommandLine.Option<int> param2)
//            {
//                Method = method;
//                Param1 = param1;
//                Param2 = param2;
//            }

//            public System.Action<System.CommandLine.Generator.Tests.CommandHandlerTests.Character, System.CommandLine.IConsole> Method { get; }

//            private global::System.CommandLine.Option<string> Param1 { get; }
//            private global::System.CommandLine.Option<int> Param2 { get; }
//            public async Task<int> InvokeAsync(InvocationContext context)
//            {

//                var model = new global::System.CommandLine.Generator.Tests.CommandHandlerTests.Character(context.ParseResult.GetValueForOption(Param1), context.ParseResult.GetValueForOption(Param2));

//                Method.Invoke(model, context.Console);
//                return await Task.FromResult(context.ExitCode);
//            }
//        }
//        public static void SetHandler<T1>(
//            this Command command,
//            global::System.Func<int, int, int> method,
//            global::System.CommandLine.Argument<int> param1,
//            global::System.CommandLine.Argument<int> param2)

//        {
//            command.Handler = new GeneratedHandler_3(method, param1, param2);
//        }

//        private class GeneratedHandler_3 : System.CommandLine.Invocation.ICommandHandler
//        {
//            public GeneratedHandler_3(
//                System.Func<int, int, int> method,
//                global::System.CommandLine.Argument<int> param1,
//                global::System.CommandLine.Argument<int> param2)
//            {
//                Method = method;
//                Param1 = param1;
//                Param2 = param2;
//            }

//            public System.Func<int, int, int> Method { get; }

//            private global::System.CommandLine.Argument<int> Param1 { get; }
//            private global::System.CommandLine.Argument<int> Param2 { get; }
//            public async Task<int> InvokeAsync(InvocationContext context)
//            {

//                var rv =
//                Method.Invoke(context.ParseResult.GetValueForArgument(Param1), context.ParseResult.GetValueForArgument(Param2));

//                return await Task.FromResult(rv);
//            }
//        }
//        public static void SetHandler<T1>(
//            this Command command,
//            global::System.Action<global::System.CommandLine.Invocation.InvocationContext, global::System.CommandLine.IConsole, global::System.CommandLine.Parsing.ParseResult, global::System.CommandLine.Help.IHelpBuilder, global::System.CommandLine.Binding.BindingContext> method)
//        {
//            command.Handler = new GeneratedHandler_4(method);
//        }

//        private class GeneratedHandler_4 : System.CommandLine.Invocation.ICommandHandler
//        {
//            public GeneratedHandler_4(
//                System.Action<System.CommandLine.Invocation.InvocationContext, System.CommandLine.IConsole, System.CommandLine.Parsing.ParseResult, System.CommandLine.Help.IHelpBuilder, System.CommandLine.Binding.BindingContext> method)
//            {
//                Method = method;
//            }

//            public System.Action<System.CommandLine.Invocation.InvocationContext, System.CommandLine.IConsole, System.CommandLine.Parsing.ParseResult, System.CommandLine.Help.IHelpBuilder, System.CommandLine.Binding.BindingContext> Method { get; }

//            public async Task<int> InvokeAsync(InvocationContext context)
//            {

//                Method.Invoke(context, context.Console, context.ParseResult, context.HelpBuilder, context.BindingContext);

//                return await Task.FromResult(context.ExitCode);
//            }
//        }
//        public static void SetHandler<T1>(
//            this Command command,
//            global::System.Func<string, global::System.CommandLine.IConsole, int, global::System.Threading.Tasks.Task> method,
//            global::System.CommandLine.Argument<string> param1,
//            global::System.CommandLine.Option<int> param2)

//        {
//            command.Handler = new GeneratedHandler_5(method, param1, param2);
//        }

//        private class GeneratedHandler_5 : System.CommandLine.Invocation.ICommandHandler
//        {
//            public GeneratedHandler_5(
//                System.Func<string, System.CommandLine.IConsole, int, System.Threading.Tasks.Task> method,
//                global::System.CommandLine.Argument<string> param1,
//                global::System.CommandLine.Option<int> param2)
//            {
//                Method = method;
//                Param1 = param1;
//                Param2 = param2;
//            }

//            public System.Func<string, System.CommandLine.IConsole, int, System.Threading.Tasks.Task> Method { get; }

//            private global::System.CommandLine.Argument<string> Param1 { get; }
//            private global::System.CommandLine.Option<int> Param2 { get; }
//            public async Task<int> InvokeAsync(InvocationContext context)
//            {

//                var rv =
//                Method.Invoke(context.ParseResult.GetValueForArgument(Param1), context.Console, context.ParseResult.GetValueForOption(Param2));

//                await rv;
//                return context.ExitCode;
//            }
//        }
//        public static void SetHandler<T1>(
//            this Command command,
//            global::System.Func<int, int, global::System.Threading.Tasks.Task<int>> method,
//            global::System.CommandLine.Argument<int> param1,
//            global::System.CommandLine.Argument<int> param2)

//        {
//            command.Handler = new GeneratedHandler_6(method, param1, param2);
//        }

//        private class GeneratedHandler_6 : System.CommandLine.Invocation.ICommandHandler
//        {
//            public GeneratedHandler_6(
//                System.Func<int, int, System.Threading.Tasks.Task<int>> method,
//                global::System.CommandLine.Argument<int> param1,
//                global::System.CommandLine.Argument<int> param2)
//            {
//                Method = method;
//                Param1 = param1;
//                Param2 = param2;
//            }

//            public System.Func<int, int, System.Threading.Tasks.Task<int>> Method { get; }

//            private global::System.CommandLine.Argument<int> Param1 { get; }
//            private global::System.CommandLine.Argument<int> Param2 { get; }
//            public async Task<int> InvokeAsync(InvocationContext context)
//            {

//                var rv =
//                Method.Invoke(context.ParseResult.GetValueForArgument(Param1), context.ParseResult.GetValueForArgument(Param2));

//                return await rv;
//            }
//        }
//        public static void SetHandler<T1>(
//            this Command command,
//            global::System.Action<string> method,
//            global::System.CommandLine.Argument<string> param1)

//        {
//            command.Handler = new GeneratedHandler_7(method, param1);
//        }

//        private class GeneratedHandler_7 : System.CommandLine.Invocation.ICommandHandler
//        {
//            public GeneratedHandler_7(
//                System.Action<string> method,
//                global::System.CommandLine.Argument<string> param1)
//            {
//                Method = method;
//                Param1 = param1;
//            }

//            public System.Action<string> Method { get; }

//            private global::System.CommandLine.Argument<string> Param1 { get; }
//            public async Task<int> InvokeAsync(InvocationContext context)
//            {

//                Method.Invoke(context.ParseResult.GetValueForArgument(Param1));

//                return await Task.FromResult(context.ExitCode);
//            }
//        }
//    }
