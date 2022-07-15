using System.Xml;

namespace BoundedContextCanvasGenerator.Infrastructure.Types;

public class DocumentationComment
{
    private readonly string _xmlDocumentation;

    public DocumentationComment(string xmlDocumentation) => _xmlDocumentation = xmlDocumentation;

    public string GetSummary()
    {
        var doc = new XmlDocument();
        doc.LoadXml(_xmlDocumentation);
        if (doc.DocumentElement is null) {
            throw new InvalidOperationException("Xml document element is missing");
        }
        var node = doc.DocumentElement.SelectSingleNode("/member/summary/text()");
        return node?.Value?.Trim() ?? string.Empty;
    }
}