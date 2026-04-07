using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restoran.Backend.DTOs;
using Restoran.Backend.Services;

namespace Restoran.Backend.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize(Roles = "Customer")]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;

    public CartController(CartService cartService) => _cartService = cartService;

    /// <summary>Просмотр корзины</summary>
    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart()
        => Ok(await _cartService.GetCartAsync(GetUserId()));

    /// <summary>Добавить блюдо в корзину</summary>
    [HttpPost]
    public async Task<IActionResult> AddDish([FromBody] AddToCartDto dto)
    {
        var (success, error) = await _cartService.AddDishAsync(GetUserId(), dto.DishId);
        return success ? NoContent() : BadRequest(new { message = error });
    }

    /// <summary>Удалить блюдо из корзины</summary>
    [HttpDelete("{dishId}")]
    public async Task<IActionResult> RemoveDish(Guid dishId)
    {
        var success = await _cartService.RemoveDishAsync(GetUserId(), dishId);
        return success ? NoContent() : NotFound();
    }

    /// <summary>Увеличить количество порций</summary>
    [HttpPost("{dishId}/increase")]
    public async Task<IActionResult> Increase(Guid dishId)
    {
        var success = await _cartService.IncreaseDishAsync(GetUserId(), dishId);
        return success ? NoContent() : NotFound();
    }

    /// <summary>Уменьшить количество порций</summary>
    [HttpPost("{dishId}/decrease")]
    public async Task<IActionResult> Decrease(Guid dishId)
    {
        var success = await _cartService.DecreaseDishAsync(GetUserId(), dishId);
        return success ? NoContent() : NotFound();
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value
            ?? throw new InvalidOperationException());
}
