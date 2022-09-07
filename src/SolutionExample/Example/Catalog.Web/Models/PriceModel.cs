using System.ComponentModel.DataAnnotations;

namespace Catalog.Web.Models;

public class PriceModel
{
    [Required]
    public double Amount { get; set; }
    [Required]
    public string Currency { get; set; }
}