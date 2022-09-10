using BoundedContextCanvasGenerator.Domain.BC.Inbound;

namespace BoundedContextCanvasGenerator.Tests.Acceptance.Utils;

public class DomainFlowBuilder
{
    private List<DomainFlow> domainFlows = new();
    private List<Collaborator> collaborators = new();
    private Command command = null;
    private List<Policy> policies = new();

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

    private DomainFlow Build() => new(collaborators, command, policies);

    public static implicit operator DomainFlow(DomainFlowBuilder builder) => builder.Build();
}