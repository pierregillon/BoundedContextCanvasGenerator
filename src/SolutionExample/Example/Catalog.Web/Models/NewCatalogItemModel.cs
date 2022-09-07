using System.ComponentModel.DataAnnotations;

namespace Catalog.Web.Models;

public class NewCatalogItemModel
{
    [Required]
    public string Title { get; set; }
    
    [Required]
    public PriceModel Price { get; set; }
}