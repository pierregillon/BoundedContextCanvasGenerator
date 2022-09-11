using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Domain.BC.Ubiquitous;
using BoundedContextCanvasGenerator.Domain.Configuration;
using BoundedContextCanvasGenerator.Domain.Types;
using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.BC;

public class BoundedContextCanvasAnalyser
{
    public async Task<BoundedContextCanvas> Analyse(TypeDefinitionExtract typeDefinitionExtract, ICanvasSettings canvasSettings) =>
        new(
            canvasSettings.Name,
            canvasSettings.Definition,
            GenerateUbiquitousLanguage(typeDefinitionExtract),
            GenerateInboundCommunication(typeDefinitionExtract, canvasSettings)
        );

    private static UbiquitousLanguage GenerateUbiquitousLanguage(TypeDefinitionExtract typeDefinitionExtract)
        => typeDefinitionExtract
            .Aggregates
            .Values
            .Select(CoreConcept.FromTypeDefinition)
            .ToArray()
            .Pipe(UbiquitousLanguage.FromConcepts);

    private static InboundCommunication GenerateInboundCommunication(TypeDefinitionExtract typeDefinitionExtract, ICanvasSettings canvasSettings)
    {
        var commandsGroupedByModule = typeDefinitionExtract.Commands.Values
            .Select(x => new CommandWrapper(x))
            .GroupBy(x => x.ModuleName)
            .ToArray();

        var modules = GenerateDomainModules(
            commandsGroupedByModule,
            typeDefinitionExtract.DomainEvents.Values,
            canvasSettings.InboundCommunicationSettings
        );

        return new InboundCommunication(modules);
    }

    private static IEnumerable<DomainModule> GenerateDomainModules(
        IEnumerable<IGrouping<Namespace, CommandWrapper>> commandsGroupedByModule,
        IReadOnlyCollection<TypeDefinition> domainEventTypes,
        InboundCommunicationSettings inboundCommunicationSettings
    )
        => commandsGroupedByModule.Select(commandsFromSameModule =>
            new DomainModule(
                commandsFromSameModule.Key.Name.ToReadableSentence(),
                GetDomainFlows(commandsFromSameModule, domainEventTypes, inboundCommunicationSettings)
            )
        );

    private static IEnumerable<DomainFlow> GetDomainFlows(IEnumerable<CommandWrapper> commands, IReadOnlyCollection<TypeDefinition> domainEventTypes, InboundCommunicationSettings inboundCommunicationSettings)
        => commands.Select(command => command.BuildDomainFlow(domainEventTypes, inboundCommunicationSettings));

    private record CommandWrapper(TypeDefinition CommandTypeDefinition)
    {
        private Namespace ParentNamespace { get; } = CommandTypeDefinition.FullName.Namespace;
        public Namespace ModuleName => ParentNamespace.TrimStart(CommandTypeDefinition.AssemblyDefinition.Namespace);

        public DomainFlow BuildDomainFlow(IEnumerable<TypeDefinition> domainEventTypes, InboundCommunicationSettings inboundCommunicationSettings)
            => new(
                GetCollaborators(inboundCommunicationSettings.CollaboratorDefinitions),
                Command.FromType(CommandTypeDefinition),
                GetPolicies(inboundCommunicationSettings.PolicyDefinitions),
                GetDomainEvents(domainEventTypes)
            );

        private IEnumerable<Collaborator> GetCollaborators(IEnumerable<CollaboratorDefinition> collaboratorDefinitions)
            => CommandTypeDefinition.Instanciators
                .SelectMany(i => i.FilterCollaboratorDefinitionsMatching(collaboratorDefinitions))
                .Select(Collaborator.FromCollaboratorDefinition)
                .ToArray();

        private IEnumerable<Policy> GetPolicies(IEnumerable<PolicyDefinition> policyDefinitions)
            => CommandTypeDefinition.Instanciators
                .SelectMany(i => i.FilterMethodsMatching(policyDefinitions))
                .Select(Policy.FromMethod)
                .ToArray();

        private IEnumerable<DomainEvent> GetDomainEvents(IEnumerable<TypeDefinition> domainEventTypes)
            => domainEventTypes
                .Where(domainEvent => domainEvent.IsInstanciatedBy(CommandTypeDefinition.FullName))
                .Select(DomainEvent.FromType)
                .ToArray();
    }
}