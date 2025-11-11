namespace AggregaConversazioni.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Guid? TenantId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<UserRole> Roles { get; set; } = new();

    // Navigation properties
    public Tenant? Tenant { get; set; }
    public List<Transformation> Transformations { get; set; } = new();
}

public class UserRole
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public User? User { get; set; }
}
