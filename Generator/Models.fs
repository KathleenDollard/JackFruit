module Generator.Models

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
type MemberUsage =

    /// The data entered or defaulted by parsing will be used 
    /// as an argument to the invocation method. 
    | Parameter

    /// This is not yet supported
    | Property of Name: string

    /// This is not yet supported
    | StaticProperty of Name: string

    /// This is not yet supported
    | ConstructorParameter of Name: string


/// The main structure for command members during transformation.
/// The single structure is used so that late transformers can
/// determine the member type (as opposed to requiring it during
/// structure evaluation). 
type MemberDef =
    { 
      /// The id for the member, which must be unique within the command.
      /// Note that this is not the name the user sees, but is generally 
      /// the usage name, such as the parameter name.
      ///
      /// Never changed by transformers.
      MemberId: string

      /// The name of the type, as a string. We may need work here for 
      /// nullable, etc.
      ///
      /// Never changed by transformers.
      TypeName: string

      /// Member usage is how the member appears during application running.
      /// The only currently supported usage is parameter. 
      ///
      /// Never changed by transformers.
      MemberUsage: MemberUsage

      /// Indicates whether an option or argument should be generated. 
      /// The core SetHandler/symbol AppModel uses symbols as inputs,
      /// so does not need to generate. There are probably also scenarios
      /// where an AppModel allows users to create some symbols so they 
      /// have the full System.CommandLine power, although these AppModels
      /// are not yet designed. If the MemberKind is Service, a symbol
      ///  is never generated, so GenerateSymbol is ignored. 
      ///
      /// Set by AppModel. Never changed by transformers.
      GenerateSymbol: bool

      /// Indicates whether the member is an argument, option (also called
      /// a switch) or a service. This field should not be used during 
      /// generation. Instead use the UseMemberKind property which supplies
      /// the default if MemberKind is not set. The default is Option.
      ///
      /// This is always sets by transformers. 
      MemberKind: MemberKind option

      /// Aliases for the member. Arguments do not have aliases, so only
      /// the first value is ever used. Ignored for services. 
      Aliases: string list

      /// ArgDisplayName manages the case where the argument of an option
      /// has a different name than the option itself. If both the ArgDisplayName
      /// and an alias is set for an Argument, the ArgDisplayName is used. 
      /// Ignored for services.
      ArgDisplayName: string option

      /// The description displayed for help. Ignored for services.
      Description: string option

      /// Required is almost always determined by the type. This should
      /// only be used if the type does not correctly indicate required 
      /// status
      RequiredOverride: bool option 

      /// Pocket is a property bag for the AppModel use. During structural 
      /// setup there is generally additional information discovered that 
      /// is needed by later transformers. Put that data in the pocket.
      Pocket: (string * obj) list }

      /// MemberDef's should generally be created via the Create method
      /// to increase MemberDef to additional fields. In general, a near
      /// empty CommandDef is created during structure evaluation and then 
      /// transformers fill in the details.     
      static member Create memberId typeName =
        { MemberId = memberId
          TypeName = typeName
          MemberUsage = Parameter
          GenerateSymbol = true
          MemberKind = None
          Aliases = []
          ArgDisplayName = None
          Description = None
          RequiredOverride = None
          Pocket = [] }

    /// Used during generation to retrieve the value set or the default of 
    /// option.
    member this.UseMemberKind =
        match this.MemberKind with
        | Some k -> k
        | None -> MemberKind.Option

    // KAD-Don: Remind me the downside of using "this"
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


/// The main structure for commands during transformations
type CommandDef =
    { /// The id for the command, which must be unique within the context
      /// such as a flat list or the parent command. Note that this is not 
      /// the command name, although it is one of the ways the Name function 
      /// may determine the name. It is generally the method name used for invocation.
      ///
      /// This is set to the method name by the generator during structural eval. 
      /// It can be overridden by the AppModel. This cannot be changed by transformers.
      CommandId: string

      /// The return type of the method. System.CommandLine encourages the use of
      /// the environment return, and thus this is often unit (null)
      ///
      /// This always comes from the method and cannot be changed by transformers
      ReturnType: string option

      /// Used by the generator to determine whether to output SetHandler.
      /// When GenerateSetHandler is set to false, any tree structure is unused
      /// and it will generally be fine to include a flat list. 
      ///
      /// This always comes from the AppModel during structural eval and cannot
      /// be changed by transformers.
      GenerateSetHandler: bool

      /// Path is used by transformers to find information in dictionaries. Paths 
      /// should be unique and logical for the user building a lookup. 
      ///
      /// This defaults to the method name if not set by the AppModel. For single 
      /// layer CLIs, the method name is fine, if it is known by the end user, such as 
      /// when it is also the Name/main alias. This should be set by the end of 
      /// structural eval and cannot be changed by transformers
      Path: string list

      /// Commands can have aliases, often because there are deprecated versions. 
      /// (We have not yet added deprecation to System.CommandLine).  
      ///
      /// This is always set by the AppModel structural eval or transforms. Generally 
      /// set via transformers.
      Aliases: string list

      /// The description set for help. 
      ///
      /// This is almost always set by an AppModel transformer.
      Description: string option

      /// Members include options, arguments, and services. Services is anything
      /// not entered by the user, and is generally a well known service from 
      /// System.CommandLine.  
      ///
      /// This always comes from the method during structure eval and can't be changed 
      /// by transformers. 
      Members: MemberDef list

      /// All commands need to be in either the flat list or in a tree based on 
      /// subcommands. They should not be in both. AppModels, other than the 
      /// core SetHandler/symbol generator will probably use a tree structure. 
      /// If used, SubCommands are set during structure setup.
      ///
      /// This always comes from the AppModel during structural eval and cannot 
      /// be changed by tranformers.
      SubCommands: CommandDef list

      /// Pocket is a property bag for the AppModel use. During structural 
      /// setup there is generally additional information discovered that 
      /// is needed by later transformers. Put that data in the pocket.
      ///
      /// Things are added to the Pocket by both the AppModel (such as an archetype) 
      /// and by the generator (the MethodSymbol and the SemanticModel). Ideally
      /// the order of transformer evaluation is strictly for precedence, and thus
      /// it is not ideal for transformers to use the pocket to communicate because it
      /// sets a transformer dependency order. But if transformers need to communicate
      /// something that can't be set during structural eval, then OK. But don't take
      /// anything out of the pocket ;-)
      Pocket: (string * obj) list}

    /// CommandDef's should generally be created via the Create method
    /// to increase resiliency to additional fields. In general, a near
    /// empty CommandDef is created during structure evaluation and then 
    /// transformers fill in the details. 
    static member Create commandId =
        { CommandId = commandId
          ReturnType = None
          GenerateSetHandler = true
          Path = []
          Description = None
          Aliases = []
          Members = []
          SubCommands = [] 
          Pocket = [] }

    /// Roots only differ by the empty Id. This clarifies that point. 
    /// Roots are only used when SubCommand trees are used, and are 
    /// optional in that case. They are required if there are members
    /// on the root, but that may not be supported in V1.
    static member CreateRoot = CommandDef.Create ""

    /// Name is the name the user expects to see, in Pascal or Camel form. 
    member this.Name =
        if this.Aliases.IsEmpty then
            this.CommandId
        else
            this.Aliases.Head
      


