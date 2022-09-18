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
            .Select(command => new CommandWrapper(
                command, 
                typeDefinitionExtract.Handlers.FirstOrDefault(handler => handler.Match(command))?.TypeDefinition,
                typeDefinitionExtract.Handlers.Where(handler => typeDefinitionExtract.DomainEvents.Values.Any(handler.Match)).ToArray())
            )
            .GroupBy(x => x.ModuleName)
            .ToArray();

        var modules = GenerateDomainModules(
            commandsGroupedByModule,
            typeDefinitionExtract.DomainEvents.Values,
            typeDefinitionExtract.IntegrationEvents,
            canvasSettings.InboundCommunicationSettings
        );

        return new InboundCommunication(modules);
    }

    private static IEnumerable<DomainModule> GenerateDomainModules(
        IEnumerable<IGrouping<Namespace, CommandWrapper>> commandsGroupedByModule,
        IReadOnlyCollection<TypeDefinition> domainEventTypes,
        IEnumerable<TypeDefinition> integrationEventTypes,
        InboundCommunicationSettings inboundCommunicationSettings
    )
        => commandsGroupedByModule.Select(commandsFromSameModule =>
            new DomainModule(
                commandsFromSameModule.Key.Name.ToReadableSentence(),
                GetDomainFlows(commandsFromSameModule, domainEventTypes, integrationEventTypes, inboundCommunicationSettings)
            )
        );

    private static IEnumerable<DomainFlow> GetDomainFlows(
        IEnumerable<CommandWrapper> commands, 
        IReadOnlyCollection<TypeDefinition> domainEventTypes,
        IEnumerable<TypeDefinition> integrationEventTypes,
        InboundCommunicationSettings inboundCommunicationSettings
    )
        => commands.Select(command 
            => command.BuildDomainFlow(
                domainEventTypes, 
                integrationEventTypes, 
                inboundCommunicationSettings
            )
        );

    private record CommandWrapper(
        TypeDefinition CommandTypeDefinition, 
        TypeDefinition? CommandHandlerTypeDefinition, 
        IReadOnlyCollection<LinkedTypeDefinition> DomainEventHandlers
    )
    {
        private Namespace ParentNamespace { get; } = CommandTypeDefinition.FullName.Namespace;
        public Namespace ModuleName => ParentNamespace.TrimStart(CommandTypeDefinition.AssemblyDefinition.Namespace);

        public DomainFlow BuildDomainFlow(
            IEnumerable<TypeDefinition> domainEventTypes, 
            IEnumerable<TypeDefinition> integrationEventTypes, 
            InboundCommunicationSettings inboundCommunicationSettings
        )
            => new(
                GetCollaborators(inboundCommunicationSettings.CollaboratorDefinitions),
                Command.FromType(CommandTypeDefinition),
                GetPolicies(inboundCommunicationSettings.PolicyDefinitions),
                GetDomainEvents(domainEventTypes, integrationEventTypes)
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

        private IEnumerable<DomainEvent> GetDomainEvents(IEnumerable<TypeDefinition> domainEventTypes, IEnumerable<TypeDefinition> integrationEventTypes)
        {
            var domainEvents = GetDomainEventInstanciatedByCommand(domainEventTypes)
                .Concat(GetDomainEventInstanciatedByCommandHandler(domainEventTypes))
                .Distinct()
                .ToArray();

            if (!DomainEventHandlers.Any()) {
                return domainEvents;
            }

            foreach (var domainEvent in domainEvents) {
                var domainEventType = domainEventTypes.First(x => x.FullName == domainEvent.TypeFullName);
                var specificDomainEventHandlers = DomainEventHandlers.Where(x => x.Match(domainEventType)).ToArray();
                var instanciatedIntegrationEvents = integrationEventTypes
                    .Where(integrationEvent => specificDomainEventHandlers.Any(listener => integrationEvent.IsInstanciatedBy(listener.TypeDefinition.FullName)))
                    .Select(IntegrationEvent.FromType)
                    .ToArray();

                domainEvent.AddIntegrationEvents(instanciatedIntegrationEvents);
            }

            return domainEvents;
        }

        private IEnumerable<DomainEvent> GetDomainEventInstanciatedByCommand(IEnumerable<TypeDefinition> domainEventTypes) 
            => domainEventTypes
                .Where(domainEvent => domainEvent.IsInstanciatedBy(CommandTypeDefinition.FullName))
                .Select(DomainEvent.FromType)
                .ToArray();

        private IEnumerable<DomainEvent> GetDomainEventInstanciatedByCommandHandler(IEnumerable<TypeDefinition> domainEventTypes)
        {
            if (CommandHandlerTypeDefinition is null) {
                return Enumerable.Empty<DomainEvent>();
            }

            return domainEventTypes
                .Where(domainEvent => domainEvent.IsInstanciatedBy(CommandHandlerTypeDefinition.FullName))
                .Select(DomainEvent.FromType)
                .ToArray();
        }
    }
}