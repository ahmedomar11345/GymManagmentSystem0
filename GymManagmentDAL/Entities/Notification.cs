using System;

namespace GymManagmentDAL.Entities
{
    public class Notification : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? Url { get; set; }
        public bool IsRead { get; set; } = false;
        public string Type { get; set; } = "info"; // primary, success, warning, danger
        public string? UserId { get; set; } // Null = Global Admin Notification
    }
}
