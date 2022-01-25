# Notes on the prototypes in this directory

## Fundamentals

* We need an opinion on whether the function is a "normal" function or a "shim" function. 
   * Should its XML Comments be for devs of the function, or the CLI description
   * Can it have attributes?

* There is a discoverable way to do everything, even if it is not best
   * Description is at the center of this as putting descriptions in your code really makes it hard to read
   * Method arguments and properties are the easiest to discover, but how would we even do parameters this way.
	  * The user could get the created System.CommandLine object and add a description
	    * I can't figure out how to return a strong type from the AddCommand methods, unless we rely on real time generation, which I am hesitant to do
	    * Analyzer?
	* Similar to the above, we would like friendly validation for common things, so how do we do that?

* We have a assumption in binding that I want to challenge
   * If the user wants to specify something as required, we tell them to use a nullable
   * Nullable has the assumption of a tri state, effectively, the user requests a nullable, but gets back a never null for value types
   * That seems wrong, although I do not have a better idea. Have we ever seent a comment on that?


## Open questions: 
* How do we indicate that AddCommand is not itself called? Is an empty method OK? Or is there some runtimething to do

## Concerns

### Commands

The user has the option to set everything on the SCL Command object. It may not be great stylistic syntax in the code.

|-----------------------------|--------------------------|------------------------|---------------------------------------|
|Concern                      | Most logical locations   | Alternate locations    | Notes                                 |
|-----------------------------|--------------------------|------------------------|---------------------------------------|
| Name(non-default)           | archetype, attribute     |                        |                                       |
| Description                 | XML comments, dictionary |                        |                                       |
| Alias                       | archetype, attribute     |                        |                                       |
| Hidden                      | [SCL only]               |                        |                                       |
| TreatUnmatchedTokensAsError | [SCL only]               |                        |                                       |
| Validators                  | _see validation_         |                        |                                       |

Do we make everything available via XML Comments with a convention such as '<param name=whatever>AlternateName|Description', etc?

### Options


|-----------------------------|--------------------------|------------------------|---------------------------------------|
|Concern                      | Most logical locations   | Alternate locations    | Notes                                 |
|-----------------------------|--------------------------|------------------------|---------------------------------------|
| Name(non-default)           | archetype, attribute     |                        |                                       |
| Alias                       | archetype, attribute     |                        |                                       |
| Hidden                      | [SCL only]               |                        |                                       |
| ValueType                   | param type               |                        | Must be set at generation, not in SCL |
| ArgumentHelpName            | archetype, attribute     |                        |                                       |
| Arity                       | [SCL only]               |                        | Normallyon param being IEnumerable<>  |
| DefaultValue                |                          |                        |                                       |
| DefaultValueFactory         | [SCL only]               |                        |                                       |
| AllowMultipleArgsPerToken   | ?                        |                        |                                       |
| Required                    | [SCL only]               |                        | Normally based on param nullability   |
| Validators                  | _see validation_         |                        |                                       |

Do we need alternate parameter types, or can we assume base classes and interfaces won't be used or will work?

From the code, it looks like you cannot set the ValueType. Is this correct? (at least without knowing to cast to IOption, 
get the IArgument and cast back to Argument). You can set the Argument ValueType, so seems inconsistent.

### Arguments


|-----------------------------|--------------------------|------------------------|---------------------------------------|
|Concern                      | Most logical locations   | Alternate locations    | Notes                                 |
|-----------------------------|--------------------------|------------------------|---------------------------------------|
| Name(non-default)           | archetype, attribute     |                        |                                       |
| Hidden                      | [SCL only]               |                        |                                       |
| ValueType                   |                          |                        | _See question in option_              |
| Arity                       | [SCL only]               |                        | Normallyon param being IEnumerable<>  |
| DefaultValue                |                          |                        |                                       |
| DefaultValueFactory         | [SCL only]               |                        |                                       |
| Required                    | [SCL only]               |                        | Normally based on param nullability   |
| Validators                  | _see validation_         |                        |                                       |

Do we need alternate parameter types, or can we assume base classes and interfaces won't be used or will work?

I do not see IsRequired on arguments

## Validation

TBD