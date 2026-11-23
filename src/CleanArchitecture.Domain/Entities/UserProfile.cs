using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace CleanArchitecture.Domain.Entities;

    public class UserProfile
    {
        public Guid Id { get; set; }

        public string UserId { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        public string? Address { get; set; }

        public string? Gender { get; set; }

        public string? ProfileImageUrl { get; set; }
    }


