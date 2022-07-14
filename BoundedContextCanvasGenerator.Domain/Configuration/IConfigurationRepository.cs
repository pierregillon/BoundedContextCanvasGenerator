namespace BoundedContextCanvasGenerator.Domain.Configuration;

public interface IConfigurationRepository
{
    Task<IGeneratorConfiguration> Get();
}