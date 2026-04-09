using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restoran.AdminPanel.Models;
using Restoran.AdminPanel.Services;

namespace Restoran.AdminPanel.Controllers;

[Authorize]
public class RestaurantsController : Controller
{
    private readonly BackendApiService _backend;

    public RestaurantsController(BackendApiService backend)
    {
        _backend = backend;
    }

    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        var token = GetToken();
        var result = await _backend.GetRestaurantsAsync(token, search, page, 20);
        var vm = new RestaurantsIndexViewModel
        {
            Page = result ?? new RestaurantPageDto(),
            Search = search,
            CurrentPage = page,
        };
        return View(vm);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateRestaurantDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRestaurantDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var result = await _backend.CreateRestaurantAsync(GetToken(), dto);
        if (result == null)
        {
            ModelState.AddModelError("", "Не удалось создать ресторан. Проверьте данные.");
            return View(dto);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var restaurant = await _backend.GetRestaurantAsync(GetToken(), id);
        if (restaurant == null) return NotFound();

        var dto = new UpdateRestaurantDto
        {
            Name = restaurant.Name,
            Description = restaurant.Description,
            Address = restaurant.Address,
            LogoUrl = restaurant.LogoUrl,
        };
        ViewBag.RestaurantId = id;
        ViewBag.RestaurantName = restaurant.Name;
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateRestaurantDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.RestaurantId = id;
            return View(dto);
        }

        var ok = await _backend.UpdateRestaurantAsync(GetToken(), id, dto);
        if (!ok)
        {
            ModelState.AddModelError("", "Не удалось обновить ресторан.");
            ViewBag.RestaurantId = id;
            return View(dto);
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, string? search, int page = 1)
    {
        await _backend.DeleteRestaurantAsync(GetToken(), id);
        return RedirectToAction(nameof(Index), new { search, page });
    }

    private string GetToken() => User.FindFirst("jwt_token")?.Value ?? "";
}
