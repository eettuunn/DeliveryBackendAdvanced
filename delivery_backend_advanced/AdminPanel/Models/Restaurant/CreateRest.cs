using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Models;

public class CreateRest
{
    [Required]
    public string name { get; set; }
}