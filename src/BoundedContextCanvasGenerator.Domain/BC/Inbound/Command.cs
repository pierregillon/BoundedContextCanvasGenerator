using BoundedContextCanvasGenerator.Domain.Types.Definition;

namespace BoundedContextCanvasGenerator.Domain.BC.Inbound;

public record Command(string FriendlyName, TypeFullName TypeFullName)
{
    private const string COMMAND_SUFFIX = "Command";

    public static Command FromType(TypeDefinition typeDefinition)
        => new(
            typeDefinition.FullName.Name.TrimWord(COMMAND_SUFFIX).ToReadableSentence(),
            typeDefinition.FullName
        );
}