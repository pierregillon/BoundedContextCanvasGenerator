# Catalog

## Definition

### Description
> Display the product catalog and the items available to purchase. Allows extended search to find a specific item. Provide the ability for administrators to update catalogs and associated new items.

### Strategic classification [(?)](https://github.com/ddd-crew/bounded-context-canvas#strategic-classification)
| Domain | Business Model | Evolution |
| ------------ | ------------ | ------------ |
| *Core domain*<br/>(a key strategic initiative) | *Revenue generator*<br/>(people pay directly for this) | *Commodity*<br/>(highly-standardised versions exist) |

### Domain role [(?)](https://github.com/ddd-crew/bounded-context-canvas/blob/master/resources/model-traits-worksheet.md): *gateway context*
Provide catalog item allowing Basket, Ordering and Payment contexts to properly work.


## Commands
- Catalog.Application.Items.AddItemToCatalogCommand
- Catalog.Application.Items.AdjustItemPriceCommand
- Catalog.Application.Items.EntitleItemCommand
- Catalog.Application.Items.RemoveFromCatalogCommand


## Domain events
- Catalog.Domain.Items.Events.CatalogItemAdded
- Catalog.Domain.Items.Events.CatalogItemEntitled
- Catalog.Domain.Items.Events.CatalogItemPriceAdjusted
- Catalog.Domain.Items.Events.CatalogItemRemoved
