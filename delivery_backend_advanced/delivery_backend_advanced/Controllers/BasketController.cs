using Microsoft.AspNetCore.Mvc;

namespace delivery_backend_advanced.Controllers;

[Route("api/basket")]
public class BasketController : ControllerBase
{
    /// <summary>
    /// Get user's basket
    /// </summary>
    [HttpGet]
    public void GetUserBasket()
    {
        
    }
    
    /// <summary>
    /// Add dish to basket
    /// </summary>
    [HttpPost]
    public void AddDishToBasket()
    {
        
    }

    /// <summary>
    /// Increase number of dishes in basket
    /// </summary>
    [HttpPatch]
    [Route("{dishId}/increase")]
    public void IncreaseDishNumber(Guid dishId)
    {
        
    }
    
    /// <summary>
    /// Reduce number of dishes in basket
    /// </summary>
    [HttpPatch]
    [Route("{dishId}/reduce")]
    public void ReduceDishNumber(Guid dishId)
    {
        
    }

    /// <summary>
    /// Delete dish from basket completely 
    /// </summary>
    [HttpDelete]
    [Route("{dishId}")]
    public void DeleteDishFromBasket(Guid dishId)
    {
        
    }
}