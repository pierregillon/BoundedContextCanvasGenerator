using System.ComponentModel.DataAnnotations;

namespace Catalog.Web.Models;

public class TitleModel
{
    [Required]
    public string Title { get; set; }
}