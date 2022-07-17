using Catalog.Domain.Items;

namespace Catalog.Application.Items;

public record AdjustItemPriceCommand(CatalogItemId Id, Price NewPrice) : ICommand;

public class AdjustItemPriceCommandHandler : ICommandHandler<AdjustItemPriceCommand>
{
    private readonly ICatalogItemRepository _repository;

    public AdjustItemPriceCommandHandler(ICatalogItemRepository repository) => _repository = repository;

    public async Task Handle(AdjustItemPriceCommand command)
    {
        var catalogItem = await _repository.Get(command.Id);

        catalogItem.AdjustPrice(command.NewPrice);

        await _repository.Save(catalogItem);
    }
}