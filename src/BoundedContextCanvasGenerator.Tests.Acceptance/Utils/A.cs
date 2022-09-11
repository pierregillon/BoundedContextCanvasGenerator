namespace BoundedContextCanvasGenerator.Tests.Acceptance.Utils;

internal class A
{
    public static BoundedContextCanvasBuilder BoundedContextCanvas => new();
    public static CanvasDefinitionBuilder CanvasDefinition => new();
    public static InboundCommunicationBuilder InboundCommunication => new();
    public static DomainModuleBuilder DomainModule => new();
    public static DomainFlowBuilder DomainFlow => new();
    public static TypeDefinitionBuilder Class(string fullName) => TypeDefinitionBuilder.Class(fullName);
    public static MethodInfoBuilder Method => new();
}

internal class An
{
    public static UbiquitousLanguageBuilder UbiquitousLanguage => new();

    public static InboundCommunicationSettingsBuilder InboundCommunicationSettings = new();
}