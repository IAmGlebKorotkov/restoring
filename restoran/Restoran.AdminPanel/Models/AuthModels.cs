namespace Restoran.AdminPanel.Models;

public class LoginViewModel
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginDto
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class TokenResponseDto
{
    public string AccessToken { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public string RefreshToken { get; set; } = "";
    public Guid UserId { get; set; }
    public string Email { get; set; } = "";
    public List<string> Roles { get; set; } = new();
}

public class UserListDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = "";
    public string FullName { get; set; } = "";
    public bool IsBanned { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class UserPageDto
{
    public List<UserListDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = "";
    public string FullName { get; set; } = "";
    public DateTime BirthDate { get; set; }
    public string? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class UsersIndexViewModel
{
    public UserPageDto Page { get; set; } = new();
    public string? Search { get; set; }
    public string? Role { get; set; }
    public bool? IsBanned { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
