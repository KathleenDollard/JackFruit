# JackFruit

This is a prototype and playground. Please do not take it too seriously, it is not my current intention to maintain it!

## Current requests

The greatest current need is to review the OutputCode draft. This is preliminary and I know there are issues. It would be helpful to see what issues you see. Here are a few of the constraints I was working with: 

* F# is beautiful here because it often does not require parens, making a much more readable DSL.
* There are other approaches, but my first feedback liked the introductory word so I went with it. If you can sketch out something different, within constraints, I'd love to see it.
* F# is strongly typed and inferred and partial. As such, if there is a type named "blah" and a function named "blah" they will collide. To manage this I generally used lower case for the introductory word.
* F# functions do not have overloads. To use overloads you have to use the tupled calling syntax, which I found rather ugly. For example, I had these choices: 
  * `method Public Blah...` and `staticMethod Public Blah...`
  * `method Public Instance Blah...` and `method Public Static Blah...`
  * I went with the first, but it means combinations are difficult:
  * `staticAsyncPartialMethod Public Blah...` with 8 variations or
  * `methodFull Partial Public Static Async Blah...` and `methodFull NotPartial Public Instance NotAsync Blah...`
* I used variables so the way values were created was separated from the shape of the code. Should I have done more or less of this?

I suspect it will initially feel weird that you can't see the C# code you're used to. I'd appreciate you're spending a bit of time with this to see if the important part, the semantics come through. There are three benefits to this approach: 

