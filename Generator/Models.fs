module Generator.Models

open Microsoft.CodeAnalysis
open System.CommandLine
open System.Collections.Generic
open Generator.GeneralUtils 
open Common
open Generator.LanguageModel


type ItemReturn<'T> =
| NewValue of Value: 'T
| UsePreviousValue


//type CommandReturnType =
//    | Void
//    | CommandReturnType of t: NamedItem
//    static member Create typeName =
//        match typeName with 
//            | "void" -> Void
//            | _ -> CommandReturnType (NamedItem.Create typeName)


/// MemberKind indicates the System.CommandLine symbol used
/// for the member. They are treated the same during transformation
/// so that a late transformer can determine which they are. 
/// This means some data is ignored. 
type MemberKind =

    /// The user should see a System.CommandLine Option 
    | Option

    /// The user should see a System.CommandLine Argument 
    | Argument

    /// The user should see no System.CommandLine feature, but 
    /// the parser will determine a value. 
    | Service


/// MemberUsage indicates to generating code what to do with the 
/// value retrieved in parsing. Only parameters are currently 
/// supported. 
type MemberDefUsage =

    /// The data entered or defaulted by parsing will be used 
    /// as an argument to the invocation method. 
    | UserParameter of Parameter: IParameterSymbol
    | HandlerParameter of Parameter: IParameterSymbol * Symbol: Symbol

    /// This is not yet supported
    | Property of Property: IPropertySymbol

    /// This is not yet supported
    | StaticProperty of Property: IPropertySymbol

    /// This is not yet supported
    | ConstructorParameter of Parameter: IParameterSymbol

    | ArbitraryMember

type CommandDefUsage =
    | UserMethod of Method: IMethodSymbol * SemanticModel: SemanticModel
    // Set Handler is just to support the work Kevin Bost did in 2021. Might be deleted in future
    //| SetHandlerMethod of Method: IMethodSymbol * SemanticModel: SemanticModel * Symbol: Symbol
    | Arbitrary of Name: string // might just be used in testing

