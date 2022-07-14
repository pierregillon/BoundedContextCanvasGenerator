namespace BoundedContextCanvasGenerator.Domain.Configuration;

public interface IGeneratorConfiguration
{
    public TypeDefinitionPredicates CommandsConfiguration { get; }
    public TypeDefinitionPredicates DomainEventsConfiguration { get; }
}