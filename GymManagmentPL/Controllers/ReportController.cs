using GymManagmentBLL.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagmentPL.Controllers
{
    [Authorize(Policy = "AdminOrAbove")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> Financial(int? year)
        {
            int reportYear = year ?? DateTime.Now.Year;
            var model = await _reportService.GetFinancialReportAsync(reportYear);
            return View(model);
        }

        public async Task<IActionResult> ExportExcel(int year)
        {
            var content = await _reportService.GenerateFinancialExcelReportAsync(year);
            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"FinancialReport_{year}.xlsx");
        }

        public async Task<IActionResult> ExportPdf(int year)
        {
            var content = await _reportService.GenerateFinancialPdfReportAsync(year);
            return File(content, "application/pdf", $"FinancialReport_{year}.pdf");
        }
    }
}