/// The main structure for command members during transformation.
/// The single structure is used so that late transformers can
/// determine the member type (as opposed to requiring it during
/// structure evaluation). 
type MemberDef(memberId: string, commandDef: CommandDef, typeName: NamedItem, memberDefUsage: MemberDefUsage, generateSymbol: bool) =
    let pocket = Dictionary<string, obj>()

    /// Indicates whether the member is an argument, option (also called
    /// a switch) or a service. This field should not be used during 
    /// generation. Instead use the UseMemberKind property which supplies
    /// the default if MemberKind is not set. The default is Option.
    ///
    /// This is always sets by transformers. 
    member val MemberKind: MemberKind option = None
        with get, set
    member this.Kind =
        match this.MemberKind with
        | Some value -> value
        | None -> Option   
    member this.KindName  =
        match this.Kind  with 
        | Argument -> "Argument" 
        | Option -> "Option" 
        | Service -> ""       
    //static member Create()



    /// ArgDisplayName manages the case where the argument of an option
    /// has a different name than the option itself. If both the ArgDisplayName
    /// and an alias is set for an Argument, the ArgDisplayName is used. 
    /// Ignored for services.
    ///
    /// Often not set, but if it is set, it is always sets by transformers. 
    member val ArgDisplayName: string option = None
         with get, set

    /// Aliases for the member. Arguments do not have aliases, so only
    /// the first value is ever used. Ignored for services. 
    ///
    /// Often not set, but if it is set, it is always sets by transformers.  
    member val Aliases: string list = []
        with get, set
    
    /// The description displayed for help. Ignored for services.
    ///
    /// This is always sets by transformers. 
    member val Description: string option = None
         with get, set

    /// Required is almost always determined by the type. This should
    /// only be used if the type does not correctly indicate required 
    /// status
    ///
    /// Rarely should be set, but if it is set, it is always sets by transformers.  
    member val RequiredOverride: bool option = None
         with get, set

    /// Pocket is a property bag for the AppModel use. During structural 
    /// setup there is generally additional information discovered that 
    /// is needed by later transformers. Put that data in the pocket.
    ///
    /// This is manipulated by both the AppModel and the transformers
    member _.Pocket
        with get(key) = 
            match pocket.TryGetValue key with
            | true, value -> Some value
            | _           -> None

    member _.AddToPocket key value =
        pocket.Add(key, value)

    /// The id for the member, which must be unique within the command.
      /// Note that this is not the name the user sees, but is generally 
      /// the usage name, such as the parameter name.
      ///
      /// Never changed by transformers.
    member _.MemberId = memberId

    member _.CommandDef = commandDef

    /// The name of the type, as a string. We may need work here for 
      /// nullable, etc.
      ///
      /// Never changed by transformers.
    member _.TypeName = typeName

    /// Member usage is how the member appears during application running.
      /// The only currently supported usage is parameter. 
      ///
      /// Never changed by transformers.
    member _.MemberDefUsage = memberDefUsage

    /// Indicates whether an option or argument should be generated. 
    /// The core SetHandler/symbol AppModel uses symbols as inputs,
    /// so does not need to generate. There are probably also scenarios
    /// where an AppModel allows users to create some symbols so they 
    /// have the MemberDefUsage.CommandLine power, although these AppModels
    /// are not yet designed. If the MemberKind is Service, a symbol
    ///  is never generated, so GenerateSymbol is ignored. 
    ///
    /// Set by AppModel. Never changed by transformers.
    member _.GenerateSymbol = generateSymbol

    /// Used during generation to retrieve the value set or the default of 
    /// option.
    member this.UseMemberKind =
        match this.MemberKind with
        | Some k -> k
        | None -> MemberKind.Option

    /// The calculated name of the member in the style (usually Pascal
    /// or Camel) set. Use utility methods for snake and kebab case.
    member this.Name =
        let innerName = 
            if this.Aliases.IsEmpty then
                this.MemberId
            else
                this.Aliases.Head
        match this.UseMemberKind with
        | Argument -> 
            match this.ArgDisplayName with 
            | Some n -> n
            | None -> innerName
        | _ -> innerName

    /// Returns the string used by the user.
    /// <br/>
    /// This introduces two order dependencies in AppModels that is handled by the default 
    /// description lookup but might need to be considered by other lookups. If this returns
    /// None, all member kinds should be tried. The other is harder. Name is used, and name is 
    /// dependent on Aliases, MemberKind and ArgDisplayName. I do not see a way around this
    /// as it would seem illogical to a user for the dictionary to be on the ID instead of 
    /// the name. We could make it an error to not have a MemberKind when this is called.
    member this.Syntax =
        match this.MemberKind with 
        | None -> $"*{this.Name}*"
        | Some kind -> 
            match kind with 
            | Option -> $"--{this.Name}"
            | Argument -> $"<{this.Name}>"
            | Service -> $"{{{this.Name}}}"


