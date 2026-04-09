using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Restoran.AdminPanel.Models;

namespace Restoran.AdminPanel.Services;

public class BackendApiService
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public BackendApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<RestaurantPageDto?> GetRestaurantsAsync(string token, string? search, int page, int pageSize)
    {
        SetBearer(token);
        var query = BuildQuery(new Dictionary<string, string?>
        {
            ["search"] = search,
            ["page"] = page.ToString(),
            ["pageSize"] = pageSize.ToString()
        });
        var response = await _http.GetAsync($"/api/restaurants{query}");
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<RestaurantPageDto>(json, JsonOpts);
    }

    public async Task<RestaurantDto?> GetRestaurantAsync(string token, Guid id)
    {
        SetBearer(token);
        var response = await _http.GetAsync($"/api/restaurants/{id}");
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<RestaurantDto>(json, JsonOpts);
    }

    public async Task<RestaurantDto?> CreateRestaurantAsync(string token, CreateRestaurantDto dto)
    {
        SetBearer(token);
        var body = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var response = await _http.PostAsync("/api/restaurants", body);
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<RestaurantDto>(json, JsonOpts);
    }

    public async Task<bool> UpdateRestaurantAsync(string token, Guid id, UpdateRestaurantDto dto)
    {
        SetBearer(token);
        var body = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var response = await _http.PutAsync($"/api/restaurants/{id}", body);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteRestaurantAsync(string token, Guid id)
    {
        SetBearer(token);
        var response = await _http.DeleteAsync($"/api/restaurants/{id}");
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
