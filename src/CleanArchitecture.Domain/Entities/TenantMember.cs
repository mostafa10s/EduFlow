

    using CleanArchitecture.Domain.Common;
    using CleanArchitecture.Domain.Enums;


    namespace CleanArchitecture.Domain.Entities;

    public class TenantMember : BaseEntity
    {
        public long TenantId { get; set; }

        public string UserId { get; set; } = string.Empty;

        public TenantMemberRole Role { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }


