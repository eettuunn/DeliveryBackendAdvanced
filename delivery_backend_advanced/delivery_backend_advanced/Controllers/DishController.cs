using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Controllers;

[Route("api/dish")]
public class DishController : ControllerBase
{
    /// <summary>
    /// Get dish details
    /// </summary>
    [HttpGet]
    [Route("{dishId}")]
    public void GetDishDetails(Guid dishId)
    {
        
    }

    /// <summary>
    /// Check ability to rate dish
    /// </summary>
    [HttpGet]
    [Route("{dishId}/rating/check")]
    public bool CheckAbilityToRate(Guid dishId)
    {
        return true;
    }

    /// <summary>
    /// Rate dish
    /// </summary>
    [HttpPost]
    [Route("{dishId}/rating")]
    public void RateDish(Guid dishId)
    {
        
    }
}