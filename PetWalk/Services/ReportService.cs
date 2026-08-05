using System;
using System.Collections.Generic;
using System.Linq;
using PetWalk.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PetWalk.Services
{
    public class ReportService
    {
        public ReportService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public void GeneratePdfReport(List<Walk> walks, User user, string filePath)
        {
            var completedWalks = walks.Where(w => w.Status == WalkStatus.Completed).ToList();
            var totalMoney = completedWalks.Sum(w => w.Price);
            string moneyLabel = user is Walker ? "Total earned" : "Total spent";

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);

                    // Header
                    page.Header().Column(col =>
                    {
                        col.Item().Text("PetWalk - Walk Report")
                            .FontSize(24).Bold().FontColor(Colors.Green.Darken2);
                        col.Item().Text($"Generated: {DateTime.Now:dd.MM.yyyy HH:mm}")
                            .FontSize(10).FontColor(Colors.Grey.Darken1);
                        col.Item().PaddingBottom(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    // Content
                    page.Content().Column(col =>
                    {
                        // User info
                        col.Item().PaddingBottom(15).Column(info =>
                        {
                            info.Item().Text($"User: {user.GetFullName()}").FontSize(14).Bold();
                            info.Item().Text($"Email: {user.Email}").FontSize(11);
                            info.Item().Text($"Location: {user.Location}").FontSize(11);
                        });

                        // Summary
                        col.Item().PaddingBottom(15).Background(Colors.Grey.Lighten4).Padding(10).Column(summary =>
                        {
                            summary.Item().Text("Summary").FontSize(14).Bold();
                            summary.Item().Text($"Total walks: {walks.Count}").FontSize(11);
                            summary.Item().Text($"Completed: {completedWalks.Count}").FontSize(11);
                            summary.Item().Text($"Scheduled: {walks.Count(w => w.Status == WalkStatus.Scheduled)}").FontSize(11);
                            summary.Item().Text($"Cancelled: {walks.Count(w => w.Status == WalkStatus.Cancelled)}").FontSize(11);
                            summary.Item().Text($"Total spent: {moneyLabel:F2}€").FontSize(11).Bold();
                        });

                        // Walk table
                        col.Item().PaddingBottom(5).Text("Walk Details").FontSize(14).Bold();

                        if (walks.Count > 0)
                        {
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);  // Date
                                    columns.RelativeColumn(2);  // Dog
                                    columns.RelativeColumn(3);  // Walker/Owner
                                    columns.RelativeColumn(2);  // Duration
                                    columns.RelativeColumn(2);  // Status
                                    columns.RelativeColumn(2);  // Price
                                });

                                // Header row
                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Green.Darken2).Padding(5)
                                        .Text("Date").FontColor(Colors.White).FontSize(10).Bold();
                                    header.Cell().Background(Colors.Green.Darken2).Padding(5)
                                        .Text("Dog").FontColor(Colors.White).FontSize(10).Bold();
                                    header.Cell().Background(Colors.Green.Darken2).Padding(5)
                                        .Text("Walker/Owner").FontColor(Colors.White).FontSize(10).Bold();
                                    header.Cell().Background(Colors.Green.Darken2).Padding(5)
                                        .Text("Duration").FontColor(Colors.White).FontSize(10).Bold();
                                    header.Cell().Background(Colors.Green.Darken2).Padding(5)
                                        .Text("Status").FontColor(Colors.White).FontSize(10).Bold();
                                    header.Cell().Background(Colors.Green.Darken2).Padding(5)
                                        .Text("Price").FontColor(Colors.White).FontSize(10).Bold();
                                });

                                // Data rows
                                foreach (var walk in walks.OrderByDescending(w => w.ScheduledDate))
                                {
                                    string dogName = walk.Dog?.Name ?? "N/A";
                                    string otherUser = walk.Walker?.GetFullName() ?? walk.Owner?.GetFullName() ?? "N/A";
                                    var bgColor = walks.IndexOf(walk) % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                                    table.Cell().Background(bgColor).Padding(5)
                                        .Text(walk.ScheduledDate.ToString("dd.MM.yyyy HH:mm")).FontSize(9);
                                    table.Cell().Background(bgColor).Padding(5)
                                        .Text(dogName).FontSize(9);
                                    table.Cell().Background(bgColor).Padding(5)
                                        .Text(otherUser).FontSize(9);
                                    table.Cell().Background(bgColor).Padding(5)
                                        .Text($"{walk.Duration} min").FontSize(9);
                                    table.Cell().Background(bgColor).Padding(5)
                                        .Text(walk.Status.ToString()).FontSize(9);
                                    table.Cell().Background(bgColor).Padding(5)
                                        .Text($"{walk.Price:F2}€").FontSize(9);
                                }
                            });
                        }
                        else
                        {
                            col.Item().Text("No walks recorded.").FontSize(11).Italic();
                        }
                    });

                    // Footer
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("PetWalk © 2026 | Page ").FontSize(9).FontColor(Colors.Grey.Darken1);
                        text.CurrentPageNumber().FontSize(9);
                        text.Span(" of ").FontSize(9).FontColor(Colors.Grey.Darken1);
                        text.TotalPages().FontSize(9);
                    });
                });
            }).GeneratePdf(filePath);
        }
    }
}