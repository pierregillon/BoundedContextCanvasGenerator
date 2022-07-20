using Catalog.Domain.Catalog;
using static Catalog.Domain.Catalog.Catalog;

namespace Catalog.Application.Catalog;

public record RegisterNewCatalogCommand(CatalogName Name, CatalogDescription Description) : ICommand;

public class RegisterNewCatalogCommandHandler : ICommandHandler<RegisterNewCatalogCommand>
{
    private readonly IExistingCatalogFinder _finder;
    private readonly ICatalogRepository _repository;

    public RegisterNewCatalogCommandHandler(IExistingCatalogFinder finder, ICatalogRepository repository)
    {
        _finder = finder;
        _repository = repository;
    }

    public async Task Handle(RegisterNewCatalogCommand command)
    {
        if (await _finder.AnyCatalogWithName(command.Name)) {
            throw new ACatalogWithSameNameAlreadyExists();
        }

        var catalog = Register(command.Name, command.Description);

        await _repository.Save(catalog);
    }
}

public class ACatalogWithSameNameAlreadyExists : Exception { }
