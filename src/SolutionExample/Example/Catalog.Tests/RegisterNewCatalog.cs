using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Application.Catalog;
using Catalog.Domain;
using Catalog.Domain.Catalog;
using Catalog.Domain.Catalog.Events;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Catalog.Tests;

public class RegisterNewCatalog
{
    private readonly ICatalogRepository _repository;
    private readonly IExistingCatalogFinder _finder;
    private readonly RegisterNewCatalogCommandHandler _handler;

    public RegisterNewCatalog()
    {
        _repository = Substitute.For<ICatalogRepository>();
        _finder = Substitute.For<IExistingCatalogFinder>();

        _handler = new RegisterNewCatalogCommandHandler(_finder, _repository);
    }

    [Fact]
    public async Task A_catalog_name_is_unique()
    {
        _finder.AnyCatalogWithName(new CatalogName("Winter season")).Returns(true);

        var command = new RegisterNewCatalogCommand(new CatalogName("Winter season"), CatalogDescription.Empty);

        var creating = () => _handler.Handle(command);

        await creating.Should().ThrowAsync<ACatalogWithSameNameAlreadyExists>();
    }

    [Fact]
    public async Task Registering_a_catalog_raises_registered_event()
    {
        var catalogName = new CatalogName("Winter season");
        var catalogDescription = new CatalogDescription("The best clothes for winter season 2022");

        var command = new RegisterNewCatalogCommand(catalogName, catalogDescription);

        await _handler.Handle(command);

        IEnumerable<IDomainEvent> ExpectedEvents(CatalogId id)
        {
            yield return new CatalogRegistered(id, catalogName, catalogDescription);
        }

        await AssertUncommittedEvents(ExpectedEvents);
    }

    private async Task AssertUncommittedEvents(Func<CatalogId, IEnumerable<IDomainEvent>> assert)
    {
        await _repository
            .Received()
            .Save(Arg.Is<Domain.Catalog.Catalog>(x => x.UncommittedEvents.SequenceEqual(assert(x.Id))));
    }
}