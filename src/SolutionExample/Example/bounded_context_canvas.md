# Catalog

## Definition

### Description

> Display the product catalog and the items available to purchase. Allows extended search to find a specific item. Provide the ability for administrators to update catalogs and associated items.

### Strategic classification [(?)](https://github.com/ddd-crew/bounded-context-canvas#strategic-classification)

| Domain                                         | Business Model                                         | Evolution                                             |
| ---------------------------------------------- | ------------------------------------------------------ | ----------------------------------------------------- |
| *Core domain*<br/>(a key strategic initiative) | *Revenue generator*<br/>(people pay directly for this) | *Commodity*<br/>(highly\-standardised versions exist) |

### Domain role [(?)](https://github.com/ddd-crew/bounded-context-canvas/blob/master/resources/model-traits-worksheet.md): *gateway context*

Provide catalog item allowing Basket, Ordering and Payment contexts to properly work.

## Ubiquitous language (Context\-specific domain terminology)

| Catalog                                                                                             | Catalog item                                                                               |
| --------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------ |
| An enumeration of items to purchase. It is systematically described and target a specific audience. | An item of a catalog. It is the minimum unit to purchase. The price includes the currency. |

## Inbound communication

### Catalog

---

```mermaid
flowchart LR
    classDef collaborators fill:#FFE5FF;
    classDef domainEvents fill:#FFA431;
    classDef policies fill:#FFFFAD, font-style:italic;
    CatalogApplicationCatalogDeleteCatalogCommand["Delete catalog"]
    CatalogApplicationCatalogDeleteCatalogCommandWebAppCollaborator>"Web app"]
    class CatalogApplicationCatalogDeleteCatalogCommandWebAppCollaborator collaborators;
    CatalogDomainCatalogEventsCatalogDeleted["Catalog deleted"]
    class CatalogDomainCatalogEventsCatalogDeleted domainEvents;
    CatalogApplicationCatalogRegisterNewCatalogCommand["Register new catalog"]
    CatalogApplicationCatalogRegisterNewCatalogCommandWebAppCollaborator>"Web app"]
    class CatalogApplicationCatalogRegisterNewCatalogCommandWebAppCollaborator collaborators;
    CatalogApplicationCatalogRegisterNewCatalogCommandPolicies[/"A catalog name is unique<br/>Registering a catalog raises registered event"/]
    class CatalogApplicationCatalogRegisterNewCatalogCommandPolicies policies;
    CatalogDomainCatalogEventsCatalogRegistered["Catalog registered"]
    class CatalogDomainCatalogEventsCatalogRegistered domainEvents;
    CatalogApplicationCatalogDeleteCatalogCommandWebAppCollaborator --> CatalogApplicationCatalogDeleteCatalogCommand
    CatalogApplicationCatalogDeleteCatalogCommand -.-> CatalogDomainCatalogEventsCatalogDeleted
    CatalogApplicationCatalogRegisterNewCatalogCommandWebAppCollaborator --> CatalogApplicationCatalogRegisterNewCatalogCommand
    CatalogApplicationCatalogRegisterNewCatalogCommand --- CatalogApplicationCatalogRegisterNewCatalogCommandPolicies
    CatalogApplicationCatalogRegisterNewCatalogCommandPolicies -.-> CatalogDomainCatalogEventsCatalogRegistered
```

### Items

---

```mermaid
flowchart LR
    classDef collaborators fill:#FFE5FF;
    classDef domainEvents fill:#FFA431;
    CatalogApplicationItemsAddItemToCatalogCommand["Add item to catalog"]
    CatalogApplicationItemsAddItemToCatalogCommandWebAppCollaborator>"Web app"]
    class CatalogApplicationItemsAddItemToCatalogCommandWebAppCollaborator collaborators;
    CatalogApplicationItemsAdjustItemPriceCommand["Adjust item price"]
    CatalogApplicationItemsAdjustItemPriceCommandWebAppCollaborator>"Web app"]
    class CatalogApplicationItemsAdjustItemPriceCommandWebAppCollaborator collaborators;
    CatalogDomainItemsEventsCatalogItemPriceAdjusted["Catalog item price adjusted"]
    class CatalogDomainItemsEventsCatalogItemPriceAdjusted domainEvents;
    CatalogApplicationItemsEntitleItemCommand["Entitle item"]
    CatalogApplicationItemsEntitleItemCommandWebAppCollaborator>"Web app"]
    class CatalogApplicationItemsEntitleItemCommandWebAppCollaborator collaborators;
    CatalogDomainItemsEventsCatalogItemEntitled["Catalog item entitled"]
    class CatalogDomainItemsEventsCatalogItemEntitled domainEvents;
    CatalogApplicationItemsRemoveFromCatalogCommand["Remove from catalog"]
    CatalogApplicationItemsRemoveFromCatalogCommandWebAppCollaborator>"Web app"]
    class CatalogApplicationItemsRemoveFromCatalogCommandWebAppCollaborator collaborators;
    CatalogDomainItemsEventsCatalogItemRemoved["Catalog item removed"]
    class CatalogDomainItemsEventsCatalogItemRemoved domainEvents;
    CatalogApplicationItemsAddItemToCatalogCommandWebAppCollaborator --> CatalogApplicationItemsAddItemToCatalogCommand
    CatalogApplicationItemsAdjustItemPriceCommandWebAppCollaborator --> CatalogApplicationItemsAdjustItemPriceCommand
    CatalogApplicationItemsAdjustItemPriceCommand -.-> CatalogDomainItemsEventsCatalogItemPriceAdjusted
    CatalogApplicationItemsEntitleItemCommandWebAppCollaborator --> CatalogApplicationItemsEntitleItemCommand
    CatalogApplicationItemsEntitleItemCommand -.-> CatalogDomainItemsEventsCatalogItemEntitled
    CatalogApplicationItemsRemoveFromCatalogCommandWebAppCollaborator --> CatalogApplicationItemsRemoveFromCatalogCommand
    CatalogApplicationItemsRemoveFromCatalogCommand -.-> CatalogDomainItemsEventsCatalogItemRemoved
```
