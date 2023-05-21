using System.ComponentModel.DataAnnotations;

namespace AdminPanel._Common.Models.Restaurant;

public class CreateRest
{
    [Required]
    public string name { get; set; }

}