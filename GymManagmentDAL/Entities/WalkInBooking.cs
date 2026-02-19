namespace GymManagmentDAL.Entities
{
    /// <summary>
    /// حجز Walk-in: لعضو يريد دفع بالحصة (سواء عضو قديم أو زائر جديد)
    /// </summary>
    public class WalkInBooking : BaseEntity
    {
        // بيانات الشخص (اسم + تليفون فقط — لا يحتاج account كامل)
        public string GuestName { get; set; } = null!;
        public string GuestPhone { get; set; } = null!;

        // إذا كان عضواً مسجلاً مسبقاً — اختياري
        public int? MemberId { get; set; }
        public Member? Member { get; set; }

        // عدد الحصص المدفوعة وسعر الحصة وقت الحجز
        public int SessionCount { get; set; }
        public decimal PricePerSession { get; set; }
        public decimal TotalAmount => SessionCount * PricePerSession;

        // الحصص المستخدمة حتى الآن
        public int SessionsUsed { get; set; } = 0;
        public int SessionsRemaining => SessionCount - SessionsUsed;

        // تاريخ انتهاء صلاحية الحصص (سنة من تاريخ الشراء)
        public DateTime ExpiryDate { get; set; }

        // ملاحظات
        public string? Notes { get; set; }

        // الحصص المرتبطة بهذا الحجز
        public ICollection<WalkInSession> WalkInSessions { get; set; } = new List<WalkInSession>();
    }

    /// <summary>
    /// تسجيل استخدام حصة واحدة من الـ WalkInBooking
    /// </summary>
    public class WalkInSession : BaseEntity
    {
        public int WalkInBookingId { get; set; }
        public WalkInBooking WalkInBooking { get; set; } = null!;

        // الجلسة المحجوزة (اختياري — ممكن يجي بدون جلسة محددة)
        public int? SessionId { get; set; }
        public Session? Session { get; set; }

        public bool IsAttended { get; set; } = false;
        public DateTime? AttendedAt { get; set; }
    }
}
