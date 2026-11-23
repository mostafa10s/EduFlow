namespace CleanArchitecture.Domain.Common;

/// <summary>
/// Base entity with a strongly-typed primary key. All domain entities inherit from this.
/// </summary>
public abstract class BaseEntity
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
