using BoundedContextCanvasGenerator.Domain.BC.Inbound;

namespace BoundedContextCanvasGenerator.Tests.Acceptance.Utils;

public class DomainModuleBuilder
{
    private readonly List<DomainFlow> _domainFlows = new();
    private string _name = string.Empty;

    public DomainModuleBuilder WithFlow(DomainFlow domainFlow)
    {
        _domainFlows.Add(domainFlow);
        return this;
    }

    public DomainModuleBuilder Named(string name)
    {
        this._name = name;
        return this;
    }

    public DomainModule Build() => new(this._name, _domainFlows);

    public static implicit operator DomainModule(DomainModuleBuilder builder) => builder.Build();
}