using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Tests.Acceptance.Utils;

internal class InboundCommunicationSettingsBuilder
{
    private CommandDefinition _commandDefinition = CommandDefinition.Empty;
    private readonly List<CollaboratorDefinition> _collaboratorDefinitions = new();
    private readonly List<PolicyDefinition> _policyDefinitions = new();
    private DomainEventDefinition _domainEventDefinition = DomainEventDefinition.Empty;
    private IntegrationEventDefinition _integrationEventDefinition = IntegrationEventDefinition.Empty;

    public InboundCommunicationSettingsBuilder WithCommandDefinition(CommandDefinition commandDefinition)
    {
        _commandDefinition = commandDefinition;
        return this;
    }

    public InboundCommunicationSettingsBuilder WithCollaboratorDefinition(CollaboratorDefinition collaboratorDefinition)
    {
        _collaboratorDefinitions.Add(collaboratorDefinition);
        return this;
    }

    public InboundCommunicationSettingsBuilder WithPolicyDefinition(PolicyDefinition policyDefinition)
    {
        _policyDefinitions.Add(policyDefinition);
        return this;
    }

    public InboundCommunicationSettingsBuilder WithDomainEventDefinition(DomainEventDefinition domainEventDefinition)
    {
        this._domainEventDefinition = domainEventDefinition;
        return this;
    }

    public InboundCommunicationSettingsBuilder WithIntegrationEventDefinition(IntegrationEventDefinition integrationEventDefinition)
    {
        this._integrationEventDefinition = integrationEventDefinition;
        return this;
    }

    private InboundCommunicationSettings Build() =>
        new(
            _commandDefinition,
            _collaboratorDefinitions,
            _policyDefinitions,
            _domainEventDefinition,
            _integrationEventDefinition
        );

    public static implicit operator InboundCommunicationSettings(InboundCommunicationSettingsBuilder builder) => builder.Build();
}