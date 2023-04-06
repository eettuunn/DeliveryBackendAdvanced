namespace delivery_backend_advanced.Models.Dtos;

public class OrdersPageDto
{
    public List<OrderListElementDto> orders { get; set; } = new();
    
    public PaginationDto pagination { get; set; }
}