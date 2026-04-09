using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restoran.AdminPanel.Models;
using Restoran.AdminPanel.Services;

namespace Restoran.AdminPanel.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly AuthApiService _auth;
    private readonly BackendApiService _backend;

    public HomeController(AuthApiService auth, BackendApiService backend)
    {
        _auth = auth;
        _backend = backend;
    }

    public async Task<IActionResult> Index()
    {
        var token = User.FindFirst("jwt_token")?.Value ?? "";

        var usersTask = _auth.GetUsersAsync(token, null, null, null, 1, 1);
        var bannedTask = _auth.GetUsersAsync(token, null, null, true, 1, 1);
        var restaurantsTask = _backend.GetRestaurantsAsync(token, null, 1, 1);

        await Task.WhenAll(usersTask, bannedTask, restaurantsTask);

        var vm = new DashboardViewModel
        {
            TotalUsers = usersTask.Result?.TotalCount ?? 0,
            BannedUsers = bannedTask.Result?.TotalCount ?? 0,
            TotalRestaurants = restaurantsTask.Result?.TotalCount ?? 0,
        };

        return View(vm);
    }
}
