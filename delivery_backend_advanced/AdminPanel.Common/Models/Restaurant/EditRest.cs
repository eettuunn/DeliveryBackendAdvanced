using System.ComponentModel.DataAnnotations;

namespace AdminPanel._Common.Models.Restaurant;

public class EditRest
{
    [Required]
    [MinLength(1)]
    public string name { get; set; }

}