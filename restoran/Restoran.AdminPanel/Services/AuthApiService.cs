using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Restoran.AdminPanel.Models;

namespace Restoran.AdminPanel.Services;

public class AuthApiService
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public AuthApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<TokenResponseDto?> LoginAsync(LoginDto dto)
    {
        var body = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var response = await _http.PostAsync("/api/auth/login", body);
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TokenResponseDto>(json, JsonOpts);
    }

    public async Task<UserPageDto?> GetUsersAsync(string token, string? search, string? role, bool? isBanned, int page, int pageSize)
    {
        SetBearer(token);
        var query = BuildQuery(new Dictionary<string, string?>
        {
            ["search"] = search,
            ["role"] = role,
            ["isBanned"] = isBanned?.ToString().ToLower(),
            ["page"] = page.ToString(),
            ["pageSize"] = pageSize.ToString()
        });
        var response = await _http.GetAsync($"/api/admin/users{query}");
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserPageDto>(json, JsonOpts);
    }

    public async Task<UserProfileDto?> GetUserAsync(string token, Guid userId)
    {
        SetBearer(token);
        var response = await _http.GetAsync($"/api/auth/users/{userId}");
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserProfileDto>(json, JsonOpts);
    }

    public async Task<bool> BanUserAsync(string token, Guid userId)
    {
        SetBearer(token);
        var response = await _http.PostAsync($"/api/admin/users/{userId}/ban", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UnbanUserAsync(string token, Guid userId)
    {
        SetBearer(token);
        var response = await _http.PostAsync($"/api/admin/users/{userId}/unban", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteUserAsync(string token, Guid userId)
    {
        SetBearer(token);
        var response = await _http.DeleteAsync($"/api/admin/users/{userId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ChangeRoleAsync(string token, Guid userId, string newRole)
    {
        SetBearer(token);
        var response = await _http.PutAsync($"/api/admin/users/{userId}/role?newRole={newRole}", null);
        return response.IsSuccessStatusCode;
    }

    private void SetBearer(string token)
    {
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private static string BuildQuery(Dictionary<string, string?> params_)
    {
        var parts = params_
            .Where(p => p.Value != null)
            .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value!)}");
        var qs = string.Join("&", parts);
        return qs.Length > 0 ? "?" + qs : "";
    }
}
