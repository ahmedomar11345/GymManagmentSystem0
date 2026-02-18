using System;
using System.ComponentModel.DataAnnotations;

namespace GymManagmentBLL.ViewModels.ExpenseViewModel
{
    public class ExpenseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "ExpenseTitleRequired")]
        [Display(Name = "Expense Title")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "AmountRequired")]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be at least 1.")]
        [Display(Name = "Amount (EGP)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "ExpenseDateRequired")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "CategoryRequired")]
        public string Category { get; set; } = null!;

        public string? Notes { get; set; }
    }
}
