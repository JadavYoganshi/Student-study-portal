using System;

namespace SSP.Models.Domain
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }

        // ✅ Add these two properties if missing:
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }

        public DateTime? LogoutTime { get; set; }
    }
}
