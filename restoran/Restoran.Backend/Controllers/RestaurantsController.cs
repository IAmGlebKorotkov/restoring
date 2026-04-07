using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restoran.Backend.DTOs;
using Restoran.Backend.Models;
using Restoran.Backend.Services;

namespace Restoran.Backend.Controllers;

[ApiController]
[Route("api/restaurants")]
public class RestaurantsController : ControllerBase
{
    private readonly RestaurantService _restaurantService;

    public RestaurantsController(RestaurantService restaurantService)
        => _restaurantService = restaurantService;

    /// <summary>Список ресторанов с пагинацией и поиском</summary>
    [HttpGet]
    public async Task<ActionResult<RestaurantPageDto>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
        => Ok(await _restaurantService.GetRestaurantsAsync(search, page, pageSize));

    /// <summary>Детали ресторана</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<RestaurantDetailDto>> GetById(Guid id)
    {
        var result = await _restaurantService.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>Создать ресторан</summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<RestaurantDto>> Create([FromBody] CreateRestaurantDto dto)
        => Ok(await _restaurantService.CreateAsync(dto));

    /// <summary>Обновить ресторан</summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<RestaurantDto>> Update(Guid id, [FromBody] UpdateRestaurantDto dto)
    {
        var result = await _restaurantService.UpdateAsync(id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>Удалить ресторан</summary>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _restaurantService.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }

    // ── Меню ────────────────────────────────────────────────────────────────

    /// <summary>Добавить меню к ресторану</summary>
    [HttpPost("{id}/menus")]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult<MenuDto>> CreateMenu(Guid id, [FromBody] CreateMenuDto dto)
    {
        var result = await _restaurantService.CreateMenuAsync(id, dto);
        return result == null ? NotFound("Ресторан не найден") : Ok(result);
    }

    /// <summary>Обновить меню</summary>
    [HttpPut("{id}/menus/{menuId}")]
    [Authorize(Roles = "Manager")]
    public async Task<ActionResult<MenuDto>> UpdateMenu(Guid id, Guid menuId, [FromBody] UpdateMenuDto dto)
    {
        var result = await _restaurantService.UpdateMenuAsync(id, menuId, dto);
        return result == null ? NotFound() : Ok(result);
    }

    /// <summary>Удалить меню</summary>
    [HttpDelete("{id}/menus/{menuId}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> DeleteMenu(Guid id, Guid menuId)
    {
        var success = await _restaurantService.DeleteMenuAsync(id, menuId);
        return success ? NoContent() : NotFound();
    }

    // ── Сотрудники ──────────────────────────────────────────────────────────

    /// <summary>Список сотрудников ресторана</summary>
    [HttpGet("{id}/employees")]
    [Authorize]
    public async Task<ActionResult<List<EmployeeDto>>> GetEmployees(Guid id)
        => Ok(await _restaurantService.GetEmployeesAsync(id));

    /// <summary>Назначить сотрудника в ресторан</summary>
    [HttpPost("{id}/employees")]
    [Authorize]
    public async Task<IActionResult> AssignEmployee(Guid id, [FromQuery] Guid userId, [FromQuery] EmployeeRole role)
    {
        var success = await _restaurantService.AssignEmployeeAsync(id, userId, role);
        return success ? NoContent() : NotFound("Ресторан не найден");
    }

    /// <summary>Убрать сотрудника из ресторана</summary>
    [HttpDelete("{id}/employees/{userId}")]
    [Authorize]
    public async Task<IActionResult> RemoveEmployee(Guid id, Guid userId)
    {
        var success = await _restaurantService.RemoveEmployeeAsync(id, userId);
        return success ? NoContent() : NotFound();
    }
}
