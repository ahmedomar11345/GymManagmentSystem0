namespace GymManagmentDAL.Entities.Enums
{
    public enum StockAdjustmentType
    {
        Addition = 0,    // توريد / إضافة مخزون
        Damage = 1,      // تالف
        Loss = 2,        // فقد
        Return = 3,      // مرتجع عميل
        Correction = 4   // تصحيح جرد
    }
}
