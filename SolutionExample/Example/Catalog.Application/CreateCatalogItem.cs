using Catalog.Domain;

namespace Catalog;

public record CreateCatalogItemCommand(string Name, Price Price) : ICommand;

public class CreateCatalogItemCommandHandler : ICommandHandler<CreateCatalogItemCommand>
{
    private readonly ICatalogItemRepository _repository;

    public CreateCatalogItemCommandHandler(ICatalogItemRepository repository) => _repository = repository;

    public async Task Handle(CreateCatalogItemCommand command)
    {
        var catalogItem = CatalogItem.New(command.Name, command.Price);

        await _repository.Save(catalogItem);
    }
}