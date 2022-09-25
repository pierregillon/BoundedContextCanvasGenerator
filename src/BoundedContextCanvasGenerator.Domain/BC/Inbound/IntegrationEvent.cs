using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.BC.Inbound;

public record IntegrationEvent(string FriendlyName, TypeFullName TypeFullName)
{
    private const string INTEGRATION_EVENT_SUFFIX = "IntegrationEvent";
 
    public static IntegrationEvent FromType(TypeDefinition typeDefinition)
        => new(typeDefinition.FullName.Name.TrimWord(INTEGRATION_EVENT_SUFFIX).ToReadableSentence(), typeDefinition.FullName);

}