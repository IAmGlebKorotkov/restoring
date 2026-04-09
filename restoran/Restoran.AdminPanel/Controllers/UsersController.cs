using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Restoran.AdminPanel.Models;
using Restoran.AdminPanel.Services;

namespace Restoran.AdminPanel.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly AuthApiService _auth;

    public UsersController(AuthApiService auth)
    {
        _auth = auth;
    }

    public async Task<IActionResult> Index(string? search, string? role, bool? isBanned, int page = 1)
    {
        var token = GetToken();
        var pageSize = 20;
        var result = await _auth.GetUsersAsync(token, search, role, isBanned, page, pageSize);

        var vm = new UsersIndexViewModel
        {
            Page = result ?? new UserPageDto(),
            Search = search,
            Role = role,
            IsBanned = isBanned,
            CurrentPage = page,
            PageSize = pageSize,
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ban(Guid id, string? search, string? role, bool? isBanned, int page = 1)
    {
        await _auth.BanUserAsync(GetToken(), id);
        return RedirectToAction(nameof(Index), new { search, role, isBanned, page });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unban(Guid id, string? search, string? role, bool? isBanned, int page = 1)
    {
        await _auth.UnbanUserAsync(GetToken(), id);
        return RedirectToAction(nameof(Index), new { search, role, isBanned, page });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, string? search, string? role, bool? isBanned, int page = 1)
    {
        await _auth.DeleteUserAsync(GetToken(), id);
        return RedirectToAction(nameof(Index), new { search, role, isBanned, page });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(Guid id, string newRole, string? search, string? role, bool? isBanned, int page = 1)
    {
        await _auth.ChangeRoleAsync(GetToken(), id, newRole);
        return RedirectToAction(nameof(Index), new { search, role, isBanned, page });
    }

    private string GetToken() => User.FindFirst("jwt_token")?.Value ?? "";
}
