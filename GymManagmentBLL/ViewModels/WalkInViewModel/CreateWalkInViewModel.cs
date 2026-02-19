using System.ComponentModel.DataAnnotations;

namespace GymManagmentBLL.ViewModels.WalkInViewModel
{
    public class CreateWalkInViewModel
    {
        [Required(ErrorMessage = "Required")]
        [MaxLength(100)]
        public string GuestName { get; set; } = null!;

        [Required(ErrorMessage = "Required")]
        [MaxLength(20)]
        public string GuestPhone { get; set; } = null!;

        [Required(ErrorMessage = "Required")]
        [Range(1, 100, ErrorMessage = "RangeError")]
        public int SessionCount { get; set; } = 1;

        // سعر الحصة — يُملأ تلقائياً من GymSettings لكن يمكن تعديله
        [Required(ErrorMessage = "Required")]
        [Range(0.0, 100000, ErrorMessage = "PositivePrice")]
        public decimal PricePerSession { get; set; }

        // إذا كان عضواً مسجلاً مسبقاً (اختياري)
        public int? MemberId { get; set; }

        public string? Notes { get; set; }

        // للعرض فقط
        public decimal TotalAmount => SessionCount * PricePerSession;
    }
}
