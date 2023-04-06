namespace delivery_backend_advanced.Models.Dtos;

public class PaginationDto
{
    public int size { get; set; }
    
    public int count { get; set; }
    
    public int current { get; set; }
}