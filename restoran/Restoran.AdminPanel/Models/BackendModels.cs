using System.ComponentModel.DataAnnotations;

namespace Restoran.AdminPanel.Models;

public class RestaurantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string Address { get; set; } = "";
    public string? LogoUrl { get; set; }
}

public class RestaurantPageDto
{
    public List<RestaurantDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class CreateRestaurantDto
{
    [Required(ErrorMessage = "Название обязательно")]
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    [Required(ErrorMessage = "Адрес обязателен")]
    public string Address { get; set; } = "";
    public string? LogoUrl { get; set; }
}

public class UpdateRestaurantDto
{
    [Required(ErrorMessage = "Название обязательно")]
    public string? Name { get; set; }
    public string? Description { get; set; }
    [Required(ErrorMessage = "Адрес обязателен")]
    public string? Address { get; set; }
    public string? LogoUrl { get; set; }
}

public class RestaurantsIndexViewModel
{
    public RestaurantPageDto Page { get; set; } = new();
    public string? Search { get; set; }
    public int CurrentPage { get; set; } = 1;
}

public class DashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalRestaurants { get; set; }
    public int BannedUsers { get; set; }
}
