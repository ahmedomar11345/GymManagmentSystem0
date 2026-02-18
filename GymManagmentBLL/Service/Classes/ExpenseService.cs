using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.ExpenseViewModel;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Classes
{
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExpenseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddExpenseAsync(ExpenseViewModel model)
        {
            var expense = new Expense
            {
                Title = model.Title,
                Amount = model.Amount,
                Date = model.Date,
                Category = model.Category,
                Notes = model.Notes
            };

            await _unitOfWork.GetRepository<Expense>().AddAsync(expense);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteExpenseAsync(int id)
        {
            var expense = await _unitOfWork.GetRepository<Expense>().GetByIdAsync(id);
            if (expense != null)
            {
                _unitOfWork.GetRepository<Expense>().Delete(expense);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ExpenseViewModel>> GetAllExpensesAsync()
        {
            var expenses = await _unitOfWork.GetRepository<Expense>().GetAllAsync();
            return expenses.Select(e => new ExpenseViewModel
            {
                Id = e.Id,
                Title = e.Title,
                Amount = e.Amount,
                Date = e.Date,
                Category = e.Category,
                Notes = e.Notes
            }).OrderByDescending(e => e.Date);
        }

        public async Task<ExpenseViewModel?> GetExpenseByIdAsync(int id)
        {
            var expense = await _unitOfWork.GetRepository<Expense>().GetByIdAsync(id);
            if (expense == null) return null;

            return new ExpenseViewModel
            {
                Id = expense.Id,
                Title = expense.Title,
                Amount = expense.Amount,
                Date = expense.Date,
                Category = expense.Category,
                Notes = expense.Notes
            };
        }

        public async Task<decimal> GetTotalExpensesForYearAsync(int year)
        {
            var expenses = await _unitOfWork.GetRepository<Expense>().GetAllAsync(e => e.Date.Year == year);
            return expenses.Sum(e => e.Amount);
        }

        public async Task UpdateExpenseAsync(ExpenseViewModel model)
        {
            var expense = await _unitOfWork.GetRepository<Expense>().GetByIdAsync(model.Id);
            if (expense != null)
            {
                expense.Title = model.Title;
                expense.Amount = model.Amount;
                expense.Date = model.Date;
                expense.Category = model.Category;
                expense.Notes = model.Notes;

                _unitOfWork.GetRepository<Expense>().Update(expense);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
