using BoundedContextCanvasGenerator.Domain.BC;
using BoundedContextCanvasGenerator.Domain.BC.Definition;
using BoundedContextCanvasGenerator.Domain.BC.Inbound;
using BoundedContextCanvasGenerator.Domain.BC.Ubiquitous;

namespace BoundedContextCanvasGenerator.Tests.Acceptance.Utils;

internal class BoundedContextCanvasBuilder
{
    private CanvasName name = CanvasName.Default;
    private CanvasDefinition definition = CanvasDefinition.Empty;
    private UbiquitousLanguage ubiquitousLanguage = UbiquitousLanguage.None;
    private InboundCommunication inboundCommunication = InboundCommunication.Empty;

    public BoundedContextCanvasBuilder Named(CanvasName canvasName)
    {
        this.name = canvasName;
        return this;
    }

    public BoundedContextCanvasBuilder DefinedAs(CanvasDefinition canvasDefinition)
    {
        this.definition = canvasDefinition;
        return this;
    }

    public BoundedContextCanvasBuilder WithUbiquitousLanguage(UbiquitousLanguage ubiquitousLanguage)
    {
        this.ubiquitousLanguage = ubiquitousLanguage;
        return this;
    }

    public BoundedContextCanvasBuilder WithInboundCommunication(InboundCommunication inboundCommunication)
    {
        this.inboundCommunication = inboundCommunication;
        return this;
    }

    private BoundedContextCanvas Build()
    {
        return new BoundedContextCanvas(
            this.name,
            this.definition,
            this.ubiquitousLanguage,
            this.inboundCommunication
        );
    }

    public static implicit operator BoundedContextCanvas(BoundedContextCanvasBuilder builder) => builder.Build();
}