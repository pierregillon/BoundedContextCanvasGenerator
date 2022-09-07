using System.ComponentModel.DataAnnotations;

namespace Catalog.Web.Models;

public class RegisterNewCatalogModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }
}