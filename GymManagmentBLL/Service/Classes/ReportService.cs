using ClosedXML.Excel;
using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.ReportViewModel;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Repositories.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Classes
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            // Set QuestPDF license to Community
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<FinancialReportViewModel> GetFinancialReportAsync(int year)
        {
            var memberships = (await _unitOfWork.GetRepository<MemberShip>().GetAllAsync(m => m.CreatedAt.Year == year && !m.IsDeleted)).ToList();
            var plans = (await _unitOfWork.GetRepository<Plane>().GetAllAsync()).ToList();

            var report = new FinancialReportViewModel
            {
                ReportPeriod = year.ToString(),
                ActiveMembersCount = memberships.Count(m => m.Status == "Active"),
                ExpiredMembersCount = memberships.Count(m => m.Status == "Expired"),
                FrozenMembersCount = memberships.Count(m => m.Status == "Frozen"),
                TotalRevenue = 0,
                TotalExpenses = 0 // Assumption 30% for demo if needed
            };

            var monthlyData = new List<MonthlyRevenueViewModel>();
            for (int i = 1; i <= 12; i++)
            {
                var monthMemberships = memberships.Where(m => m.CreatedAt.Month == i).ToList();
                decimal monthRevenue = 0;
                foreach (var ms in monthMemberships)
                {
                    var plan = plans.FirstOrDefault(p => p.Id == ms.PlanId);
                    if (plan != null) monthRevenue += plan.Price;
                }

                monthlyData.Add(new MonthlyRevenueViewModel
                {
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                    Revenue = monthRevenue,
                    SubscriptionsCount = monthMemberships.Count
                });
            }
            report.MonthlyRevenue = monthlyData;
            report.TotalRevenue = monthlyData.Sum(m => m.Revenue);
            
            // Calculate Actual Expenses
            var expenses = await _unitOfWork.GetRepository<Expense>().GetAllAsync(e => e.Date.Year == year && !e.IsDeleted);
            report.TotalExpenses = expenses.Sum(e => e.Amount);

            var planStats = memberships.GroupBy(m => m.PlanId)
                .Select(g => {
                    var plan = plans.FirstOrDefault(p => p.Id == g.Key);
                    return new PlanPopularityViewModel
                    {
                        PlanName = plan?.Name ?? "Unknown",
                        SubscriberCount = g.Count(),
                        TotalRevenue = g.Count() * (plan?.Price ?? 0)
                    };
                }).OrderByDescending(p => p.SubscriberCount).ToList();

            report.TopPlans = planStats;

            return report;
        }

        public async Task<byte[]> GenerateFinancialExcelReportAsync(int year)
        {
            var data = await GetFinancialReportAsync(year);

            using (var workbook = new XLWorkbook())
            {
                var sheet = workbook.Worksheets.Add("Financial Report");
                sheet.RightToLeft = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

                // Header
                sheet.Cell(1, 1).Value = "Gym Financial Report - " + year;
                sheet.Cell(1, 1).Style.Font.Bold = true;
                sheet.Cell(1, 1).Style.Font.FontSize = 16;
                sheet.Range(1, 1, 1, 4).Merge();

                // Summary
                sheet.Cell(3, 1).Value = "Total Revenue";
                sheet.Cell(3, 2).Value = data.TotalRevenue;
                sheet.Cell(4, 1).Value = "Total Expenses";
                sheet.Cell(4, 2).Value = data.TotalExpenses;
                sheet.Cell(5, 1).Value = "Net Profit";
                sheet.Cell(5, 2).Value = data.NetProfit;
                sheet.Range(3, 2, 5, 2).Style.NumberFormat.Format = "#,##0.00";

                // Monthly Breakdown
                sheet.Cell(7, 1).Value = "Month";
                sheet.Cell(7, 2).Value = "Revenue";
                sheet.Cell(7, 3).Value = "Subscriptions";
                sheet.Range(7, 1, 7, 3).Style.Fill.BackgroundColor = XLColor.LightGray;

                int row = 8;
                foreach (var m in data.MonthlyRevenue)
                {
                    sheet.Cell(row, 1).Value = m.MonthName;
                    sheet.Cell(row, 2).Value = m.Revenue;
                    sheet.Cell(row, 3).Value = m.SubscriptionsCount;
                    row++;
                }

                // Plan Popularity
                row += 2;
                sheet.Cell(row, 1).Value = "Plan Name";
                sheet.Cell(row, 2).Value = "Subscribers";
                sheet.Cell(row, 3).Value = "Total Revenue";
                sheet.Range(row, 1, row, 3).Style.Fill.BackgroundColor = XLColor.LightSkyBlue;
                row++;

                foreach (var p in data.TopPlans)
                {
                    sheet.Cell(row, 1).Value = p.PlanName;
                    sheet.Cell(row, 2).Value = p.SubscriberCount;
                    sheet.Cell(row, 3).Value = p.TotalRevenue;
                    row++;
                }

                sheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public async Task<byte[]> GenerateFinancialPdfReportAsync(int year)
        {
            var data = await GetFinancialReportAsync(year);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Segoe UI"));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Flex Gym Management").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                            col.Item().Text("Financial Analytics Report").FontSize(14);
                            col.Item().Text($"Reporting Year: {year}");
                        });

                        row.ConstantItem(100).AlignRight().Text(DateTime.Now.ToString("d")).FontSize(10);
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        // Summary Cards
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Padding(5).Border(1).BorderColor(Colors.Grey.Lighten2).Column(c =>
                            {
                                c.Item().AlignCenter().Text("Total Revenue").FontSize(10).FontColor(Colors.Grey.Medium);
                                c.Item().AlignCenter().Text(data.TotalRevenue.ToString("N2") + " EGP").FontSize(14).Bold();
                            });
                            row.RelativeItem().Padding(5).Border(1).BorderColor(Colors.Grey.Lighten2).Column(c =>
                            {
                                c.Item().AlignCenter().Text("Net Profit").FontSize(10).FontColor(Colors.Grey.Medium);
                                c.Item().AlignCenter().Text(data.NetProfit.ToString("N2") + " EGP").FontSize(14).Bold().FontColor(Colors.Green.Medium);
                            });
                        });

                        col.Item().PaddingTop(20).Text("Monthly Revenue Breakdown").FontSize(14).SemiBold();
                        col.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Month");
                                header.Cell().Element(CellStyle).Text("Revenue");
                                header.Cell().Element(CellStyle).Text("Subscribers");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            foreach (var m in data.MonthlyRevenue)
                            {
                                table.Cell().Element(CellStyle).Text(m.MonthName);
                                table.Cell().Element(CellStyle).Text(m.Revenue.ToString("N2"));
                                table.Cell().Element(CellStyle).Text(m.SubscriptionsCount.ToString());

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten3);
                                }
                            }
                        });

                        col.Item().PaddingTop(30).Text("Membership Plan Statistics").FontSize(14).SemiBold();
                        col.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Plan Name");
                                header.Cell().Element(CellStyle).Text("Subscribers");
                                header.Cell().Element(CellStyle).Text("Revenue");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            foreach (var p in data.TopPlans)
                            {
                                table.Cell().Element(CellStyle).Text(p.PlanName);
                                table.Cell().Element(CellStyle).Text(p.SubscriberCount.ToString());
                                table.Cell().Element(CellStyle).Text(p.TotalRevenue.ToString("N2"));

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten3);
                                }
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
                });
            });

            using (var stream = new MemoryStream())
            {
                document.GeneratePdf(stream);
                return stream.ToArray();
            }
        }
    }
}
