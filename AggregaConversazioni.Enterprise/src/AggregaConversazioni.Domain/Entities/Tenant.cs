namespace AggregaConversazioni.Domain.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Dictionary<string, object> Settings { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public List<User> Users { get; set; } = new();
    public List<Transformation> Transformations { get; set; } = new();
}
