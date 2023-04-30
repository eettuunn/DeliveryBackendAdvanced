using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Models;

public class CreateRest
{
    [Required]
    public string Name { get; set; }
}