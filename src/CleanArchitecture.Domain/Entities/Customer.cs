using CleanArchitecture.Domain.Common;

namespace CleanArchitecture.Domain.Entities;

public class Customer : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}".Trim();
}
