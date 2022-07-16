# Catalog

## Definition

### Description
> Display the product catalog and the items available to purchase. Allows extended search to find a specific item. Provide the ability for administrators to update catalogs and associated items.

### Strategic classification [(?)](https://github.com/ddd-crew/bounded-context-canvas#strategic-classification)
| Domain | Business Model | Evolution |
| ------------ | ------------ | ------------ |
| *Core domain*<br/>(a key strategic initiative) | *Revenue generator*<br/>(people pay directly for this) | *Commodity*<br/>(highly-standardised versions exist) |

### Domain role [(?)](https://github.com/ddd-crew/bounded-context-canvas/blob/master/resources/model-traits-worksheet.md): *gateway context*
Provide catalog item allowing Basket, Ordering and Payment contexts to properly work.


## Ubiquitous language (Context-specific domain terminology)
| Catalog | Catalog item |
| ----- | ----- |
| An enumeration of items to purchase. It is systematically described and target a specific audience. | An item of a catalog. It is the minimum unit to purchase. The price includes the currency. |


## Commands
- Catalog.Application.Catalog.DeleteCatalog
- Catalog.Application.Catalog.RegisterNewCatalogCommand
- Catalog.Application.Items.AddItemToCatalogCommand
- Catalog.Application.Items.AdjustItemPriceCommand
- Catalog.Application.Items.EntitleItemCommand
- Catalog.Application.Items.RemoveFromCatalogCommand


## Domain events
- Catalog.Domain.Catalog.Events.CatalogDeleted
- Catalog.Domain.Catalog.Events.CatalogRegistered
- Catalog.Domain.Items.Events.CatalogItemAdded
- Catalog.Domain.Items.Events.CatalogItemEntitled
- Catalog.Domain.Items.Events.CatalogItemPriceAdjusted
- Catalog.Domain.Items.Events.CatalogItemRemoved

