namespace BoundedContextCanvasGenerator.Domain.Configuration;

public interface IYamlFileReader
{
    Task<IGeneratorConfiguration> Read(string filePath);
}