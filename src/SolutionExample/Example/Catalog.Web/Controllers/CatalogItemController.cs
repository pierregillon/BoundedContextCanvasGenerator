using Catalog.Application.Catalog;
using Catalog.Application.Items;
using Catalog.Domain.Catalog;
using Catalog.Domain.Items;
using Catalog.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class CatalogItemController : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;

    public CatalogItemController(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost]
    public async Task AddItem([FromRoute] Guid catalogId, [FromBody] NewCatalogItemModel model)
    {
        await this._commandDispatcher.Dispatch(new AddItemToCatalogCommand(
            new CatalogId(catalogId), 
            new Title(model.Title), 
            new Price(new Amount(model.Price.Amount), new Currency(model.Price.Currency))
        ));
    }

    [HttpPut]
    public async Task AdjustPrice([FromRoute] Guid catalogItemId, [FromBody] PriceModel model)
    {
        await this._commandDispatcher.Dispatch(new AdjustItemPriceCommand(
            new CatalogItemId(catalogItemId), 
            new Price(new Amount(model.Amount), new Currency(model.Currency))
        ));
    }

    [HttpPut]
    public async Task Entitle([FromRoute] Guid catalogItemId, [FromBody] TitleModel model)
    {
        await this._commandDispatcher.Dispatch(new EntitleItemCommand(
            new CatalogItemId(catalogItemId), 
            new Title(model.Title)
        ));
    }

    [HttpDelete]
    public async Task Remove([FromRoute] Guid catalogItemId)
    {
        await this._commandDispatcher.Dispatch(new RemoveFromCatalogCommand(
            new CatalogItemId(catalogItemId)
        ));
    }
}