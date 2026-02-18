using GymManagmentBLL.ViewModels.ExpenseViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface IExpenseService
    {
        Task<IEnumerable<ExpenseViewModel>> GetAllExpensesAsync();
        Task<ExpenseViewModel?> GetExpenseByIdAsync(int id);
        Task AddExpenseAsync(ExpenseViewModel model);
        Task UpdateExpenseAsync(ExpenseViewModel model);
        Task DeleteExpenseAsync(int id);
        Task<decimal> GetTotalExpensesForYearAsync(int year);
    }
}
