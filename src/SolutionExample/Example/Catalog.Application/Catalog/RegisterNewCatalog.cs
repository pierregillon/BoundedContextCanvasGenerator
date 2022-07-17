using Catalog.Domain.Catalog;
using static Catalog.Domain.Catalog.Catalog;

namespace Catalog.Application.Catalog;

public record RegisterNewCatalogCommand(CatalogDescription Description) : ICommand;

public class RegisterNewCatalogCommandHandler : ICommandHandler<RegisterNewCatalogCommand>
{
    private readonly ICatalogRepository _repository;

    public RegisterNewCatalogCommandHandler(ICatalogRepository repository) => _repository = repository;

    public async Task Handle(RegisterNewCatalogCommand command)
    {
        var catalog = Register(command.Description);

        await _repository.Save(catalog);
    }
}