/// The main structure for commands during transformations
and CommandDef(commandId: string, path: string list, returnType: ReturnType, commandDefUsage: CommandDefUsage) =

    let pocket = Dictionary<string, obj>()

    /// The description set for help. 
    ///
    /// This is almost always set by an AppModel transformer.
    member val Description: string option = None
         with get, set

    /// Commands can have aliases, often because there are deprecated versions. 
    /// (We have not yet added deprecation to System.CommandLine).  
    ///
    /// This is always set by the AppModel structural eval or transforms. Generally 
    /// set via transformers.
    member val Aliases: string list = []
         with get, set

    /// Members include options, arguments, and services. Services is anything
    /// not entered by the user, and is generally a well known service from 
    /// System.CommandLine.  
    ///
    /// This always comes from the method during structure eval and can't be changed 
    /// by transformers.
    member val Members: MemberDef list = []
        with get, set
        // TODO: Remove the ability to set this and SubCommands at any time.
    member this.OptionsAndArgs =
        [ for memberDef in this.Members do
            match memberDef.Kind with 
            | Option | Argument -> memberDef
            | _ -> () ]


    member this.HandlerMethodName : string = 
        match commandDefUsage with 
        | UserMethod (m, _) -> 
            m.ToDisplayString().SubstringBefore("(", m.ToDisplayString())
        | Arbitrary n -> n

    member this.ParameterTypes : NamedItem list =
       match commandDefUsage with 
        | UserMethod (m, _) -> 
            [ for p in m.Parameters do 
                NamedItem.Create (p.Type.ToDisplayString()) ]
        | Arbitrary n -> []
        

    /// All commands need to be in either the flat list or in a tree based on 
    /// subcommands. They should not be in both. AppModels, other than the 
    /// core SetHandler/symbol generator will probably use a tree structure. 
    /// If used, SubCommands are set during structure setup.
    ///
    /// This always comes from the AppModel during structural eval and cannot 
    /// be changed by tranformers.
    member val SubCommands: CommandDef list = []
        with get, set

    /// Pocket is a property bag for the AppModel use. During structural 
      /// setup there is generally additional information discovered that 
      /// is needed by later transformers. Put that data in the pocket.
      ///
      /// Things are added to the Pocket by both the AppModel (such as details of the input) 
      /// and by the generator (the MethodSymbol and the SemanticModel). Ideally
      /// the order of transformer evaluation is strictly for precedence, and thus
      /// it is not ideal for transformers to use the pocket to communicate because it
      /// sets a transformer dependency order. But if transformers need to communicate
      /// something that can't be set during structural eval, then OK. But don't steal
      /// anything out of the pocket ;-)
      ///
      /// Open question: Pocket is currently a list of tuples. This seems kind of a half way
      /// thing. Perhaps, either use a map, or use an object list, and allow the value
      /// to often be retrieved via the type. And if we use a map, should it be the current
      /// random string map, or a map based on a DU for expected items. ** Update: This open
      /// question is somewhat less important with the redesign that made expected things 
      /// part of the CommandDef and MemberDef rather than being in the pocket. 
    member _.Pocket
        with get(key) = 
            match pocket.TryGetValue key with
            | true, value -> Some value
            | _           -> None

    member _.AddToPocket key value =
        pocket.Add(key, value)
 
    /// The id for the command, which must be unique within the context
    /// such as a flat list or the parent command. Note that this is not 
    /// the command name, although it is one of the ways the Name function 
    /// may determine the name. It is generally the method name used for invocation.
    ///
    /// This is set to the method name by the generator during structural eval. 
    /// It can be overridden by the AppModel. This cannot be changed by transformers.
    member _.CommandId: string = commandId


    /// Path is used by transformers to find information in dictionaries. Paths 
    /// should be unique and logical for the user building a lookup. 
    ///
    /// This defaults to the method name if not set by the AppModel. For single 
    /// layer CLIs, the method name is fine, if it is known by the end user, such as 
    /// when it is also the Name/main alias. This should be set by the end of 
    /// structural eval and cannot be changed by transformers
    member _.Path: string list = path

    /// The return type of the invoked method. System.CommandLine encourages the use of
    /// the environment return, and thus this is often unit (null)
    ///
    /// This always comes from the method and cannot be changed by transformers
    member _.ReturnType: ReturnType = returnType

    /// Used by the generator to determine whether to output SetHandler.
    /// When GenerateSetHandler is set to false, any tree structure is unused
    /// and it will generally be fine to include a flat list. 
    ///
    /// This always comes from the AppModel during structural eval and cannot
    /// be changed by transformers.
    member _.CommandDefUsage: CommandDefUsage = commandDefUsage


    ///// Roots only differ by the empty Id. This clarifies that point. 
    ///// Roots are only used when SubCommand trees are used, and are 
    ///// optional in that case. They are required if there are members
    ///// on the root, but that may not be supported in V1.
    //static member CreateRoot commandDefUsage = CommandDef.Create commandDefUsage ""

    /// Name is the name the user expects to see, in Pascal or Camel form. 
    member this.Name =
        if this.Aliases.IsEmpty then
            this.CommandId
        else
            this.Aliases.Head

    member this.PathString =
        String.concat " " this.Path

      


