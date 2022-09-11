using BoundedContextCanvasGenerator.Domain.BC.Inbound;

namespace BoundedContextCanvasGenerator.Tests.Acceptance.Utils;

public class DomainFlowBuilder
{
    private readonly List<Collaborator> collaborators = new();
    private Command command = null;
    private readonly List<Policy> policies = new();
    private readonly List<DomainEvent> _domainEvents = new();

    public DomainFlowBuilder WithCommand(Command command)
    {
        this.command = command;
        return this;
    }

    public DomainFlowBuilder WithCollaborator(Collaborator collaborator)
    {
        collaborators.Add(collaborator);
        return this;
    }

    public DomainFlowBuilder WithPolicy(Policy policy)
    {
        this.policies.Add(policy);
        return this;
    }

    public DomainFlowBuilder WithDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
        return this;
    }

    private DomainFlow Build() => new(collaborators, command, policies, this._domainEvents);

    public static implicit operator DomainFlow(DomainFlowBuilder builder) => builder.Build();
}