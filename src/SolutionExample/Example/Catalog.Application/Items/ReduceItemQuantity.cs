using Catalog.Domain.Items;

namespace Catalog.Application.Items;

public record ReduceItemQuantityCommand(CatalogItemId Id, Quantity Quantity) : ICommand;

internal class ReduceItemQuantityCommandHandler : ICommandHandler<ReduceItemQuantityCommand>
{
    private readonly ICatalogItemRepository _repository;

    public ReduceItemQuantityCommandHandler(ICatalogItemRepository repository) => _repository = repository;

    public async Task Handle(ReduceItemQuantityCommand command)
    {
        var item = await _repository.Get(command.Id);

        item.AdjustQuantity(item.CurrentQuantity - command.Quantity);

        await _repository.Save(item);
    }
}