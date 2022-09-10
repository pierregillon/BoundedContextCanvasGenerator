namespace BoundedContextCanvasGenerator.Domain.BC.Inbound;

public record DomainFlow(IEnumerable<Collaborator> Collaborators, Command Command, IEnumerable<Policy> Policies);