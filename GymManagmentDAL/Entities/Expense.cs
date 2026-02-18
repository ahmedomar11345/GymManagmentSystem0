using System;
using System.ComponentModel.DataAnnotations;

namespace GymManagmentDAL.Entities
{
    public class Expense : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = null!; // e.g., Rent, Utilities, Salaries, Maintenance, Equipment

        public string? Notes { get; set; }
    }
}