* You will never have a syntax error in generated code.
* This will output CE and Visual Basic code (see [Wait, C# and VB?](#wait-c-and-vb))
* The semantics of the output will be easier to understand (I hope).

## Overview

This project currently has several purposes - which explains some strange things about it:

* Provide a prototype of a Roslyn source generation solution to the problem of simplifying System.CommandLine
* Demonstrate a Roslyn language (VB/C#) agnostic approach to source generation
* Illustrate the benefit of a DSL driven (as opposed to literal language with holes) definition for generating source code
* Give me personally a playground to learn F# in a bit more depth
* Offer code for comparing F# and C# for generation, if someone rewrites this in C#

These purposes result in a few weird things: 

* F# is used to generate code - a generation strategy based on Roslyn so it is the only .NET language that cannot take part
  * This doesn't bother me because I suspect there is an orthogonal approach to System.CommandLine for F# that makes more sense for F#
* C# and F# projects are mixed in the solution
* It's a mess (remember the prototype/playground statement)
  * Now that it is basically working end to end, I will be cleaning up and refactoring. It may get worse before it gets better
* I do not currently care much about performance. If this prototype grows up, we'll take a pass at that later

It's based on some very strong opinions I have after a few decades in and around the generation space:

Some of this is from my 5 Principles of Code Generation/generative techniques, annotated for this context here: 

* The developer using generation is in control
  * This was partly in response of living out the 4GL debacle of the 1980's. 
  * I've evolved to think the generation designer must be flexible and opinionated about as much of generation as possible, but as we moved away from discrete templates like T4, generative techniques must balance:
    * Opinions and physics (what the using developer cannot change)
    * Extensibility (what the using developer can change with difficulty, likely used by architect types)
    * Customization (what the using developer can easily change, likely used by teams)
    * Generative input (what the user naturally, and generally must provide)
    * If it's possible for the user to just be able to add code that modifies key steps, developers of the generative systems should consider it - devs like to code, but this can make systems exceedingly hard to debug.
* Metadata is (apparent)
  * Generative systems when run have a part that patterns the output (the broadest possible use of the term "template") and input. The input must be easy to work with.
  * This was partly in response to some early database generation that used bizarre files to define simple things like validation
  * The best metadata for most using developers most of the time is code. There is an astounding amount of information that is naturally expressed in code, at least an order of magnitude more than any of the alphabet: XML, JSON, YAML, etc.
* Generation should be repeatable and the simplest way for the developer to get the job done
  * This is in response to systems that provide very little initial benefit because of difficulty in configuration or other issues. No amount of later benefit pays off for up front loss in a developers mind, and probably in reality. Systems should be at least break even up front.
  * This is also in response to systems that provide amazing initial benefit and are extremely difficult to maintain. This has often happened from developers new to creating generative systems enchanted by the power.
  * The "repeatable" (sometimes "re-entrant") part is in response to mixing up of initial project creation from long term evolution of a project. Initial project creation is a learning tool and one time shortcut. No one knows what their project looks like when they create it, ever. The poster child is asking for complex auth information when creating a .NET application. Auth would best be handled by a re-entrant mechanism.
* User code is sacred and must be protected
  * This is partly in response to a world without adequate source control, but even with great source control where you can retrieve lost code, the cost is often high.
  * Some tools now do the most basic thing here, allow files to be marked as generated and don't allow the user to edit them, or don't allow it without sufficient disclosure (editing these files is extremely helpful during debugging)
  * This principle is behind my deep belief that generated and user authored code should always be in different files.
* *Cheapest?*

## Wait, C# and VB?

Wait, what? How can you generate both, since their generators are different and their SyntaxNodes are different. This agnostic Code Model part of the project will be extracted to a separate library. 

This uses (will use) a shim generator for C# and VB which collects the syntax and passes it to a common generator. Where syntax is needed, there is selection processing based on language. This is minimized by moving quickly to the semantic model and using it to the greatest degree possible. 

The code model is least common denominator. You can't use VB XML literals, and you can't use C# pattern matching. Generated code needs to be good, not best possible. Issues may arise for some scenarios, but not the most common generation scenarios.

Expected areas of further work or where this will fall apart:

* `span` - I hope we can find a VB equivalent, but there may need to be a way to split off blocks of code. 
* XML literals
* Null reference types - I think we can just ignore this in VB
* Attributes that are mod-req'd out in VB

## Wait, tell me again why you are using F# for a Roslyn generator?

A major reason is that I could not both learn F# and move this forward, and learning F# was more time critical. But there are other benefits:

* I make extensive use of Discriminated Unions where the corresponding base/derived class code would be messier (such as for Result)
* The ability to write without parens made it easy to look DSL like within the context of complex logic
* Type inference and nested functions made things clean
* The ability to smoothly incorporate data, functions, classes and methods let me use an appropriate structure in the right place

I realize the downside is you needing to understand F#. I also deeply care what you struggle with if you are new to F#, so please ask questions. Issues are fine for that as this is a casual prototype repo. I also deeply care about what I'm learning, so if you already know F#, please (kindly) open issues where you think I could have done something better.

## The Domain: Console apps

I know, I know. You thought console apps were already simple. They only are until you do something interesting. But, as you add command line parsing, and DI, and result handling, and console display, well it the things that should be simple can get pretty complicated.

The problem addressed by the prototype is to easily define a CLI: a set of commands and sub commands each of which have options and arguments.

This is based on [System.CommandLine](https://github.com/dotnet/command-line-api).

## Internal design

Generation is taking input, applying a pattern and getting output. In all but trivial cases, doing this as one step results in code that is very difficult to maintain (maybe add link to David Weigner's video as he agrees with me :-):

* Input
* Model
* Output

Among other things, you can test the creation of your model independently of creation of output. 

**Note on Roslyn Source Generators** Roslyn source generation allows you to not only smush all this together, but then to place it directly into the compiler context. This creates complex and slow testing scenarios. If you are more complex than the [Cookbook samples]() consider extracting your code out of Roslyn (passing in Syntax) as a first step to sanity, even if you hate everything else I say in this section).

The design in this  takes this further with a series of steps with discrete purpose. This results in a multi-step process:

* Create initial model (sets the shape)
* Runs a series of transforms on the model
* Morphs this into a Code Model
* Outputs the Code Model

## Whose in control at each step? 

### Model

The model is set by the system. If you need a different model, then it's a different system that can use some of the parts. Later, there may be support for deriving from CommandDef and MemberDef to provide more properties strictly for transformations and respective Code Model creation. This will be a deep customization that will not be needed unless we switch out System.CommandLine or someone is deriving from System.CommandLine Symbol classes. This is a highly advanced scenario and may never be supported. 

### Initial model creation

The initial model creation is determined by the AppModel and is extensible. I highly desire that people will create additional AppModels. The system is designed to be extensible in this by supplying two things: 

* The children of a CommandDef (sub-commands)
* Key details about a CommandDef or MemberDef (ones that can't be transformed)

If you have a different idea on how to accomplish these things, you can create your won AppModel. I have found the first to be by far the most difficult.

### Transforms

Transforms are smallish methods that take and return a CommandDef or MemberDef and change mutable members. They are extensible by creating new ones, and customizable by defining a set to use for a given generation definition. To support mix and match, and excluding some, they are granular.

Fundamental to the supporting various AppModels is that any AppModel can use any transform. Once the model is created, the AppModel is done. If the AppModel needs to make data available to transforms, there is a property bag called *The Pocket* designed for this purpose. 

Where they are performant, it is sensible to apply all of them, as the using developer will likely write code where only one wins. Sadly, this XML Comment evaluation requires loading an XDocument, and that may be slow when the XML Comment is not used for descriptions. One approach is to have that evaluated last, so if a dictionary based, attribute, or other description is found for the item, the XML Comment is not touched. Another is a single Boolean *Skip XML Comments* that would avoid the using developer having to define a full transform set for this minor issue. 

None of this will be considered until we know the importance of performance. As a Roslyn source generator if we can determine whether the current generation is design time or compile time we may have little concern about transforms as they may not change the shape of the Code Model. For example: the generated code includes a class for each command and subcommand and a named member for each argument or option. These provide easy access to the final System.CommandLine symbols to allow final fixup, which we expect will be used for certain validation and to keep up with future System.CommandLine changes. The fact these classes and properties exist is a design time concern. Their values is a compile time concern. Design time must be fast, compile time can be a bit slower. 

### Morphing to Code Model

This is based on the System.CommandLine as the underlying tool and is not currently expected to be customized. If folks want changes, they will almost certainly be improvements to the generated code and thus a PR into the repo would be preferable.

Scenarios where customization of generation is helpful would be great to hear about!

### Outputting the Code Model

This is based on C# and Visual Basic as the underlying domain and is not currently expected to be customized. If folks want changes, they will almost certainly be improvements to the generated code and thus a PR into the repo would be preferable.

I do not know of any scenarios that would require customization of this layer.

## Creating the initial model 

This is done in a file currently called "NewMapping". The purpose here is to create CommandDefs and MemberDefs. If I change the name of the file to something sane, look for where CommandDefs are created.

The model is essential. It needs to be complete in what can be output as it will hold all information used to generate the Code Model and it needs to define what can only be done during initial creation and what can be changed via a transform. A laissez-faire approach where everything can be transforms requires strict ordering of transforms. If this becomes necessary, consider two groups of transforms, those that affect the structure and those that do not. An ordered set of transforms could be done, but it would make it much more difficult for using developers to pick and choose. *This may evolve.

*I ran out of steam here*