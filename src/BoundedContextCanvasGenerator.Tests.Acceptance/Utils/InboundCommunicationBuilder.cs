using BoundedContextCanvasGenerator.Domain.BC.Inbound;

namespace BoundedContextCanvasGenerator.Tests.Acceptance.Utils;

public class InboundCommunicationBuilder
{
    private List<DomainModule> domainModules = new();

    private InboundCommunication Build() => new(domainModules);

    public InboundCommunicationBuilder WithModule(DomainModule domainModule)
    {
        this.domainModules.Add(domainModule);
        return this;
    }

    public static implicit operator InboundCommunication(InboundCommunicationBuilder builder) => builder.Build();
}