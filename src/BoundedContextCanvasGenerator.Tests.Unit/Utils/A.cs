using BoundedContextCanvasGenerator.Tests.Acceptance.Utils;

namespace BoundedContextCanvasGenerator.Tests.Unit.Utils;

public static class A
{
    public static MethodDefinitionsBuilder MethodDefinitions => new();
    public static MethodDefinitionBuilder MethodDefinition => new();
    public static MethodDefinitionsTypeBuilder Type => new();
    public static TypeDefinitionBuilder Class(string name) => TypeDefinitionBuilder.Class(name);
    public static MethodInfoBuilder Method => new();
    public static InboundCommunicationBuilder InboundCommunication => new();
    public static DomainModuleBuilder DomainModule => new();
    public static DomainFlowBuilder DomainFlow => new();
}

public static class An
{
    public static InstanciatorBuilder Instanciator => new();
}