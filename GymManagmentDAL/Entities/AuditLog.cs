using System;

namespace GymManagmentDAL.Entities
{
    public class AuditLog : BaseEntity
    {
        public string? UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Action { get; set; } = null!; // Create, Update, Delete
        public string EntityName { get; set; } = null!;
        public string EntityId { get; set; } = null!;
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string? AffectedColumns { get; set; }
    }
}
