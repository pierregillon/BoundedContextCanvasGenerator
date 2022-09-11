using BoundedContextCanvasGenerator.Domain.Configuration;

namespace BoundedContextCanvasGenerator.Tests.Acceptance.Utils;

internal class InboundCommunicationSettingsBuilder
{
    private TypeDefinitionPredicates _commandPredicates;
    private readonly List<CollaboratorDefinition> _collaboratorDefinitions = new();
    private readonly List<PolicyDefinition> _policyDefinitions = new();

    public InboundCommunicationSettingsBuilder WithCommandMatching(TypeDefinitionPredicates commandTypeDefinitionPredicates)
    {
        _commandPredicates = commandTypeDefinitionPredicates;
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

    private InboundCommunicationSettings Build() =>
        new(
            _commandPredicates,
            _collaboratorDefinitions,
            _policyDefinitions
        );

    public static implicit operator InboundCommunicationSettings(InboundCommunicationSettingsBuilder builder) => builder.Build();
}