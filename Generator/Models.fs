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
      MemberId: string

      /// The name of the type, as a string. We may need work here for 
      /// nullable, etc.
      TypeName: string

      /// Member usage is how the member appears during application running.
      /// The only currently supported usage is parameter. 
      MemberUsage: MemberUsage

      /// Indicates whether an option or argument should be generated. 
      /// The core SetHandler/symbol AppModel uses symbols as inputs,
      /// so does not need to generate. There are probably also scenarios
      /// where an AppModel allows users to create some symbols so they 
      /// have the full System.CommandLine power, although these AppModels
      /// are not yet designed. If the MemberKind is Service, a symbol
      ///  is never generated, so GenerateSymbol is ignored. 
      GenerateSymbol: bool

      /// Indicates whether the member is an argument, option (also called
      /// a switch) or a service. This field should not be used during 
      /// generation. Instead use the UseMemberKind property which supplies
      /// the default if MemberKind is not set. The default is Option.
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
      CommandId: string

      /// Used by the generator to determine whether to output SetHandler.
      /// When GenerateSetHandler is set to false, any tree structure is unused
      /// and it will generally be fine to include a flat list. 
      GenerateSetHandler: bool

      /// Path is used by transformers to find information in dicationaries. Paths 
      /// should be unique and logical for the user building a lookup. 
      Path: string list

      /// Commands can have aliases, often because there are deprecated versions. 
      /// We have not yet added deprecation for this.  Generally set via a transformer.
      Aliases: string list

      /// The description set for help. Generally set via a transformer.
      Description: string option

      /// Members include options, arguments, and services. Services is anything
      /// not entered by the user, and is generally a well known service from 
      /// System.CommandLine.  Generally set via a transformer.
      Members: MemberDef list

      /// All commands need to be in either the flat list or in a tree based on 
      /// subcommands. They should not be in both. AppModels, other than the 
      /// core SetHandler/symbol generator will probably use a tree structure. 
      /// If used, SubCommands are set during structure setup.
      SubCommands: CommandDef list

      /// Pocket is a property bag for the AppModel use. During structural 
      /// setup there is generally additional information discovered that 
      /// is needed by later transformers. Put that data in the pocket.
      Pocket: (string * obj) list}

    /// CommandDef's should generally be created via the Create method
    /// to increase resiliency to additional fields. In general, a near
    /// empty CommandDef is created during structure evaluation and then 
    /// transformers fill in the details. 
    static member Create commandId =
        { CommandId = commandId
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
      


