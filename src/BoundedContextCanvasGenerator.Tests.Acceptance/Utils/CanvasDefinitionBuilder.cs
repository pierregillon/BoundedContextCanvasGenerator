using BoundedContextCanvasGenerator.Domain.BC;
using BoundedContextCanvasGenerator.Domain.BC.Definition;

namespace BoundedContextCanvasGenerator.Tests.Acceptance.Utils;

internal class CanvasDefinitionBuilder
{
    private Text text = Text.Empty;
    private StrategicClassification strategicClassification = StrategicClassification.Empty;
    private DomainRole domainRole = DomainRole.Empty;

    public CanvasDefinitionBuilder DescribedAs(Text text)
    {
        this.text = text;
        return this;
    }

    public CanvasDefinitionBuilder StrategicallyClassifiedAs(StrategicClassification strategicClassification)
    {
        this.strategicClassification = strategicClassification;
        return this;
    }

    public CanvasDefinitionBuilder WithRole(DomainRole domainRole)
    {
        this.domainRole = domainRole;
        return this;
    }

    private CanvasDefinition Build()
    {
        return new CanvasDefinition(
            this.text,
            this.strategicClassification,
            this.domainRole
        );
    }

    public static implicit operator CanvasDefinition(CanvasDefinitionBuilder builder) => builder.Build();
}