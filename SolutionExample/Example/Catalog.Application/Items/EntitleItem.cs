using Catalog.Domain.Items;

namespace Catalog.Application.Items;

public record EntitleItemCommand(CatalogItemId Id, Title NewTitle) : ICommand;

public class EntitleItemCommandHandler : ICommandHandler<EntitleItemCommand>
{
    private readonly ICatalogItemRepository _repository;

    public EntitleItemCommandHandler(ICatalogItemRepository repository) => _repository = repository;

    public async Task Handle(EntitleItemCommand command)
    {
        var catalogItem = await _repository.Get(command.Id);

        catalogItem.Entitle(command.NewTitle);

        await _repository.Save(catalogItem);
    }
}