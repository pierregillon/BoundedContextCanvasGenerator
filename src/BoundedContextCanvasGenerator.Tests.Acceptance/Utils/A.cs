using BoundedContextCanvasGenerator.Domain.BC.Inbound;

namespace BoundedContextCanvasGenerator.Tests.Acceptance.Utils;

internal class A
{
    public static BoundedContextCanvasBuilder BoundedContextCanvas => new();
    public static CanvasDefinitionBuilder CanvasDefinition => new();
    public static InboundCommunicationBuilder InboundCommunication => new();
    public static DomainModuleBuilder DomainModule => new();
    public static DomainFlowBuilder DomainFlow => new();
}