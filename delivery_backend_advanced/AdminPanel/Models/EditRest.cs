using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Models;

public class EditRest
{
    [Required]
    [MinLength(1)]
    public string name { get; set; }
}