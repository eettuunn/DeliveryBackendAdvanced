using Microsoft.AspNetCore.Http.HttpResults;

namespace delivery_backend_advanced.Models.Enums;

public enum OrderStatus
{
    Created,
    Kitchen,
    Packaging,
    Delivery,
    Delivered,
    Canceled
}