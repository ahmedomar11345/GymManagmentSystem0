using GymManagmentBLL.ViewModels.ReportViewModel;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface IReportService
    {
        Task<FinancialReportViewModel> GetFinancialReportAsync(int year);
        Task<byte[]> GenerateFinancialExcelReportAsync(int year);
        Task<byte[]> GenerateFinancialPdfReportAsync(int year);
    }
}
