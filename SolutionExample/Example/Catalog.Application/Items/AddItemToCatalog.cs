using Catalog.Domain.Items;

namespace Catalog.Application.Items;

public record AddItemToCatalogCommand(CatalogId CatalogId, Title Title, Price Price) : ICommand;

public class AddItemToCatalogCommandHandler : ICommandHandler<AddItemToCatalogCommand>
{
    private readonly ICatalogItemRepository _repository;

    public AddItemToCatalogCommandHandler(ICatalogItemRepository repository) => _repository = repository;

    public async Task Handle(AddItemToCatalogCommand command)
    {
        var catalog = await this._repository.GetCatalog(command.CatalogId);

        var catalogItem = catalog.Add(command.Title, command.Price);

        await _repository.Save(catalogItem);
    }
}