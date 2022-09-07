using Catalog.Application.Catalog;
using Catalog.Domain.Catalog;
using Catalog.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class CatalogController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;

    public CatalogController(ICommandDispatcher commandDispatcher) => _commandDispatcher = commandDispatcher;

    [HttpPost]
    public async Task Register([FromBody] RegisterNewCatalogModel model)
    {
        await this._commandDispatcher.Dispatch(new RegisterNewCatalogCommand(new CatalogName(model.Name), new CatalogDescription(model.Description)));
    }

    [HttpDelete]
    public async Task Register([FromRoute] Guid catalogId)
    {
        await this._commandDispatcher.Dispatch(new DeleteCatalogCommand(new CatalogId(catalogId)));
    }
}