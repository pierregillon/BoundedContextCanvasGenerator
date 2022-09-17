using Catalog.Application.Items;
using Catalog.Domain.Items;
using Catalog.Web;

namespace Catalog.Infrastructure.Order;

internal class AdjustItemQuantityOnItemOrdered : IBusListener<ItemOrderedIntegrationEvent>
{
    private readonly ICommandDispatcher _commandDispatcher;

    public AdjustItemQuantityOnItemOrdered(ICommandDispatcher commandDispatcher) => _commandDispatcher = commandDispatcher;

    public async Task On(ItemOrderedIntegrationEvent @event)
    {
        await _commandDispatcher.Dispatch(new ReduceItemQuantityCommand(new CatalogItemId(@event.ItemId), new Quantity(@event.OrderedQuantity)));
    }
}