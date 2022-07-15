using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoundedContextCanvasGenerator.Infrastructure.Types;
using FluentAssertions;
using Xunit;

namespace BoundedContextCanvasGenerator.Tests.Unit.Types
{
    public class DocumentationCommentTests
    {
        [Fact]
        public void Parse()
        {
            const string xmlDocumentation = 
@"<member name=""T:Catalog.Domain.Items.CatalogItem"">
    <summary>
    An item of a catalog. It is the minimum unit to purchase. The price includes the currency.
    </summary>
</member>
";

            var summary = new DocumentationComment(xmlDocumentation).GetSummary();

            summary.Should().Be("An item of a catalog. It is the minimum unit to purchase. The price includes the currency.");
        }
    }
}
