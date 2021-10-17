# JackFruit - a simplified console app prototype

I know, I know. You thought console apps were already simple. And they often are. But, as you add command line parsing, and DI, and result handling, and console display, well it the things that should be simple can get pretty complicated.

## Internal design

### Notes

This prototype is modeled for handlers that are methods. In a simple sense, all handler methods are commands and all parameters are options, arguments or services. To model this distinction, the word "member" is used for command children that are not subcommands. 

Services are treated as first class. They are treated as another member, like arguments and options. They just do not appear in the System.CommandLine command tree.

One step in creating a CLI is establishing the relationship between commands. This is done first to build a *path* that is part of the id's used by providers.  

The parameters, and their order, are important in the generated code, so the model centers on this. The first step of creating a CommandDef is to build its children from the parameters. All parameters have the same properties. This allows late differentiation of arguments and services from options. It also means some data is thrown away.

The parameters set the id and typename, and this can never be changed. These are simple strings, and all other member data items are options. 

Once the parameters are established, a series of providers are called to enrich the MemberDef. Each provider receives the command path and the current state of the MemberDef. It does something to create a set of enhancements to the member in the form of a ew member with None for any data that is not changed. This approach allows the changes to be separately zipped to isolate the overriding logic and potentially offer tracking of how changes evolved. 

The initial overriding logic will be first one wins without error for all data. The options are first wins, last wins, or error if data is set twice. There are arguments for the appropriateness of each, so this logic is clearly isolated for later review. This may be a non-issue if people are able to arrange the order of their providers.

In this model *the current approach of manually creating System.CommandLine member symbols is managed as a provider*. It is a special case where the System.CommandLine member symbols are not generated. This model is to support two future scenarios: a) allow Providers to enhance models, such as adding descriptions and b) a mixed mode is possible, although it may not be useful or realistic. The major challenge is figuring out how to modify the existing symbols. Maybe a "SetHandlerAndEnhance" method. If feels sneaky to do it in the SetHandler, and we do not have another hook. 

There are at least two scenarios where data in the method and parameters are themselves accessed via providers: a) attributes and b) XmlComments. Providing this via providers allows better control of precedence. 